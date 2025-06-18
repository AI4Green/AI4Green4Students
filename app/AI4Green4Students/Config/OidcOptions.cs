namespace AI4Green4Students.Config;

public class OidcOptions
{
  public string ClientId { get; set; } = string.Empty;
  public string ClientSecret { get; set; } = string.Empty;
  public string Authority { get; set; } = string.Empty;
  public string RedirectUri { get; set; } = string.Empty;
}