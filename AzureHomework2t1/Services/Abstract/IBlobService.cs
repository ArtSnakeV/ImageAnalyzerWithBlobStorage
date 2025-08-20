namespace AzureHomework2t1.Services.Abstract
{
    public interface IBlobService
    {
        Task<string> UploadBlobAsync(string fileName, Stream data);
        Task<Stream> DownloadBlobAsync(string blobName);
    }
}
