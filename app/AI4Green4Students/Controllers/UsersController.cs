namespace AI4Green4Students.Controllers;

using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using Auth;
using Data.Entities.Identity;
using Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.User;
using Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
  private readonly SignInManager<ApplicationUser> _signIn;
  private readonly UserManager<ApplicationUser> _user;
  private readonly UserService _users;
  private readonly UserProfileService _userProfile;

  public UsersController(
    UserService users,
    SignInManager<ApplicationUser> signIn,
    UserManager<ApplicationUser> user,
    UserProfileService userProfile
  )
  {
    _users = users;
    _signIn = signIn;
    _user = user;
    _userProfile = userProfile;
  }

  /// <summary>
  /// List users.
  /// </summary>
  /// <returns>List.</returns>
  [Authorize(nameof(AuthPolicies.CanViewAllUsers))]
  [HttpGet]
  public async Task<IActionResult> List() => Ok(await _users.List());

  /// <summary>
  /// Get user.
  /// </summary>
  /// <param name="id">User id.</param>
  /// <returns>User.</returns>
  [Authorize(nameof(AuthPolicies.CanViewAllUsers))]
  [HttpGet("{id}")]
  public async Task<IActionResult> Get(string id)
  {
    try
    {
      var user = await _users.Get(id);
      return Ok(user);
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
  }

  /// <summary>
  /// Delete user.
  /// </summary>
  /// <param name="id">User id.</param>
  /// <param name="notify">Notify deletion.</param>
  [Authorize(nameof(AuthPolicies.CanDeleteUsers))]
  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(string id, [FromQuery] bool notify = false)
  {
    try
    {
      await _users.Delete(id, notify);
      return NoContent();
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
    catch (InvalidOperationException e)
    {
      return BadRequest(e.Message);
    }
  }

  /// <summary>
  /// Update user roles.
  /// </summary>
  /// <param name="id">User id.</param>
  /// <param name="roles">Roles to assign.</param>
  [Authorize(nameof(AuthPolicies.CanEditUsers))]
  [HttpPut("{id}/roles/update")]
  public async Task<IActionResult> UpdateRoles(string id, [FromBody] List<string> roles)
  {
    try
    {
      await _users.UpdateRoles(id, roles);
      return NoContent();
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
    catch (InvalidOperationException e)
    {
      return BadRequest(e.Message);
    }
  }

  /// <summary>
  /// Start an email change process. Only the user themselves or a user with the appropriate permissions can initiate this.
  /// </summary>
  /// <param name="id">User id.</param>
  /// <param name="newEmail">New email.</param>
  [HttpPut("{id}/email/request-change")]
  public async Task<IActionResult> RequestEmailChange(string id, [FromQuery] string newEmail)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var hasPermission = User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.EditUsers);

    if (userId != id && !hasPermission)
    {
      return Forbid();
    }

    try
    {
      await _users.RequestEmailChange(id, newEmail);
      return NoContent();
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
    catch (InvalidOperationException e)
    {
      return BadRequest(e.Message);
    }
  }

  /// <summary>
  /// Set the user's UI culture.
  /// </summary>
  /// <param name="culture">Culture</param>
  [HttpPut("ui-culture")]
  public async Task<IActionResult> SetUICulture([FromBody] string culture)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
    {
      return Unauthorized("User not authenticated.");
    }

    try
    {
      await _users.SetUICulture(userId, culture);
      var user = await _user.FindByIdAsync(userId);

      if (user is null)
      {
        return NotFound("User not found.");
      }

      await _signIn.SignInAsync(user, false);
      var profile = await _userProfile.BuildProfile(user);

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
  /// Invite a user.
  /// </summary>
  /// <param name="model">Invite model.</param>
  /// <returns>Invite result.</returns>
  [Authorize(nameof(AuthPolicies.CanInviteUsers))]
  [Authorize(nameof(AuthPolicies.CanInviteInstructors))]
  [Authorize(nameof(AuthPolicies.CanInviteStudents))]
  [HttpPost("invite")]
  public async Task<IActionResult> Invite([FromBody] UserInviteModel model)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState.CollapseErrors());
    }

    try
    {
      var result = await _users.Invite(model, Request.GetUICulture().Name);
      if (result.Errors.Count != 0)
      {
        return BadRequest(result);
      }
      return NoContent();
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
  }

  /// <summary>
  /// Resend an invitation or activation email to a user.
  /// </summary>
  /// <param name="userIdOrEmail">User id or email.</param>
  [Authorize(nameof(AuthPolicies.CanInviteUsers))]
  [Authorize(nameof(AuthPolicies.CanInviteInstructors))]
  [Authorize(nameof(AuthPolicies.CanInviteStudents))]
  [HttpPut("invite/resend")]
  public async Task<IActionResult> InviteResend([FromBody] string userIdOrEmail)
  {
    try
    {
      await _users.InviteResend(userIdOrEmail, new RequestContextModel(Request));
      return NoContent();
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
  }

  /// <summary>
  /// Get the current user's profile.
  /// </summary>
  /// <returns>User profile.</returns>
  [HttpGet("me")]
  public async Task<IActionResult> Me()
  {
    var profile = await _userProfile.BuildProfile(User);
    return Ok(profile);
  }
}
