using AI4Green4Students.Services;

namespace AI4Green4Students.Tests;

public class RegistrationRulesServiceTests : IClassFixture<DatabaseFixture>
{
  private readonly DatabaseFixture _databaseFixture;

  public RegistrationRulesServiceTests(DatabaseFixture databaseFixture)
  {
    _databaseFixture = databaseFixture;
  }
  /// <summary>
  /// Valid email attempts to register, with no rule blocking or adding them. Returns true.
  /// </summary>
  [Fact]
  public async void TestValidEmail_WithNoBlocksOrAllows()
  {
    //Arrange
    var registrationRuleService = new RegistrationRuleService(_databaseFixture.DbContext);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultRules();

    //Act
    var result = await registrationRuleService.ValidEmail(StringConstants.GoodGmailEmail);

    //Assert
    Assert.True(result);
  }

  /// <summary>
  /// Valid email attempts to register, with domain blocked, but specific email allowed.
  /// </summary>
  [Fact]
  public async void TestValidEmail_WithDomainBlocked()
  {
    //Arrange
    var registrationRuleService = new RegistrationRuleService(_databaseFixture.DbContext);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultRules();

    //Act
    var result = await registrationRuleService.ValidEmail(StringConstants.AllowedMailEmail);

    //Assert
    Assert.True(result);
  }

  /// <summary>
  /// Email attempts to register, but domain is blocked.
  /// </summary>
  [Fact]
  public async void TestInvalidEmail_DomainBlocked()
  {
    //Arrange
    var registrationRuleService = new RegistrationRuleService(_databaseFixture.DbContext);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultRules();

    //Act
    var result = await registrationRuleService.ValidEmail(StringConstants.HelloMailEmail);

    //Assert
    Assert.False(result);
  }

  /// <summary>
  /// Email attempts to register, domain is allowed, but specific email is blocked
  /// </summary>
  [Fact]
  public async void TestInvalidEmail_DomainAllowed()
  {
    //Arrange
    var registrationRuleService = new RegistrationRuleService(_databaseFixture.DbContext);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultRules();

    //Act
    var result = await registrationRuleService.ValidEmail(StringConstants.BlockedGmailEmail);

    //Assert
    Assert.False(result);
  }

  /// <summary>
  /// Valid email attempts to register, but part of email domain is blocked, but the specific domain is allowed - e.g. mail.com blocked, gmail.com allowed
  /// </summary>
  [Fact]
  public async void TestValidEmail_DomainAllowed_PartDomainNameBlocked()
  {
    //Arrange
    var registrationRuleService = new RegistrationRuleService(_databaseFixture.DbContext);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultRules();

    //Act
    var result = await registrationRuleService.ValidEmail(StringConstants.SomeoneGmailEmail);

    //Assert
    Assert.True(result);
  }

  /// <summary>
  /// Email attempts to register, but global block prevents them.
  /// </summary>
  [Fact]
  public async void InvalidEmail_GlobalBlock()
  {
    //Arrange
    var registrationRuleService = new RegistrationRuleService(_databaseFixture.DbContext);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedGlobalBlock();

    //Act
    var result = await registrationRuleService.ValidEmail(StringConstants.ExampleMailEmail);

    //Assert
    Assert.False(result);
  }

  /// <summary>
  /// Valid Email attempts to register, global block is in place, but domain is allowed.
  /// </summary>
  [Fact]
  public async void ValidEmail_GlobalBlock_ValidDomain()
  {
    //Arrange
    var registrationRuleService = new RegistrationRuleService(_databaseFixture.DbContext);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedGlobalBlock();

    //Act
    var result = await registrationRuleService.ValidEmail(StringConstants.ValidGmailEmail);

    //Assert
    Assert.True(result);
  }
}
