using AI4Green4Students.Config;
using AI4Green4Students.Data;
using AI4Green4Students.Services;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Moq;

namespace AI4Green4Students.Tests.Fixtures;

public class FieldResponseServiceFixture
{
  public readonly FieldResponseService Service;
  private readonly Mock<AzureStorageService> _mockAZExperimentStorageService;
  
  public FieldResponseServiceFixture(ApplicationDbContext dbContext)
  {
    _mockAZExperimentStorageService = new Mock<AzureStorageService>(new Mock<BlobServiceClient>().Object, Options.Create(new AzureStorageOptions()));
    var fieldService = new FieldService(dbContext);
    Service = new FieldResponseService(dbContext, fieldService, _mockAZExperimentStorageService.Object);
  }
}
