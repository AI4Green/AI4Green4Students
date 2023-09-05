using AI4Green4Students.Config;
using AI4Green4Students.Services;
using AI4Green4Students.Services.Contracts;
using AI4Green4Students.Services.EmailSender;
using AI4Green4Students.Services.EmailServices;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AI4Green4Students.Extensions
{
  public static class ServiceCollectionExtensions
  {

    public static IServiceCollection AddEmailSender(this IServiceCollection s, IConfiguration c)
    {

      var emailProvider = c["OutboundEmail:Provider"] ?? string.Empty;

      var useSendGrid = emailProvider.Equals("sendgrid", StringComparison.InvariantCultureIgnoreCase);

      if (useSendGrid) s.Configure<SendGridOptions>(c.GetSection("OutboundEmail"));
      else s.Configure<LocalDiskEmailOptions>(c.GetSection("OutboundEmail"));

      s
              .AddTransient<TokenIssuingService>()
              .AddTransient<RazorViewService>()
              .AddTransient<AccountEmailService>()
              .AddTransient<ProjectGroupEmailService>()
              .TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

      if (useSendGrid) s.AddTransient<IEmailSender, SendGridEmailSender>();
      else s.AddTransient<IEmailSender, LocalDiskEmailSender>();

      return s;
    }
  }
}
