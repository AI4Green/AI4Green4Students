namespace AI4Green4Students.Auth;

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

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

  public static AuthorizationPolicy CanInviteInstructors
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.InviteInstructors)
      .Build();

  public static AuthorizationPolicy CanInviteStudents
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.InviteStudents)
      .Build();

  public static AuthorizationPolicy CanInviteUsers
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.InviteUsers)
      .Build();

  public static AuthorizationPolicy CanEditUsers
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.EditUsers)
      .Build();

  public static AuthorizationPolicy CanDeleteUsers
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.DeleteUsers)
      .Build();

  public static AuthorizationPolicy CanViewAllUsers
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllUsers)
      .Build();

  public static AuthorizationPolicy CanViewRoles
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewRoles)
      .Build();

  public static AuthorizationPolicy CanCreateRegistrationRules
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.CreateRegistrationRules)
      .Build();

  public static AuthorizationPolicy CanEditRegistrationRules
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.EditRegistrationRules)
      .Build();

  public static AuthorizationPolicy CanDeleteRegistrationRules
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.DeleteRegistrationRules)
      .Build();

  public static AuthorizationPolicy CanViewRegistrationRules
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewRegistrationRules)
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
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewProjects)
      .Build();

  public static AuthorizationPolicy CanCreateExperiments
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.CreateExperiments)
      .Build();

  public static AuthorizationPolicy CanDeleteExperiments
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.DeleteExperiments)
      .Build();

  public static AuthorizationPolicy CanViewExperiments
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewExperiments)
      .Build();

  public static AuthorizationPolicy CanEditComments
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.EditComments)
      .Build();

  public static AuthorizationPolicy CanComments
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.AddComments)
      .Build();

  public static AuthorizationPolicy CanMarkCommentsAsRead
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.MarkCommentsAsRead)
      .Build();

  public static AuthorizationPolicy CanDeleteComments
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.DeleteComments)
      .Build();

  public static AuthorizationPolicy CanApproveFieldResponses
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ApproveFieldResponses)
      .Build();

  public static AuthorizationPolicy CanLockProjectGroupNotes
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.LockProjectGroupNotes)
      .Build();

  public static AuthorizationPolicy CanCreateProjectTypes
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.CreateProjectTypes)
      .Build();

  public static AuthorizationPolicy CanEditProjectTypes
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.EditProjectTypes)
      .Build();

  public static AuthorizationPolicy CanDeleteProjectTypes
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.DeleteProjectTypes)
      .Build();

  public static AuthorizationPolicy CanViewProjectTypes
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewProjectTypes)
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
      if (env.IsDevelopment())
      {
        return true;
      }

      var referer = request?.Headers.Referer.FirstOrDefault();
      if (referer is null)
      {
        return false;
      }

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
