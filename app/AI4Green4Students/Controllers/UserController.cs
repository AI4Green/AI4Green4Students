using System.Globalization;
using System.Text.Json;
using AI4Green4Students.Auth;
using AI4Green4Students.Config;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.Account.Email;
using AI4Green4Students.Models.Emails;
using AI4Green4Students.Models.User;
using AI4Green4Students.Services;
using AI4Green4Students.Services.EmailServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
  private readonly UserManager<ApplicationUser> _users;
  private readonly SignInManager<ApplicationUser> _signIn;
  private readonly RoleManager<IdentityRole> _roles;
  private readonly UserAccountOptions _userAccountOptions;
  private readonly TokenIssuingService _tokens;
  private readonly UserService _user;
  
  public UserController(
    UserManager<ApplicationUser> users,
    SignInManager<ApplicationUser> signIn,
    RoleManager<IdentityRole> roles,
    IOptions<UserAccountOptions> userAccountOptions,
    TokenIssuingService tokens,
    UserService user)
  {
    _users = users;
    _signIn = signIn;
    _roles= roles;
    _userAccountOptions = userAccountOptions.Value;
    _tokens = tokens;
    _user = user;
  }

  [HttpGet("me")]
  public async Task<IActionResult> Me()
  {
    var profile = await _user.BuildProfile(User);
    return Ok(profile);
  }

  [HttpPut("uiCulture")]
  public async Task<IActionResult> SetUICulture([FromBody] string culture)
  {
    try
    {
      var user = await _users.FindByNameAsync(User.Identity?.Name);

      // Save it
      await _user.SetUICulture(user.Id, culture);

      // Sign In again to reset user cookie
      await _signIn.SignInAsync(user, false);

      var profile = await _user.BuildProfile(user);

      // Write a basic Profile Cookie for JS
      HttpContext.Response.Cookies.Append(
        AuthConfiguration.ProfileCookieName,
        JsonSerializer.Serialize((BaseUserProfileModel)profile),
        AuthConfiguration.ProfileCookieOptions);
    }
    catch (KeyNotFoundException) { return NotFound(); }
    catch (CultureNotFoundException) { return BadRequest(); }

    return NoContent();
  }
  
  /// <summary>
  /// Update user roles
  /// </summary>
  /// <param name="userModel"></param>
  /// <param name="id"></param>
  [Authorize(nameof(AuthPolicies.CanEditUsers))]
  [HttpPut("userRoles/{id}")]
  public async Task<IActionResult> SetUserRoles (string id, [FromBody] UserModel userModel)
  {
    // Check minimum roles is selected
    if (userModel.Roles.Count < 1)
      throw new ArgumentException("Minimum of one role required");
    
    // check role is available and valid
    var rolesAvailable = await _roles.Roles.ToListAsync(); // get list of available roles
    var valid = userModel.Roles.All(x => rolesAvailable.Any(y => x == y.NormalizedName));
    if (!valid) throw new ArgumentException("Invalid roles");

    var user = await _users.FindByIdAsync(id); // Find the user
    if (user is null) return NotFound(); // return 404 if user not found
    
    var currentRoles = await _users.GetRolesAsync(user); // Get user's current roles
    await _users.RemoveFromRolesAsync(user, currentRoles); // remove the existing roles
    await _users.AddToRolesAsync(user, userModel.Roles); // assign the new roles to the user
    return NoContent();
  }
  
  /// <summary>
  /// Start email change process. Generate email change link and send it to the user
  /// </summary>
  /// <param name="userModel"></param>
  /// <param name="id"></param>
  [HttpPut("userEmail/{id}")]
  public async Task<IActionResult> ChangeEmail (string id, UserModel userModel)
  {
    if (_users.GetUserId(User) != id && // only allow user to change their own email or user with EditUsers permission
        !User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.EditUsers))
      return Forbid();
    
    var user = await _users.FindByIdAsync(id); // Find the user
    if (user is null) return NotFound(); // return 404 if user not found
    
    // check if email already exist
    var isEmailExist = await _users.FindByEmailAsync(userModel.Email);
    if (isEmailExist is not null) return BadRequest(new SetEmailResult() { IsExistingEmail = true });

    // check if email is confirmed. Only allow email change if the user has their current email confirmed
    if (!user.EmailConfirmed) return BadRequest(new SetEmailResult() { IsUnconfirmedAccount = true });
    
    // check if new email is valid i.e. check if this new email would satisfy the registration rule
    if (!await _user.CanRegister(userModel.Email)) return BadRequest(new SetEmailResult() { IsNotAllowlisted = true });
    
    if (_userAccountOptions.SendEmail) // Check the config and send email Change link email if true
      await _tokens.SendEmailChange(user, userModel.Email);

    if (_userAccountOptions.GenerateLink) // return email change link if true
      return Ok(await _tokens.GenerateEmailChangeLink(user, userModel.Email)); // Check 200 status in the client for EmailChangeLink
    
    return NoContent(); // Return 204 if no EmailChangeLink 
  }
}

