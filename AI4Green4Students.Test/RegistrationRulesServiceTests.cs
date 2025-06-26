using AI4Green4Students.Services;
using AI4Green4Students.Tests.Fixtures;

namespace AI4Green4Students.Tests;

using Data;
using Microsoft.Extensions.DependencyInjection;

public class RegistrationRulesServiceTests : IClassFixture<TestHostFixture>, IAsyncLifetime
{
  private readonly TestHostFixture _fixture;

  public RegistrationRulesServiceTests(TestHostFixture fixture) => _fixture = fixture;
  public async Task InitializeAsync() => await _fixture.InitializeServices();
  public async Task DisposeAsync() => await _fixture.DropTestDatabase();

  /// <summary>
  /// Valid email attempts to register, with no rule blocking or adding them. Returns true.
  /// </summary>
  [Fact]
  public async Task ValidEmail_WithNoBlocksOrAllows_ReturnsTrue()
  {
    //Arrange
    var (seeder, service) = await GetContextModel();
    await seeder.SeedDefaultRules();

    //Act
    var result = await service.ValidEmail(StringConstants.GoodGmailEmail);

    //Assert
    Assert.True(result);
  }

  /// <summary>
  /// Valid email attempts to register, with domain blocked, but specific email allowed.
  /// </summary>
  [Fact]
  public async Task ValidEmail_WithDomainBlockedButSpecificEmailAllowed_ReturnsTrue()
  {
    //Arrange
    var (seeder, service) = await GetContextModel();
    await seeder.SeedDefaultRules();

    //Act
    var result = await service.ValidEmail(StringConstants.AllowedMailEmail);

    //Assert
    Assert.True(result);
  }

  /// <summary>
  /// Email attempts to register, but domain is blocked.
  /// </summary>
  [Fact]
  public async Task ValidEmail_DomainBlocked_ReturnsFalse()
  {
    //Arrange
    var (seeder, service) = await GetContextModel();
    await seeder.SeedDefaultRules();

    //Act
    var result = await service.ValidEmail(StringConstants.HelloMailEmail);

    //Assert
    Assert.False(result);
  }

  /// <summary>
  /// Email attempts to register, domain is allowed, but specific email is blocked
  /// </summary>
  [Fact]
  public async Task ValidEmail_DomainAllowedButSpecificEmailBlocked_ReturnsFalse()
  {
    //Arrange
    var (seeder, service) = await GetContextModel();
    await seeder.SeedDefaultRules();

    //Act
    var result = await service.ValidEmail(StringConstants.BlockedGmailEmail);

    //Assert
    Assert.False(result);
  }

  /// <summary>
  /// Valid email attempts to register, but part of email domain is blocked, but the specific domain is allowed - e.g. mail.com blocked, gmail.com allowed
  /// </summary>
  [Fact]
  public async Task ValidEmail_DomainAllowed_PartDomainNameBlocked_ReturnsTrue()
  {
    //Arrange
    var (seeder, service) = await GetContextModel();
    await seeder.SeedDefaultRules();

    //Act
    var result = await service.ValidEmail(StringConstants.SomeoneGmailEmail);

    //Assert
    Assert.True(result);
  }

  /// <summary>
  /// Email attempts to register, but global block prevents them.
  /// </summary>
  [Fact]
  public async Task ValidEmail_GlobalBlock_ReturnsFalse()
  {
    //Arrange
    var (seeder, service) = await GetContextModel();
    await seeder.SeedGlobalBlock();

    //Act
    var result = await service.ValidEmail(StringConstants.ExampleMailEmail);

    //Assert
    Assert.False(result);
  }

  /// <summary>
  /// Valid Email attempts to register, global block is in place, but domain is allowed.
  /// </summary>
  [Fact]
  public async Task ValidEmail_GlobalBlockButSpecificDomainAllowed_ReturnsTrue()
  {
    //Arrange
    var (seeder, service) = await GetContextModel();
    await seeder.SeedGlobalBlock();

    //Act
    var result = await service.ValidEmail(StringConstants.ValidGmailEmail);

    //Assert
    Assert.True(result);
  }

  private Task<ContextModel> GetContextModel()
  {
    var db = _fixture.GetServiceProvider().GetRequiredService<ApplicationDbContext>();
    var registrationRuleService = _fixture.GetServiceProvider().GetRequiredService<RegistrationRuleService>();

    var dataSeeder = new DataSeeder(db);

    return Task.FromResult(new ContextModel(dataSeeder, registrationRuleService));
  }

  private record ContextModel(
    DataSeeder Seeder,
    RegistrationRuleService RegistrationRuleService
  );
}
