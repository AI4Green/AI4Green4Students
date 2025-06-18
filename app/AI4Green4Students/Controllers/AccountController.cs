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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
  private readonly OidcOptions _oidcOptions;
  private readonly RoleManager<IdentityRole> _roles;
  private readonly SignInManager<ApplicationUser> _signIn;
  private readonly TokenIssuingService _tokens;
  private readonly UserService _user;
  private readonly UserAccountOptions _userAccountOptions;
  private readonly UserManager<ApplicationUser> _users;

  public AccountController(
    UserManager<ApplicationUser> users,
    SignInManager<ApplicationUser> signIn,
    UserService user,
    TokenIssuingService tokens,
    RoleManager<IdentityRole> roles,
    IOptions<UserAccountOptions> userAccountOptions,
    IOptions<OidcOptions> oidcOptions)
  {
    _users = users;
    _signIn = signIn;
    _user = user;
    _tokens = tokens;
    _roles = roles;
    _oidcOptions = oidcOptions.Value;
    _userAccountOptions = userAccountOptions.Value;
  }

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

  [HttpPost("register")]
  public async Task<IActionResult> Register(RegisterAccountModel model)
  {
    RegisterAccountResult regResult = new();

    if (ModelState.IsValid) // Additional Pre-registration checks
    {
      if (!await _user.CanRegister(model.Email))
      {
        ModelState.AddModelError(string.Empty, "The email address provided is not eligible for registration.");
        regResult = regResult with { IsNotAllowlisted = true, };
      }
      
      // check if user already exist
      var user = await _users.FindByEmailAsync(model.Email);
      if (user is not null && (user.EmailConfirmed || await IsPendingOnlyEmailConfirmation(user)))
      {
        ModelState.AddModelError(string.Empty, "The email address is already registered.");
        regResult = regResult with { IsExistingUser = true, };
      }
    }

    if (ModelState.IsValid) // Actual success route
    {
      var user = await _users.FindByEmailAsync(model.Email);
      
      // check if user exist. If yes, then mostly likely user email is not confirmed and 
      // some fields are incomplete, such as full name and password might have not been set.
      if (user is not null) 
      {
        var hashedPassword = _users.PasswordHasher.HashPassword(user, model.Password); // hash the password
        user.PasswordHash = hashedPassword; // update password
        user.FullName = model.FullName; // update user full name
        
        await _users.UpdateAsync(user); // update user
       
        var userRoles = await _users.GetRolesAsync(user); // get user roles
        if (userRoles.Count == 0) await _users.AddToRoleAsync(user, Roles.Student); // assign student role if roles not found

        await _tokens.SendAccountConfirmation(user); // send confirmation email
        return NoContent();
      }
      
      var newUser = new ApplicationUser // Register a new user
      {
        UserName = model.Email,
        Email = model.Email,
        FullName = model.FullName,
        UICulture = Request.GetUICulture().Name
      };

      var result = await _users.CreateAsync(newUser, model.Password);
      if (result.Succeeded)
      { 
        await _users.AddToRoleAsync(newUser, Roles.Student); // assign student role when user self register
        await _tokens.SendAccountConfirmation(newUser);
        return NoContent();
      }

      foreach (var e in result.Errors)
        ModelState.AddModelError(string.Empty, e.Description);
    }
    
    return BadRequest(regResult with { Errors = ModelState.CollapseErrors() });
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login(LoginModel model)
  {
    if (ModelState.IsValid)
    {
      var result = await _signIn.PasswordSignInAsync(model.Username, model.Password, false, true);
      var user = await _users.FindByNameAsync(model.Username);

      if (result.Succeeded)
      {
        if (user is null)
          throw new InvalidOperationException(
            $"Successfully signed in user could not be retrieved! Username: {model.Username}");

        var profile = await _user.BuildProfile(user);

        // Write a basic Profile Cookie for JS
        HttpContext.Response.Cookies.Append(
          AuthConfiguration.ProfileCookieName,
          JsonSerializer.Serialize((BaseUserProfileModel)profile),
          AuthConfiguration.ProfileCookieOptions);

        return Ok(new LoginResult
        {
          User = profile,
        });
      }

      // Handle login failures
      if (result.IsNotAllowed)
      {
        // But WHY was it disallowed?
        // Distinguish some specific cases we care about
        // So the login form can behave accordingly

        LoginResult loginResult = user switch
        {
          { EmailConfirmed: false } => new() { IsUnconfirmedAccount = true },
          _ => new() { }
        };

        return BadRequest(loginResult);
      }
    }

    return BadRequest(new LoginResult
    {
      Errors = ModelState.CollapseErrors()
    });
  }

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

  [HttpPost("confirm")]
  public async Task<IActionResult> Confirm(UserTokenModel model)
  {
    if (ModelState.IsValid)
    {
      var user = await _users.FindByIdAsync(model.UserId);
      if (user is null) return NotFound();

      var result = await _users.ConfirmEmailAsync(user, model.Token);

      if (!result.Errors.Any())
      {
        await _signIn.SignInAsync(user, false);

        var profile = await _user.BuildProfile(user);

        // Write a basic Profile Cookie for JS
        HttpContext.Response.Cookies.Append(
          AuthConfiguration.ProfileCookieName,
          JsonSerializer.Serialize((BaseUserProfileModel)profile),
          AuthConfiguration.ProfileCookieOptions);

        return Ok(profile);
      }
    }

    return BadRequest();
  }

  [HttpPost("confirm/resend")]
  public async Task<IActionResult> ConfirmResend([FromBody] string userIdOrEmail)
  {
    var user = await _users.FindByIdAsync(userIdOrEmail);
    if (user is null) user = await _users.FindByEmailAsync(userIdOrEmail);
    if (user is null) return NotFound();

    // Check if user is only pending for email confirmation
    if (await IsPendingOnlyEmailConfirmation(user)) 
      await _tokens.SendAccountConfirmation(user); // send confirmation email if only email confirmation pending
    else 
      await _tokens.SendUserInvite(user); // send invite email if registration is not complete
    return NoContent();
  }

  [HttpPost("password/reset")]
  public async Task<IActionResult> RequestPasswordReset([FromBody] string userIdOrEmail)
  {
    var user = await _users.FindByIdAsync(userIdOrEmail);
    if (user is null) user = await _users.FindByEmailAsync(userIdOrEmail);
    if (user is null) return NotFound();

    await _tokens.SendPasswordReset(user);
    return NoContent();
  }

  [HttpPost("password")]
  public async Task<IActionResult> ResetPassword(AnonymousSetPasswordModel model)
  {
    if (ModelState.IsValid)
    {
      var user = await _users.FindByIdAsync(model.Credentials.UserId);
      if (user is null) return NotFound();

      var result = await _users.ResetPasswordAsync(user, model.Credentials.Token, model.Data.Password);

      if (!result.Errors.Any())
      {
        if (user.EmailConfirmed)
        {
          await _signIn.SignInAsync(user, false);

          var profile = await _user.BuildProfile(user);

          // Write a basic Profile Cookie for JS
          HttpContext.Response.Cookies.Append(
            AuthConfiguration.ProfileCookieName,
            JsonSerializer.Serialize((BaseUserProfileModel)profile),
            AuthConfiguration.ProfileCookieOptions);

          return Ok(new SetPasswordResult
          {
            User = profile
          });
        }
        else
        {
          return Ok(new SetPasswordResult
          {
            IsUnconfirmedAccount = true
          });
        }
      }
    }

    return BadRequest(new SetPasswordResult
    {
      Errors = ModelState.CollapseErrors()
    });
  }
  
  /// <summary>
  /// Update user's email
  /// User must have confirmed email before their email can be changed
  /// If user has not confirmed their email, this suggests user's account is not active yet
  /// </summary>
  /// <param name="model">userId, NewEmail and ChangeEmail token</param>
  /// <returns></returns>
  [HttpPost("confirmEmailChange")]
  public async Task<IActionResult> ConfirmEmailChange(AnonymousSetEmailModel model)
  {
    if (ModelState.IsValid)
    {
      var user = await _users.FindByIdAsync(model.Credentials.UserId);
      if (user is null) return NotFound();
      
      if (!user.EmailConfirmed) // user must have confirmed email before their email can be changed 
        return BadRequest(new SetEmailResult { IsUnconfirmedAccount = true }); 
      
      var result = await _users.ChangeEmailAsync(user, model.Data.NewEmail, model.Credentials.Token); // change email using token and new email

      if (!result.Errors.Any())
      {
        user.UserName = model.Data.NewEmail; 
        await _users.UpdateAsync(user); // update username to new email
        
        await _signIn.SignInAsync(user, false);

        var profile = await _user.BuildProfile(user);

        // Write a basic Profile Cookie for JS
        HttpContext.Response.Cookies.Append(
          AuthConfiguration.ProfileCookieName,
          JsonSerializer.Serialize((BaseUserProfileModel)profile),
          AuthConfiguration.ProfileCookieOptions);

        return Ok(new SetEmailResult { User = profile });
      }
    }
    return BadRequest(new SetEmailResult { Errors = ModelState.CollapseErrors() });
  }
  
  /// <summary>
  /// Complete user's registration and activate account
  /// User must have valid token
  /// </summary>
  /// <param name="model">userId, data and Activation token</param>
  /// <returns></returns>
  [HttpPut("activate")] //api/account/activate
  public async Task<IActionResult> Activate (AnonymousSetAccountActivateModel model)
  {
    if (ModelState.IsValid)
    { 
      var user = await _users.FindByIdAsync(model.Credentials.UserId);
      if (user is null) return NotFound();
      
      var isTokenValid = await _users.VerifyUserTokenAsync(user, "Default", "ActivateAccount", model.Credentials.Token); // validate token
      
      if (!isTokenValid || user.EmailConfirmed) // Basically invalidating token if user has already activated account
        return BadRequest(new SetAccountActivateResult() { IsActivationTokenInvalid = true });
      
      // if token is valid, then do the following
      var hashedPassword = _users.PasswordHasher.HashPassword(user, model.Data.Password); // hash the password
      user.PasswordHash = hashedPassword; // update password
      user.EmailConfirmed = true; // update Account status
      user.FullName = model.Data.FullName; // update user full name
      
      // TODO: add more fields to update,

      await _users.UpdateAsync(user); // update the user

      await _signIn.SignInAsync(user, false); // sign in the user
      
      var profile = await _user.BuildProfile(user);
      
      // Write a basic Profile Cookie for JS
      HttpContext.Response.Cookies.Append(
        AuthConfiguration.ProfileCookieName,
        JsonSerializer.Serialize((BaseUserProfileModel)profile),
        AuthConfiguration.ProfileCookieOptions);
      return Ok(new SetAccountActivateResult { User = profile });
    }
    return BadRequest(new SetAccountActivateResult{ Errors = ModelState.CollapseErrors() });
  }
  
  /// <summary>
  /// Create a user with a basic information.
  /// Send an invite email to the user if SendUserInviteEmail config is not false (By default, its true)
  /// </summary>
  /// <param name="model">Bind request body values to the model</param>
  /// <returns>
  /// Return activation link to the client with a status 200 if GenerateInviteLink config is true.
  /// Return 204 status if GenerateInviteLink config is false.
  /// </returns>
  [Authorize (nameof(AuthPolicies.CanInviteUsers))]
  [Authorize (nameof(AuthPolicies.CanInviteInstructors))]
  [Authorize (nameof(AuthPolicies.CanInviteStudents))]
  [HttpPost("invite")] //api/account/invite
  public async Task<IActionResult> Invite([FromBody] UserModel model)
  {
    RegisterAccountResult regResult = new();

    if (ModelState.IsValid)
    {
      if (!await _user.CanRegister(model.Email))
      {
        ModelState.AddModelError(string.Empty, "The email address provided is not eligible for registration.");
        regResult = regResult with { IsNotAllowlisted = true, };
      }
      
      // Only email and roles is required for invite
      if (string.IsNullOrEmpty(model.Email) || model.Roles.Count < 1)
      {
        ModelState.AddModelError(string.Empty, "Minimum of one role and email required");
        regResult = regResult with { IsRolesNotValidOrSelected = true };
      }
    
      // check role is available and valid
      var rolesAvailable = await _roles.Roles.ToListAsync(); // get list of available roles
      var valid = model.Roles.All(x => rolesAvailable.Any(y => x == y.NormalizedName));
      if (!valid)
      {
        ModelState.AddModelError(string.Empty, "Invalid roles selected");
        regResult = regResult with { IsRolesNotValidOrSelected = true };
      }
    
      // check if user already exist
      var user = await _users.FindByEmailAsync(model.Email);
      if (user is not null)
      {
        ModelState.AddModelError(string.Empty, "The email address is already registered.");
        regResult = regResult with { IsExistingUser = true, };
      }
    }

    if (ModelState.IsValid)
    {
      var newUser = new ApplicationUser() // Register a new user
      {
        UserName = model.Email,
        Email = model.Email,
        UICulture = Request.GetUICulture().Name
      };
    
      var result = await _users.CreateAsync(newUser);
      if (result.Succeeded)
      {
        await _users.AddToRolesAsync(newUser, model.Roles); // assign roles to the user

        if (_userAccountOptions.SendEmail) // Check the config and send email invite if true
          await _tokens.SendUserInvite(newUser);

        if (_userAccountOptions.GenerateLink) // Return 200 status with activation link, which can be extracted by client
          return Ok(await _tokens.GenerateAccountActivationLink(newUser)); // Check 200 status in the client for ActivationLink
        
        return NoContent(); // Return 204 if no ActivationLink 
      }
      foreach (var e in result.Errors)
        ModelState.AddModelError(string.Empty, e.Description);
    }
    return BadRequest(regResult with { Errors = ModelState.CollapseErrors() });
  }
  
  /// <summary>
  /// Resend or generate invite link or account confirmation link based on the user status
  /// Send Account confirmation email if the user is only pending email confirmation.
  /// Else, send invite email.
  /// </summary>
  /// <param name="userIdOrEmail">Invite to be sent for user with Id or email</param>
  /// <returns>
  /// Return activation link to the client with a status 200 if GenerateInviteLink config is true.
  /// Return 204 status if GenerateInviteLink config is false.
  /// </returns>
  [Authorize (nameof(AuthPolicies.CanInviteUsers))]
  [Authorize (nameof(AuthPolicies.CanInviteInstructors))]
  [Authorize (nameof(AuthPolicies.CanInviteStudents))]
  [HttpPut("invite/resend")]
  public async Task<IActionResult> InviteResend([FromBody] string userIdOrEmail)
  {
    var user = await _users.FindByIdAsync(userIdOrEmail);
    if (user is null) user = await _users.FindByEmailAsync(userIdOrEmail);
    if (user is null) return NotFound();

    var isOnlyPendingEmailConfirmation = await IsPendingOnlyEmailConfirmation(user);
    
    // Check the config and resend email confirmation or invite
    if (_userAccountOptions.SendEmail)
    {
      if (isOnlyPendingEmailConfirmation) 
        await _tokens.SendAccountConfirmation(user); // send account confirmation email
      else await _tokens.SendUserInvite(user); // send user invite email
    }

    if (!_userAccountOptions.GenerateLink) return NoContent(); // Return 204 if link generation is not enabled by config 
    
    var link = isOnlyPendingEmailConfirmation 
      ? (await _tokens.GenerateAccountConfirmationLink(user)).EmailConfirmationLink // generate account confirmation link
      : (await _tokens.GenerateAccountActivationLink(user)).ActivationLink; // generate user invite link
      
    // Return 200 status with activation or confirmation link, which can be extracted by client
    // Check 200 status in the client for ActivationLink or EmailConfirmationLink
    return Ok(link);
  }
  
  /// <summary>
  /// Check if the user is only pending email confirmation as a part of the registration process
  /// Useful when an invited user tries to self-register after an invite
  /// </summary>
  /// <param name="user"></param>
  /// <returns></returns>
  [NonAction]
  private async Task<bool> IsPendingOnlyEmailConfirmation (ApplicationUser user)
  {
    return 
      !user.EmailConfirmed && // email not confirmed
      
      await _users.HasPasswordAsync(user) &&
      !string.IsNullOrWhiteSpace(user.PasswordHash) && // password set
      !string.IsNullOrWhiteSpace(user.FullName); // full name set
  }
}
