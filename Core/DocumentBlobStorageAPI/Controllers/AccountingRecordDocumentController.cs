using Azure;
using Azure.Identity;
using Core.BlobStorage;
using Core.Configuration;
using Core.Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO.Compression;

namespace DocumentBlobStorageAPI.Controllers
{
    [Route("api/accountingrecords/document")]
    [ApiController]
    public class AccountingRecordDocumentController : ControllerBase
    {
        private IAccountingRecordBlobStorageProvider _BlobStorageProvider;

        public AccountingRecordDocumentController(IAccountingRecordBlobStorageProvider BlobStorageProvider)
        {
            _BlobStorageProvider = BlobStorageProvider;
        }

        //[HttpGet]
        //public string Get()
        //{
        //    string message = "Success";
        //    try
        //    {
        //        message = _AppConfiguration["TestApp:Settings:Message"] ?? "No data found";
        //        //var azureAppConfigEndpoint = Environment.GetEnvironmentVariable("AzureAppConfigEndpoint");
        //        //var builder = new ConfigurationBuilder();
        //        //if (!string.IsNullOrEmpty(azureAppConfigEndpoint))
        //        //{
        //        //    builder.AddAzureAppConfiguration(options =>
        //        //    {
        //        //        options.Connect(new Uri(azureAppConfigEndpoint), new DefaultAzureCredential())
        //        //        .ConfigureKeyVault(options =>
        //        //        {
        //        //            options.SetCredential(new DefaultAzureCredential());
        //        //        });
        //        //    });
        //        //}
        //        //_Configuration = builder.Build();
        //        //message = _Configuration["TestApp:Settings:Message"];
        //    }
        //    catch (Exception ex)
        //    {
        //        message = ex.Message;
        //    }

        //    return message;
        //}

        [HttpPost]
        [Route("uploadDocumentListAsync")]
        public async Task<IActionResult> UploadDocumentListAsync(List<IFormFile> files)
        {
            var response = await _BlobStorageProvider.UploadDocumentListAsync(files);
            return Ok(response);
        }

        [HttpPost]
        [Route("downloadDocumentAsync")]
        public async Task<IActionResult> DownloadDocumentAsync(string BlobName)
        {
            try
            {
                var documentDownloadResponse = await _BlobStorageProvider.DownloadDocumentAsync(BlobName);
                return File(documentDownloadResponse.DownloadStream, documentDownloadResponse.MetadataHash["ContentType"], documentDownloadResponse.MetadataHash["FileName"]);
            }
            catch(FileNotFoundException ex) 
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Route("downloadDocumentListAsync")]
        public async Task<IActionResult> DownloadDocumentListAsync(ICollection<string> BlobNameList, string ZipFileName)
        {
            using(var memstream = new MemoryStream())
            {
                var documentDownloadResponseList = await _BlobStorageProvider.DownloadDocumentListAsync(BlobNameList);                
                using (var zipArchive = new ZipArchive(memstream, ZipArchiveMode.Create, true))
                {
                    foreach (var documentDownloadResponse in documentDownloadResponseList)
                    {
                        var fileName = documentDownloadResponse.MetadataHash["FileName"];
                        var fileEntryInZip = zipArchive.CreateEntry(fileName);
                        using (var fileStream = fileEntryInZip.Open())
                        {
                            await documentDownloadResponse.DownloadStream.CopyToAsync(fileStream);
                        }
                    }
                }
                memstream.Position = 0;
                return File(memstream.ToArray(), "application/zip", ZipFileName);
            }           
        }
    }
}
