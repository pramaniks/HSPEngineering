using Azure;
using Azure.Storage.Blobs.Models;
using Core.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BlobStorage
{
    public class DocumentUploadContentInfo 
    {
        public string FileName { get; set; }
        public string BlobName { get; set; }
        public ETag ETag { get; internal set; }
        public byte[] ContentHash { get; set; }
        public string ErrorMessage { get; internal set; }
    }

    public interface IBlobStorageProvider
    {
        Task<List<DocumentUploadContentInfo>> UploadDocumentListAsync(List<IFormFile> documentList);
        Task<DownloadDocumentResponse> DownloadDocumentAsync(string BlobName);
        Task<ICollection<DownloadDocumentResponse>> DownloadDocumentListAsync(ICollection<string> BlobNameList);
    }

    public interface IAccountingRecordBlobStorageProvider : IBlobStorageProvider { }
    public interface IFillingBlobStorageProvider : IBlobStorageProvider { }


    public class AccountingRecordBlobStorageProvider : BlobStorageProvider, IAccountingRecordBlobStorageProvider
    {
        public AccountingRecordBlobStorageProvider() : base(
            ConfigurationUtility.ConfigurationManager["AzureStorageConnection:PrimaryConnectionEndpoint"],
            ConfigurationUtility.ConfigurationManager["AzureStorageConnection:SecondaryConnectionEndpoint"],
            ConfigurationUtility.ConfigurationManager["AzureStorageConnection:AccountingRecordsDocumentContainerName"])
        {

        }
    }
    public class FillingBlobStorageProvider : BlobStorageProvider, IFillingBlobStorageProvider
    {
        public FillingBlobStorageProvider() : base(
            ConfigurationUtility.ConfigurationManager["AzureStorageConnection:PrimaryConnectionEndpoint"],
            ConfigurationUtility.ConfigurationManager["AzureStorageConnection:SecondaryConnectionEndpoint"],
            ConfigurationUtility.ConfigurationManager["AzureStorageConnection:FillingDocumentContainerName"])
        {

        }
    }
    public class BlobStorageProvider : IBlobStorageProvider
    {
        private readonly AzureBlobProvider _AzureBlobProvider;
        public BlobStorageProvider(string connectionEndpoint, string geosecondaryConnectionEndpoint, string containerName)
        {
            _AzureBlobProvider = new AzureBlobProvider(connectionEndpoint, geosecondaryConnectionEndpoint, containerName);
        }

        public async Task<List<DocumentUploadContentInfo>> UploadDocumentListAsync(List<IFormFile> documentList)
        {
            var response = await _AzureBlobProvider.UploadDocuments(documentList);
            return response;
        }

        public async Task<DownloadDocumentResponse> DownloadDocumentAsync(string BlobName)
        {
            var response = await _AzureBlobProvider.DownloadDocumentAsync(BlobName);
            return response;
        }

        public async Task<ICollection<DownloadDocumentResponse>> DownloadDocumentListAsync(ICollection<string> BlobNameList)
        {
            var response = await _AzureBlobProvider.DownloadDocumentListAsync(BlobNameList);
            return response;
        }
    }
}
