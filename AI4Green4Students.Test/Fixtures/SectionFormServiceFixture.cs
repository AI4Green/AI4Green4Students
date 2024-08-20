using AI4Green4Students.Data;
using AI4Green4Students.Services;

namespace AI4Green4Students.Tests.Fixtures;
public class SectionFormServiceFixture
{
  public readonly SectionFormService Service;
  
  public SectionFormServiceFixture(ApplicationDbContext dbContext)
  {
    var sectionService = new SectionService(dbContext);
    var fieldService = new FieldService(dbContext);
    Service = new SectionFormService(sectionService, fieldService);
  }
}
