using AI4Green4Students.Startup.Web.Extensions;
using AI4Green4Students.Startup.Web.Middleware;
using ClacksMiddleware.Extensions;

namespace AI4Green4Students.Startup.Web;

public static class ConfigureWebPipeline
{
  /// <summary>
  /// Configure the HTTP Request Pipeline for an ASP.NET Core app
  /// </summary>
  /// <param name="app"></param>
  /// <returns></returns>
  public static WebApplication UseWebPipeline(this WebApplication app)
  {
    // Pre-auth
    app.GnuTerryPratchett();
    app.UseSecureHttpsHandling();
    app.UseStaticFiles();
    app.UseConfigCookieMiddleware();
    app.UseSwagger();
    app.UseSwaggerUI();

    // Auth
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    // Endpoint mapping
    app.MapSwagger();

    app.MapControllers().BlockHttpRequests();

    app.MapUonVersionInformation().AllowAnonymous();

    app.MapFallbackToFile("index.html").AllowAnonymous();

    return app;
  }
}
