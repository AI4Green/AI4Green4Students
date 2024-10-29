using AI4Green4Students.Config;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

namespace AI4Green4Students.Services;
public class AzureStorageService
{
  private readonly BlobServiceClient _blobServiceClient;
  private readonly AzureStorageOptions _azureStorage;

  public AzureStorageService(BlobServiceClient blobServiceClient, IOptions<AzureStorageOptions> azureStorage)
  {
    _blobServiceClient = blobServiceClient;
    _azureStorage = azureStorage.Value;
  }

  public async Task<string> Upload(string filePath, Stream dataStream)
  {
    var containerClient = _blobServiceClient.GetBlobContainerClient(_azureStorage.ExperimentBlobContainer);
    var blobClient = containerClient.GetBlobClient(filePath);
    await blobClient.UploadAsync(dataStream);
    return blobClient.Name;
  }
  public async Task<byte[]> Get(string filePath)
  {
    var containerClient = _blobServiceClient.GetBlobContainerClient(_azureStorage.ExperimentBlobContainer);
    var blobClient = containerClient.GetBlobClient(filePath);
    var downloadContent = await blobClient.DownloadAsync();

    using (MemoryStream stream = new MemoryStream())
    {
      await downloadContent.Value.Content.CopyToAsync(stream);
      return stream.ToArray();
    }
  }
  
  public async Task Delete(string filePath)
  {
    var containerClient = _blobServiceClient.GetBlobContainerClient(_azureStorage.ExperimentBlobContainer);
    var blobClient = containerClient.GetBlobClient(filePath);

    if (await blobClient.ExistsAsync())
    {
      await blobClient.DeleteAsync();
    }
  }
  
  public async Task<string> Replace(string filePath, string newFilePath, Stream dataStream)
  {
    await Delete(filePath);
    var result = await Upload(newFilePath, dataStream);
    return result;
  }
}
