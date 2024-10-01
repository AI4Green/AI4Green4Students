using AI4Green4Students.Config;
using AI4Green4Students.Services;
using AI4Green4Students.Services.Contracts;
using AI4Green4Students.Services.EmailSender;
using AI4Green4Students.Services.EmailServices;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AI4Green4Students.Startup.ConfigureServicesExtensions
{
  public static class ServiceCollectionExtensions
  {

    public static IServiceCollection AddEmailSender(this IServiceCollection s, IConfiguration c)
    {

      var emailProvider = c["OutboundEmail:Provider"] ?? string.Empty;

      var outboundProvider = emailProvider.ToLowerInvariant();

      switch(outboundProvider)
      {
        case "sendgrid":
          s.Configure<SendGridOptions>(c.GetSection("OutboundEmail"));
          s.AddTransient<IEmailSender, SendGridEmailSender>();
          break;

        case "smtp":
          s.Configure<SmtpOptions>(c.GetSection("OutboundEmail"));
          s.AddTransient<IEmailSender, SmtpEmailSender>();
          break;

        default:
          s.Configure<LocalDiskEmailOptions>(c.GetSection("OutboundEmail"));
          s.AddTransient<IEmailSender, LocalDiskEmailSender>();
          break;
      }

      s
        .AddTransient<TokenIssuingService>()
        .AddTransient<RazorViewService>()
        .AddTransient<AccountEmailService>()
        .AddTransient<ProjectGroupEmailService>()
        .AddTransient<StageEmailService>()
        .TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

      return s;
    }
  }
}
