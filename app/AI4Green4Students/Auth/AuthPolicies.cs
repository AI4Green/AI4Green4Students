using Microsoft.AspNetCore.Authorization;

using System.Text.RegularExpressions;

namespace AI4Green4Students.Auth;

public static class AuthPolicies
{
  public static AuthorizationPolicy IsClientApp
    => new AuthorizationPolicyBuilder()
        .RequireAssertion(IsSameHost)
        .Build();

  public static AuthorizationPolicy IsAuthenticatedUser
    => new AuthorizationPolicyBuilder()
        .Combine(IsClientApp)
        .RequireAuthenticatedUser()
        .Build();

  public static AuthorizationPolicy CanManageUsers
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ManageUsers)
      .Build();
  
  public static AuthorizationPolicy CanCreateProjects
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.CreateProjects)
      .Build();
  
  public static AuthorizationPolicy CanEditProjects
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.EditProjects)
      .Build();
  
  public static AuthorizationPolicy CanDeleteProjects
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.DeleteProjects)
      .Build();
  
  public static AuthorizationPolicy CanViewProjects
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, 
        SitePermissionClaims.ViewProjects,
        SitePermissionClaims.ViewAllProjects,
        SitePermissionClaims.ViewEligibleProjects
      )
      .Build();

  private static readonly Func<AuthorizationHandlerContext, bool> IsSameHost =
    context =>
    {
      var request = ((DefaultHttpContext?)context.Resource)?.Request;

      // We don't bother checking for same host in a dev environment
      // to facilitate easier testing ;)
      var env = request?.HttpContext.RequestServices
        .GetRequiredService<IHostEnvironment>()
        ?? throw new InvalidOperationException("No Http Request");
      if (env.IsDevelopment()) return true;

      var referer = request?.Headers.Referer.FirstOrDefault();
      if (referer is null) return false;

      // NOTE: this trims the port from the origin
      // which is slightly more lax (same protocol and host, rather than same origin)
      // the following regex is the complete origin: /^http(s?)://[^/\s]*/
      // both regexes also only work safely for a referer header:
      // URLs in other contexts might be formatted differently than the referer header specifies.
      var referringHost = Regex.Match(referer, @"^http(s?)://[^/:\s]*").Value;

      var requestHost = $"{request!.Scheme}://{request!.Host.Host}";

      return requestHost == referringHost;
    };
}
