using AI4Green4Students.Config;
using AI4Green4Students.Data;
using AI4Green4Students.Services;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Moq;

namespace AI4Green4Students.Tests.Fixtures;

public class ExportServiceFixture
{
  public readonly ExportService Service;
  private readonly Mock<AzureStorageService> _mockAZExperimentStorageService;
  
  public ExportServiceFixture(ApplicationDbContext dbContext)
  {
    _mockAZExperimentStorageService = new Mock<AzureStorageService>(new Mock<BlobServiceClient>().Object, Options.Create(new AZOptions()));
    var sectionService = new SectionService(dbContext);
    var fieldService = new FieldService(dbContext);
    var fieldResponseServiceFixture = new FieldResponseServiceFixture(dbContext);
    Service = new ExportService(sectionService, fieldService, fieldResponseServiceFixture.Service, _mockAZExperimentStorageService.Object);
  }
}
