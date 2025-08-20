using Azure.Storage.Blobs;
using AzureHomework2t1.Services.Abstract;

namespace AzureHomework2t1.Services.Implementation
{   
    public class BlobService : IBlobService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobService(BlobServiceClient blobServiceClient, IConfiguration configuration)
        {
            //string containerName = configuration["AzureBlobStorage:ImageContainerName"]!;
            string containerName = configuration["AzureBlobStorage:ImageContainerName"]!;
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            _containerClient.CreateIfNotExists();
            _containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
        }

        public async Task<string> UploadBlobAsync(string fileName, Stream data)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(data, overwrite: true);
            return blobClient.Uri.ToString();
        }

        public async Task<Stream> DownloadBlobAsync(string blobName)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            var downloadInfo = await blobClient.DownloadAsync();
            return downloadInfo.Value.Content;
        }
    }
}
