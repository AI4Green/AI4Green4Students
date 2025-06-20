namespace AI4Green4Students.Controllers;

using System.Text.Json;
using Auth;
using Config;
using Data.Entities.Identity;
using Extensions;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Models.Account.Email;
using Models.Account.Login;
using Models.Account.Password;
using Models.Account.Register;
using Models.Account.Token;
using Models.User;
using Services;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
  private readonly AccountService _account;
  private readonly OidcOptions _oidcOptions;
  private readonly SignInManager<ApplicationUser> _signIn;
  private readonly UserService _user;
  private readonly UserManager<ApplicationUser> _users;

  public AccountController(
    UserManager<ApplicationUser> users,
    SignInManager<ApplicationUser> signIn,
    UserService user,
    IOptions<OidcOptions> oidcOptions,
    AccountService account
  )
  {
    _users = users;
    _signIn = signIn;
    _user = user;
    _oidcOptions = oidcOptions.Value;
    _account = account;
  }

  /// <summary>
  /// Initiate OpenID Connect login flow.
  /// </summary>
  /// <param name="redirectUri">Redirect URI after login.</param>
  /// <param name="idp">Identity Provider hint.</param>
  [HttpGet("oidc-login")]
  public Task<IActionResult> OidcLogin(string? redirectUri, string? idp)
  {
    var properties = new AuthenticationProperties
    {
      RedirectUri = redirectUri ?? "/"
    };

    if (!string.IsNullOrEmpty(idp))
    {
      properties.Items["kc_idp_hint"] = idp;
    }

    return Task.FromResult<IActionResult>(Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme));
  }

  /// <summary>
  /// Register a new account.
  /// </summary>
  /// <param name="model">Registration model.</param>
  /// <returns>Registration result.</returns>
  [HttpPost("register")]
  public async Task<IActionResult> Register(RegisterAccountModel model)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest();
    }

    var result = await _account.Register(model, new RequestContextModel(Request));
    if (result.Errors.Count != 0)
    {
      return BadRequest(result);
    }

    return NoContent();
  }

  /// <summary>
  /// Login with username and password.
  /// </summary>
  /// <param name="model">Login model.</param>
  /// <returns>Login result.</returns>
  [HttpPost("login")]
  public async Task<IActionResult> Login(LoginModel model)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest();
    }

    try
    {
      var result = await _account.Login(model);
      if (result.User is null || result.Errors.Count != 0)
      {
        return BadRequest(result);
      }

      HttpContext.Response.Cookies.Append(
        AuthConfiguration.ProfileCookieName,
        JsonSerializer.Serialize((BaseUserProfileModel)result.User),
        AuthConfiguration.ProfileCookieOptions);

      return Ok(result);
    }
    catch (InvalidOperationException e)
    {
      return BadRequest(e.Message);
    }
  }

  /// <summary>
  /// Logout.
  /// </summary>
  [HttpPost("logout")]
  public async Task<IActionResult> Logout()
  {
    // Sign out of Identity
    await _signIn.SignOutAsync();

    // Also remove the JS Profile Cookie
    HttpContext.Response.Cookies.Delete(AuthConfiguration.ProfileCookieName);

    // if applicable, revoke user session in oidc provider (Keycloak)
    var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
    if (!string.IsNullOrEmpty(refreshToken))
    {
      await _oidcOptions.Authority
        .AppendPathSegment("protocol/openid-connect/logout")
        .PostUrlEncodedAsync(new
        {
          client_id = _oidcOptions.ClientId, client_secret = _oidcOptions.ClientSecret, refresh_token = refreshToken
        });
    }

    await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
    return NoContent();
  }

  /// <summary>
  /// Confirm user account after user registration.
  /// </summary>
  /// <param name="model">Conifrmation token model.</param>
  /// <returns>Confirmation result.</returns>
  [HttpPost("confirm")]
  public async Task<IActionResult> Confirm(UserTokenModel model)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest();
    }

    try
    {
      var result = await _account.Confirm(model);
      if (result.Errors.Count != 0 || result.User is null)
      {
        return BadRequest(result);
      }

      var user = await _users.FindByIdAsync(model.UserId);
      await _signIn.SignInAsync(user!, false);

      HttpContext.Response.Cookies.Append(
        AuthConfiguration.ProfileCookieName,
        JsonSerializer.Serialize((BaseUserProfileModel)result.User),
        AuthConfiguration.ProfileCookieOptions);

      return Ok(result.User);
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
  }

  /// <summary>
  /// Resend account confirmation notification.
  /// </summary>
  /// <param name="userIdOrEmail">User id or email.</param>
  [HttpPost("confirm/resend")]
  public async Task<IActionResult> ConfirmResend([FromBody] string userIdOrEmail)
  {
    try
    {
      await _account.ConfirmResend(userIdOrEmail, new RequestContextModel(Request));
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
  /// Request password reset for a user.
  /// </summary>
  /// <param name="userIdOrEmail">User id or email.</param>
  [HttpPost("password/request-reset")]
  public async Task<IActionResult> RequestPasswordReset([FromBody] string userIdOrEmail)
  {
    try
    {
      await _user.RequestPasswordReset(userIdOrEmail, new RequestContextModel(Request));
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
  /// Reset user's password.
  /// </summary>
  /// <param name="model">Reset password token.</param>
  /// <returns>Reset password result.</returns>
  [HttpPost("password/reset")]
  public async Task<IActionResult> ResetPassword(AnonymousSetPasswordModel model)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest();
    }

    try
    {
      var result = await _user.ResetPassword(model);
      if (result.Errors.Count != 0)
      {
        return BadRequest(result);
      }

      if (result.User is null && result.IsUnconfirmedAccount is true)
      {
        return Ok(result);
      }

      var user = await _users.FindByIdAsync(model.Credentials.UserId);
      await _signIn.SignInAsync(user!, false);

      HttpContext.Response.Cookies.Append(
        AuthConfiguration.ProfileCookieName,
        JsonSerializer.Serialize((BaseUserProfileModel)result.User!),
        AuthConfiguration.ProfileCookieOptions);

      return Ok(result);
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
  }

  /// <summary>
  /// Confirm user's email change.
  /// </summary>
  /// <param name="model">Email change token model.</param>
  /// <returns>Email change result.</returns>
  [HttpPost("email/confirm-change")]
  public async Task<IActionResult> ConfirmEmailChange(AnonymousSetEmailModel model)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest();
    }

    try
    {
      var result = await _user.ConfirmEmailChange(model);
      if (result.Errors.Count != 0)
      {
        return BadRequest(result);
      }

      var user = await _users.FindByIdAsync(model.Credentials.UserId);
      await _signIn.SignInAsync(user!, false);

      HttpContext.Response.Cookies.Append(
        AuthConfiguration.ProfileCookieName,
        JsonSerializer.Serialize((BaseUserProfileModel)result.User!),
        AuthConfiguration.ProfileCookieOptions);

      return Ok(result);
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
  }

  /// <summary>
  /// Complete user's registration and activate the account.
  /// </summary>
  /// <param name="model">Activation token model.</param>
  /// <returns>Account activation result.</returns>
  [HttpPut("activate")]
  public async Task<IActionResult> Activate(AnonymousSetAccountActivateModel model)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest();
    }

    try
    {
      var result = await _account.Activate(model);
      if (result.Errors.Count != 0)
      {
        return BadRequest(result);
      }

      var user = await _users.FindByIdAsync(model.Credentials.UserId);
      await _signIn.SignInAsync(user!, false);

      HttpContext.Response.Cookies.Append(
        AuthConfiguration.ProfileCookieName,
        JsonSerializer.Serialize((BaseUserProfileModel)result.User!),
        AuthConfiguration.ProfileCookieOptions);

      return Ok(result.User);
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
  }
}
