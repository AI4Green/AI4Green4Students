using AI4Green4Students.Config;
using AI4Green4Students.Data;
using AI4Green4Students.Services;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Moq;

namespace AI4Green4Students.Tests;
public class SectionFormServiceFixture
{
  public readonly SectionFormService Service;
  private readonly Mock<AZExperimentStorageService> _mockAZExperimentStorageService;
  
  public SectionFormServiceFixture(ApplicationDbContext dbContext)
  {
    _mockAZExperimentStorageService = new Mock<AZExperimentStorageService>(new Mock<BlobServiceClient>().Object, Options.Create(new AZOptions()));
    var sectionService = new SectionService(dbContext);
    var fieldService = new FieldService(dbContext);
    Service = new SectionFormService(dbContext, sectionService, fieldService, _mockAZExperimentStorageService.Object);
  }
}
