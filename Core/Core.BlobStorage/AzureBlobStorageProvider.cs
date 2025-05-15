using Azure.Core;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Core.Configuration;
using Core.Logger;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Cryptography;

namespace Core.BlobStorage
{
    public class DownloadDocumentResponse
    {
        public IDictionary<string,string> MetadataHash { get; set; }
        public Stream DownloadStream { get; set; }
    }
    internal class AzureBlobProvider
    {
        BlobServiceClient _BlobClient;
        BlobContainerClient _ContainerClient;

        public AzureBlobProvider(string connectionEndpoint, string geosecondaryConnectionEndpoint, string containerName)
        {
            //var connectionEndpoint = ConfigurationUtility.ConfigurationManager["AzureStorageConnection:PrimaryConnectionEndpoint"];
            //var geosecondaryConnectionEndpoint = ConfigurationUtility.ConfigurationManager["AzureStorageConnection:SecondaryConnectionEndpoint"];
            //var containerName = ConfigurationUtility.ConfigurationManager["AzureStorageConnection:AccountingRecordsDocumentContainerName"];
            BlobClientOptions blobOptions = new BlobClientOptions()
            {
                Retry = {
        Delay = TimeSpan.FromSeconds(2),
        MaxRetries = 5,
        Mode = RetryMode.Exponential,
        MaxDelay = TimeSpan.FromSeconds(10),
        NetworkTimeout = TimeSpan.FromSeconds(100)
    },
                GeoRedundantSecondaryUri = string.IsNullOrEmpty(geosecondaryConnectionEndpoint) ? null : new Uri(geosecondaryConnectionEndpoint)
                
            };
            _BlobClient = new BlobServiceClient(connectionEndpoint, blobOptions);
            _ContainerClient = _BlobClient.GetBlobContainerClient(containerName);
        }

        public async Task<List<DocumentUploadContentInfo>> UploadDocuments(List<IFormFile> files)
        {
            var documentUploadContentInfoList = new List<DocumentUploadContentInfo>();
            foreach (var file in files)
            {
                try
                {
                    var blobGuidName = Guid.NewGuid().ToString();
                    Azure.Response<BlobContentInfo> response;
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        var expectedMd5Hash = CalculateMd5(file);
                        memoryStream.Position = 0;
                        var bobclient = _ContainerClient.GetBlobClient(blobGuidName);
                        var uri = bobclient.GenerateSasUri(BlobSasPermissions.Write, DateTimeOffset.UtcNow.AddHours(1));
                        BlobClient blobClientWithSas = new BlobClient(uri);
                        response = await blobClientWithSas.UploadAsync(memoryStream, new BlobUploadOptions { TransferValidation = new UploadTransferValidationOptions { ChecksumAlgorithm = StorageChecksumAlgorithm.MD5 } });
                        var metadata = new Dictionary<string, string>
                        {
                            { "FileName", file.FileName },
                            { "ContentType", file.ContentType }
                        };
                        await blobClientWithSas.SetMetadataAsync(metadata);

                        if (response != null && response.Value != null)
                        {
                            var documentUploadContentInfo = new DocumentUploadContentInfo();
                            BlobProperties properties = bobclient.GetProperties();
                            var uploadedHash = Convert.ToBase64String(response.Value.ContentHash);
                            var isHashEqual = uploadedHash.Equals(expectedMd5Hash, StringComparison.OrdinalIgnoreCase);
                            if (!isHashEqual)
                            {
                                documentUploadContentInfo = new DocumentUploadContentInfo { FileName = file.FileName, ErrorMessage = $"Hashes does not match, please retry uploading" };
                            }
                            else
                            {
                                documentUploadContentInfo = new DocumentUploadContentInfo { FileName = file.FileName, BlobName = blobGuidName, ETag = response.Value.ETag, ContentHash = response.Value.ContentHash };
                            }
                            documentUploadContentInfoList.Add(documentUploadContentInfo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await ApplicationLogger<AzureBlobProvider>.ErrorAsync(ex.Message);
                }
            }
            return documentUploadContentInfoList;
        }

        public async Task<DownloadDocumentResponse> DownloadDocumentAsync(string BlobName)
        {
            var blobClient = _ContainerClient.GetBlobClient(BlobName);
            var uri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1));
            BlobClient blobClientWithSas = new BlobClient(uri);
            if (await blobClientWithSas.ExistsAsync())
            {
                var content = await blobClientWithSas.DownloadStreamingAsync();
                return new DownloadDocumentResponse { MetadataHash = content.Value.Details.Metadata, DownloadStream = content.Value.Content };
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public async Task<ICollection<DownloadDocumentResponse>> DownloadDocumentListAsync(ICollection<string> BlobName)
        {
           var downloadDocumentResponseList = new List<DownloadDocumentResponse>();
           var blobItemList =  _ContainerClient.GetBlobs
            (BlobTraits.None).Where(blob => BlobName.Contains(blob.Name));

            foreach (var blobItem in blobItemList)
            {
                var blobClient = _ContainerClient.GetBlobClient(blobItem.Name);
                if (await blobClient.ExistsAsync())
                {
                    var content = await blobClient.DownloadStreamingAsync();
                    var downloadDocumentResponse =  new DownloadDocumentResponse { MetadataHash = content.Value.Details.Metadata, DownloadStream = content.Value.Content };
                    downloadDocumentResponseList.Add(downloadDocumentResponse);
                }
            }
            return downloadDocumentResponseList;
        }

        private string CalculateMd5(IFormFile formFile)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = formFile.OpenReadStream())
                {
                    byte[] hashBytes = md5.ComputeHash(stream);
                    return Convert.ToBase64String(hashBytes);
                }
            }
        }
    }
}
