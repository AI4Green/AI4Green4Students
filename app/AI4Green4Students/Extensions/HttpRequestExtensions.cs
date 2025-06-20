using System.Globalization;
using AI4Green4Students.Auth;
using Flurl;

namespace AI4Green4Students.Extensions;

public static class HttpRequestExtensions
{
  public static CultureInfo GetUICulture(this HttpRequest request)
  {
    CultureInfo culture = CultureInfo.CurrentUICulture;

    try
    {
      var requestCultureName =
        // Try the User first
        request.HttpContext.User.FindFirst(CustomClaimTypes.UICulture)?.Value
        // Else use the Header from the frontend
        ?? request.Headers.AcceptLanguage.FirstOrDefault();

      if (requestCultureName is not null)
        culture = CultureInfo.GetCultureInfoByIetfLanguageTag(requestCultureName);
    }
    catch (CultureNotFoundException)
    {
      // No worries, we'll just continue with CurrentUICulture
    }

    return culture;
  }


  // These don't strictly extend HttpRequest but do require it to work :)

  public static Uri ToLocalUrl(this string path, RequestContextModel model)
      => Url.Parse(Url.Combine(
            $"{model.Scheme}://{model.Host}",
            path))
        .SetQueryParam("lng", model.UiCulture)
        .ToUri();

  public static string ToLocalUrlString(this string path, RequestContextModel model)
      => path.ToLocalUrl(model).ToString();
}

public record RequestContextModel(string Scheme, string Host, string UiCulture)
{
  public RequestContextModel(HttpRequest request)
    : this(request.Scheme, request.Host.Value, request.GetUICulture().Name)
  {
  }
}

