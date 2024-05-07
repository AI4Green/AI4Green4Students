using ClacksMiddleware.Extensions;
using AI4Green4Students.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AI4Green4Students.Data;
using AI4Green4Students.Config;
using AI4Green4Students.Services;
using AI4Green4Students.Constants;
using UoN.AspNetCore.VersionMiddleware;
using AI4Green4Students.Middleware;
using AI4Green4Students.Auth;
using AI4Green4Students.Data.Entities.Identity;
using Microsoft.Extensions.Azure;
using AI4Green4Students.Data.DefaultExperimentSeeding;

var b = WebApplication.CreateBuilder(args);

#region Configure Services

// MVC
b.Services
  .AddControllersWithViews()
  .AddJsonOptions(DefaultJsonOptions.Configure);

// EF
b.Services
  .AddDbContext<ApplicationDbContext>(o =>
  {
    // migration bundles don't like null connection strings (yet)
    // https://github.com/dotnet/efcore/issues/26869
    // so if no connection string is set we register without one for now.
    // if running migrations, `--connection` should be set on the command line
    // in real environments, connection string should be set via config
    // all other cases will error when db access is attempted.
    var connectionString = b.Configuration.GetConnectionString("Default");
    if (string.IsNullOrWhiteSpace(connectionString))
      o.UseNpgsql();
    else
      o.UseNpgsql(connectionString,
        o => o.EnableRetryOnFailure());
  });

b.Services.AddDbContext<AI4GreenDbContext>(o =>
{
  var connectionString = b.Configuration.GetConnectionString("AI4Green");
  if (string.IsNullOrWhiteSpace(connectionString)) o.UseNpgsql();
  else o.UseNpgsql(connectionString, o => o.EnableRetryOnFailure());
});


// Identity
b.Services
  .AddIdentity<ApplicationUser, IdentityRole>(
    o => o.SignIn.RequireConfirmedEmail = true)
  .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();

b.Services
  .AddApplicationInsightsTelemetry()
  .ConfigureApplicationCookie(AuthConfiguration.IdentityCookieOptions)
  .AddAuthorization(AuthConfiguration.AuthOptions)
  .Configure<RegistrationOptions>(b.Configuration.GetSection("Registration"))
  .Configure<UserAccountOptions>(b.Configuration.GetSection("UserAccounts"))
  .Configure<AZOptions>(b.Configuration.GetSection("AZOptions"))
  .AddEmailSender(b.Configuration)
  .AddTransient<UserService>()
  .AddTransient<FeatureFlagService>()
  .AddTransient<RegistrationRuleService>()
  .AddTransient<ProjectService>()
  .AddTransient<ProjectGroupService>()
  .AddTransient<LiteratureReviewService>()
  .AddTransient<PlanService>()
  .AddTransient<NoteService>()
  .AddTransient<InputTypeService>()
  .AddTransient<SectionTypeService>()
  .AddTransient<SectionService>()
  .AddTransient<SectionFormService>()
  .AddTransient<FieldService>()
  .AddTransient<ReportService>()
  .AddTransient<CommentService>()
  .AddTransient<StageService>()
  .AddTransient<ReactionTableService>();

b.Services.AddSwaggerGen();

// Azure Blob and Queue
b.Services.AddAzureClients(builder =>
{
  builder.AddBlobServiceClient(b.Configuration.GetConnectionString("AzureStorage"));
});
b.Services.AddScoped<AZExperimentStorageService>();

#endregion

var app = b.Build();

// Do data seeding isolated from the running of the app
using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider
    .GetRequiredService<ApplicationDbContext>();

  var roles = scope.ServiceProvider
    .GetRequiredService<RoleManager<IdentityRole>>();

  var registrationRule = scope.ServiceProvider
    .GetRequiredService<RegistrationRuleService>();

  var project = scope.ServiceProvider
    .GetRequiredService<ProjectService>();

  var config = scope.ServiceProvider
    .GetRequiredService<IConfiguration>();

  var users = scope.ServiceProvider
    .GetRequiredService<UserManager<ApplicationUser>>();

  var passwordHasher = scope.ServiceProvider
    .GetRequiredService<IPasswordHasher<ApplicationUser>>();

  var inputTypes = scope.ServiceProvider
    .GetRequiredService<InputTypeService>();

  var sectionTypes = scope.ServiceProvider
    .GetRequiredService<SectionTypeService>();

  var sections = scope.ServiceProvider
    .GetRequiredService<SectionService>();

  var fields = scope.ServiceProvider
    .GetRequiredService<FieldService>();

  var seeder = new DataSeeder(db, roles, registrationRule, users, passwordHasher, config, inputTypes, sectionTypes);
  await seeder.SeedRoles();
  await seeder.SeedRegistrationRules();
  await seeder.SeedAdminUser();
  await seeder.SeedInputTypes();
  await seeder.SeedSectionTypes();
  await seeder.SeedStageType();
  await seeder.SeedStagePermission();
  await seeder.SeedStage();

  //todo - move this to a CLI command for creating default experiment, complete with fields
  //We may keep this seeding option in as an example experiment for users to look at 
  var defaultExperimentSeeder = new DefaultExperimentDataSeeder(project, sections, inputTypes, fields, sectionTypes);
  await defaultExperimentSeeder.SeedDefaultExperiment();
}

#region Configure Pipeline

app.GnuTerryPratchett();

if (!app.Environment.IsDevelopment())
{
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseVersion();
app.UseConfigCookieMiddleware();
app.UseSwagger();
app.UseSwaggerUI();

#endregion

#region Endpoint Routing

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Endpoints

app.MapControllers();

app.MapFallbackToFile("index.html").AllowAnonymous();

#endregion

app.Run();
