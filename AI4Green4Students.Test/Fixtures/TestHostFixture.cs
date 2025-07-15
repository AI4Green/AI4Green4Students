namespace AI4Green4Students.Tests.Fixtures;

using Azure.Storage.Blobs;
using Config;
using Data;
using Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Npgsql;
using Services;
using Services.Contracts;
using Services.EmailServices;
using Testcontainers.PostgreSql;

/// <summary>
/// Spins up a postgres container for testing.
/// </summary>
public class TestHostFixture : IAsyncLifetime
{
  private readonly PostgreSqlContainer _pgContainer = new PostgreSqlBuilder()
    .WithDatabase("test_db")
    .WithUsername("test")
    .WithPassword("test")
    .WithImage("postgres:latest")
    .WithCleanUp(true)
    .Build();

  private IServiceProvider _serviceProvider = null!;

  private string _testDb = null!;

  public async Task InitializeAsync() => await _pgContainer.StartAsync();

  public async Task DisposeAsync() => await _pgContainer.DisposeAsync();

  public async Task InitializeServices()
  {
    var services = new ServiceCollection();

    var connectionString = await CreateTestDatabase();
    services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

    AddTestMocks(services);

    services
      .AddTransient<FeatureFlagService>()
      .AddTransient<AccountService>()
      .AddTransient<UserService>()
      .AddTransient<UserProfileService>()
      .AddTransient<RegistrationRuleService>()
      .AddTransient<ProjectService>()
      .AddTransient<SectionTypeService>()
      .AddTransient<InputTypeService>()
      .AddTransient<SectionService>()
      .AddTransient<FieldService>()
      .AddTransient<ProjectGroupService>()
      .AddTransient<LiteratureReviewService>()
      .AddTransient<PlanService>()
      .AddTransient<NoteService>()
      .AddTransient<ReportService>()
      .AddTransient<SectionFormService>()
      .AddTransient<FieldResponseService>()
      .AddTransient<CommentService>()
      .AddTransient<StageService>()
      .AddTransient<ReactionTableService>()
      .AddTransient<ExportService>();

    _serviceProvider = services.BuildServiceProvider();

    // migrations
    var db = _serviceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
  }

  /// <summary>
  /// Drops the test db.
  /// Disposes any services and clears the connection pool.
  /// </summary>
  public async Task DropTestDatabase()
  {
    var serviceProvider = GetServiceProvider();
    switch (serviceProvider)
    {
      case IAsyncDisposable asyncDisposable:
        await asyncDisposable.DisposeAsync();
        break;
      case IDisposable disposable:
        disposable.Dispose();
        break;
    }

    NpgsqlConnection.ClearAllPools();
    await using var connection = new NpgsqlConnection(_pgContainer.GetConnectionString());
    await connection.OpenAsync();
    await using var command = new NpgsqlCommand($"DROP DATABASE \"{_testDb}\"", connection);
    await command.ExecuteNonQueryAsync();
  }

  public IServiceProvider GetServiceProvider() => _serviceProvider;

  /// <summary>
  /// Creates a test database, not same as spinning up a new container.
  /// </summary>
  /// <returns>Connection string to the test database.</returns>
  private async Task<string> CreateTestDatabase()
  {
    _testDb = $"test_test_db_{Guid.NewGuid():N}";

    await using (var connection = new NpgsqlConnection(_pgContainer.GetConnectionString()))
    {
      await connection.OpenAsync();
      await using var command = new NpgsqlCommand($"CREATE DATABASE \"{_testDb}\"", connection);
      await command.ExecuteNonQueryAsync();
    }

    return new NpgsqlConnectionStringBuilder(_pgContainer.GetConnectionString()) { Database = _testDb }.ToString();
  }

  /// <summary>
  /// Adds mock services to the service collection.
  /// </summary>
  /// <param name="services">Service collection to add to.</param>
  private static void AddTestMocks(IServiceCollection services)
  {
    var userManager = new Mock<UserManager<ApplicationUser>>(
      Mock.Of<IUserStore<ApplicationUser>>(),
      null,
      null,
      null,
      null,
      null,
      null,
      null,
      null
    );

    var stageEmailService = new Mock<StageEmailService>(Mock.Of<IEmailSender>());
    var azStorageService = new Mock<AzureStorageService>(
      Mock.Of<BlobServiceClient>(),
      Options.Create(new AzureStorageOptions())
    );

    services
      .AddScoped<UserManager<ApplicationUser>>(_ => userManager.Object)
      .AddScoped<StageEmailService>(_ => stageEmailService.Object)
      .AddScoped<AzureStorageService>(_ => azStorageService.Object);
  }
}
