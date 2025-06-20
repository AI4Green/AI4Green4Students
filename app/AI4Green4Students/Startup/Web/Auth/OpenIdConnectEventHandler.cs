namespace AI4Green4Students.Startup.Web.Auth;

using System.Security.Claims;
using AI4Green4Students.Auth;
using Data.Entities.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Models.User;
using Services;

public static class OpenIdConnectEventHandlers
{
  /// <summary>
  /// Handle post token validation event. Create or update the user. Set the user profile cookie.
  /// </summary>
  /// <param name="context">Context.</param>
  public static async Task HandleTokenValidated(TokenValidatedContext context)
  {
    var serviceProvider = context.HttpContext.RequestServices;
    var signInManager = serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var userProfile = serviceProvider.GetRequiredService<UserProfileService>();

    var email =
      context.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value
      ?? context.Principal?.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
    var fullName =
      context.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value
      ?? context.Principal?.Claims.FirstOrDefault(x => x.Type == "name")?.Value;

    if (email is null)
    {
      context.Fail("Email claim missing.");
      return;
    }

    var user = await userManager.FindByEmailAsync(email);
    if (user is null)
    {
      user = new ApplicationUser
      {
        FullName = fullName ?? string.Empty,
        UserName = email,
        Email = email,
        EmailConfirmed = true,
      };

      var result = await userManager.CreateAsync(user);
      if (!result.Succeeded)
      {
        context.Fail("User creation failed.");
        return;
      }
      await userManager.AddToRoleAsync(user, Roles.Student);
    }
    else
    {
      user.FullName = fullName ?? user.FullName;
      user.EmailConfirmed = true;
      await userManager.UpdateAsync(user);
    }

    // mainly to revoke user session in oidc provider (Keycloak), see logout flow
    var authProperties = new AuthenticationProperties();
    if (context.TokenEndpointResponse is not null)
    {
      authProperties.StoreTokens(
        [
          new AuthenticationToken
          {
            Name = OpenIdConnectParameterNames.RefreshToken,
            Value = context.TokenEndpointResponse.RefreshToken,
          }
        ]
      );
    }

    await signInManager.SignInAsync(user, authProperties);
    var profile = await userProfile.BuildProfile(user);
    context.HttpContext.Response.Cookies.Append(
      AuthConfiguration.ProfileCookieName,
      System.Text.Json.JsonSerializer.Serialize((BaseUserProfileModel)profile),
      AuthConfiguration.ProfileCookieOptions
    );

    context.Success();
  }
}
