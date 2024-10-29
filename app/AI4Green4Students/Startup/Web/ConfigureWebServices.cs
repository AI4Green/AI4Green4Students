using AI4Green4Students.Auth;
using AI4Green4Students.Config;
using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Config;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Services;
using AI4Green4Students.Startup.ConfigureServicesExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Azure;

namespace AI4Green4Students.Startup.Web;

public static class ConfigureWebServices
{
  public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder b)
  {
    // Identity
    b.Services
      .AddIdentity<ApplicationUser, IdentityRole>(o =>
        o.SignIn.RequireConfirmedAccount = true)
      .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultTokenProviders();

    // Auth
    b.Services
      .ConfigureApplicationCookie(AuthConfiguration.IdentityCookieOptions)
      .AddAuthorization(AuthConfiguration.AuthOptions);

    // App Options Configuration
    b.Services.AddOptions().Configure<RegistrationOptions>(b.Configuration.GetSection("Registration"));
    b.Services.AddOptions().Configure<UserAccountOptions>(b.Configuration.GetSection("UserAccounts"));
    b.Services.AddOptions().Configure<AzureStorageOptions>(b.Configuration.GetSection("AzureStorage"));
    b.Services.AddOptions().Configure<WorkerOptions>(b.Configuration.GetSection("Worker"));

    // MVC
    b.Services
      .AddControllersWithViews()
      .AddJsonOptions(DefaultJsonOptions.Configure);

    // OpenAPI
    b.Services.AddSwaggerGen(c => { c.EnableAnnotations(); });

    // App insights
    b.Services
      .AddApplicationInsightsTelemetry();

    // Blob storage
    b.Services.AddAzureClients(builder =>
    {
      builder.AddBlobServiceClient(b.Configuration.GetConnectionString("AzureStorage"));
    });
    b.Services.AddScoped<AzureStorageService>();

    // EF
    b.Services.AddDataDbContext(b.Configuration);
    b.Services.AddAI4GreenDbContext(b.Configuration);

    // App specific services 
    b.Services
      .AddEmailSender(b.Configuration)

      .AddTransient<FeatureFlagService>()
      .AddTransient<UserService>()

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
    
    return b;
  }
}
