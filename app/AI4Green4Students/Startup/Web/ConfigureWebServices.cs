namespace AI4Green4Students.Startup.Web;

using AI4Green4Students.Auth;
using Auth;
using Config;
using ConfigureServicesExtensions;
using Constants;
using Data;
using Data.Config;
using Data.Entities.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Services;

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

    b.Services.AddAuthentication(o =>
      {
        o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
      })
      .AddCookie()
      .AddOpenIdConnect(o =>
      {
        var oidcOptions = b.Configuration.GetSection("OpenIDConnect");

        o.Authority = oidcOptions["Authority"];
        o.ClientId = oidcOptions["ClientId"];
        o.ClientSecret = oidcOptions["ClientSecret"];

        o.RequireHttpsMetadata = false;
        o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        o.ResponseType = OpenIdConnectResponseType.Code;
        o.SaveTokens = true;

        o.Events = new OpenIdConnectEvents
        {
          OnTokenValidated = async context => await OpenIdConnectEventHandlers.HandleTokenValidated(context),
          OnRedirectToIdentityProvider = context =>
          {
            if (context.Properties.Items.TryGetValue("kc_idp_hint", out var idp))
            {
              context.ProtocolMessage.Parameters["kc_idp_hint"] = idp;
            }
            return Task.CompletedTask;
          }
        };
      });

    // App Options Configuration
    b.Services.AddOptions().Configure<RegistrationOptions>(b.Configuration.GetSection("Registration"));
    b.Services.AddOptions().Configure<UserAccountOptions>(b.Configuration.GetSection("UserAccounts"));
    b.Services.AddOptions().Configure<AzureStorageOptions>(b.Configuration.GetSection("AzureStorage"));
    b.Services.AddOptions().Configure<WorkerOptions>(b.Configuration.GetSection("Worker"));
    b.Services.AddOptions().Configure<OidcOptions>(b.Configuration.GetSection("OpenIDConnect"));

    // MVC
    b.Services.Configure<ApiBehaviorOptions>(o=> o.SuppressModelStateInvalidFilter = true);
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

    return b;
  }
}
