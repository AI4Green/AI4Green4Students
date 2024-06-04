using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Services;
using AI4Green4Students.Services.EmailServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace AI4Green4Students.Tests.Fixtures;
public class StageServiceFixture
{
  public readonly StageService Service;
  private readonly Mock<StageEmailService> _mockStageEmailService;
  private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
  
  public StageServiceFixture(ApplicationDbContext dbContext)
  {
    _mockStageEmailService = new Mock<StageEmailService>(new Mock<IServiceProvider>().Object);
    _mockUserManager = new Mock<UserManager<ApplicationUser>>(
      new Mock<IUserStore<ApplicationUser>>().Object,
      new Mock<IOptions<IdentityOptions>>().Object,
      new Mock<IPasswordHasher<ApplicationUser>>().Object,
      new IUserValidator<ApplicationUser>[0],
      new IPasswordValidator<ApplicationUser>[0],
      new Mock<ILookupNormalizer>().Object,
      new Mock<IdentityErrorDescriber>().Object,
      new Mock<IServiceProvider>().Object,
      new Mock<ILogger<UserManager<ApplicationUser>>>().Object
    );
    Service = new StageService(dbContext, _mockStageEmailService.Object, _mockUserManager.Object);
  }
}
