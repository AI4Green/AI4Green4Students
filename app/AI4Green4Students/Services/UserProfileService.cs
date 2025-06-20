namespace AI4Green4Students.Services;

using System.Globalization;
using System.Security.Claims;
using Auth;
using Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Models.User;

public class UserProfileService
{
  private readonly IUserClaimsPrincipalFactory<ApplicationUser> _principalFactory;

  public UserProfileService(IUserClaimsPrincipalFactory<ApplicationUser> principalFactory)
    => _principalFactory = principalFactory;

  /// <summary>
  /// Build up a client profile for a user.
  /// </summary>
  /// <param name="user"></param>
  /// <returns>User profile.</returns>
  public async Task<UserProfileModel> BuildProfile(ApplicationUser user) =>
    await BuildProfile(await _principalFactory.CreateAsync(user));

  public Task<UserProfileModel> BuildProfile(ClaimsPrincipal user)
  {
    // do a single-pass map of claims to a dictionary of those we care about
    var profileClaimTypes = new[]
    {
      ClaimTypes.Email, CustomClaimTypes.FullName, CustomClaimTypes.UICulture
    };
    var profileClaims = user.Claims.Aggregate(
      new Dictionary<string, string>(),
      (claims, claim) =>
      {
        if (profileClaimTypes.Contains(claim.Type))
        {
          // we only add the first claim of a type
          if (!claims.ContainsKey(claim.Type))
          {
            claims[claim.Type] = claim.Value;
          }
        }
        return claims;
      }
    );

    // construct a User Profile
    var profile = new UserProfileModel(
      user.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(x => x.Value).Single(),
      profileClaims[ClaimTypes.Email],
      profileClaims[CustomClaimTypes.FullName],
      profileClaims.GetValueOrDefault(CustomClaimTypes.UICulture) ?? CultureInfo.CurrentCulture.Name,
      user.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList(),
      user.Claims.Where(x => x.Type == CustomClaimTypes.SitePermission).Select(x => x.Value).ToList()
    );

    return Task.FromResult(profile);
  }
}
