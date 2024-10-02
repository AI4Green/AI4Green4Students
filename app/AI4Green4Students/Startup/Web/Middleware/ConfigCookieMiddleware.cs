using System.Text.Json;
using AI4Green4Students.Models;
using AI4Green4Students.Services;

namespace AI4Green4Students.Startup.Web.Middleware;

public class ConfigCookieMiddleware
{

  public static readonly string ConfigCookieName = ".AI4Green4Students.Config";

  private readonly RequestDelegate _next;

  public ConfigCookieMiddleware(
    RequestDelegate next
    )
  {
    _next = next;
  }

  public async Task Invoke(HttpContext context, FeatureFlagService features)
  { 
    var model = new ConfigCookieModel();
    model.Flags = await features.List();
    var jsonString = JsonSerializer.Serialize(model);
    context.Response.Cookies.Append(ConfigCookieName, jsonString);
    await _next(context);
  }
}
public static class ConfigCookieMiddlewareExtensions
{
  public static IApplicationBuilder UseConfigCookieMiddleware(
      this IApplicationBuilder builder)
  {
    return builder.UseMiddleware<ConfigCookieMiddleware>();
  }

}
