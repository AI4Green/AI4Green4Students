namespace AI4Green4Students.Services;

using Auth;
using Constants;
using Data.Entities.Identity;
using EmailServices;
using Extensions;
using Microsoft.AspNetCore.Identity;
using Models.Account;
using Models.Account.Email;
using Models.Account.Login;
using Models.Account.Register;
using Models.Account.Token;
using Models.Emails;

public class AccountService
{
  private readonly AccountEmailService _accountEmail;
  private readonly SignInManager<ApplicationUser> _signIn;
  private readonly TokenIssuingService _tokens;
  private readonly UserManager<ApplicationUser> _user;
  private readonly UserService _users;
  private readonly UserProfileService _userProfile;

  public AccountService(
    UserManager<ApplicationUser> user,
    SignInManager<ApplicationUser> signIn,
    AccountEmailService accountEmail,
    TokenIssuingService tokens,
    UserService users,
    UserProfileService userProfile
  )
  {
    _user = user;
    _signIn = signIn;
    _accountEmail = accountEmail;
    _tokens = tokens;
    _users = users;
    _userProfile = userProfile;
  }

  /// <summary>
  /// Login a user with their username and password.
  /// </summary>
  /// <param name="model">Login model.</param>
  /// <returns>Result.</returns>
  public async Task<LoginResultModel> Login(LoginModel model)
  {
    var errors = new List<string>();

    var signInResult = await _signIn.PasswordSignInAsync(model.Username, model.Password, false, true);
    var user = await _user.FindByNameAsync(model.Username);

    if (signInResult.Succeeded)
    {
      if (user is null)
      {
        throw new InvalidOperationException(
          $"Successfully signed in user could not be retrieved! Username: {model.Username}");
      }

      return new LoginResultModel(errors, await _userProfile.BuildProfile(user));
    }

    if (signInResult.IsNotAllowed)
    {
      var loginResult = user switch
      {
        { EmailConfirmed: false } => new LoginResultModel(errors, IsUnconfirmedAccount: true),
        _ => new LoginResultModel(errors)
      };
      return loginResult;
    }

    return new LoginResultModel(errors);
  }

  /// <summary>
  /// Register a new user with their email and password.
  /// </summary>
  /// <param name="model">Register model.</param>
  /// <param name="request">Request.</param>
  /// <returns></returns>
  public async Task<RegisterResultModel> Register(RegisterAccountModel model, RequestContextModel request)
  {
    var result = new RegisterResultModel(Errors: new List<string>());
    var errors = new List<string>();

    if (!await _users.CanRegister(model.Email))
    {
      errors.Add("The email address provided is not eligible for registration.");
      result = result with
      {
        IsNotAllowedToRegister = true,
        Errors = errors
      };

      return result;
    }

    var user = await _user.FindByEmailAsync(model.Email);
    if (user is not null)
    {
      if (user.EmailConfirmed || await IsPendingOnlyEmailConfirmation(user))
      {
        errors.Add("The email address is already registered.");
        result = result with
        {
          IsExistingUser = true,
          Errors = errors
        };
        return result;
      }

      // when the user is found, but the email is not confirmed including incomplete fields
      var hashedPassword = _user.PasswordHasher.HashPassword(user, model.Password);
      user.PasswordHash = hashedPassword;
      user.FullName = model.FullName;

      await _user.UpdateAsync(user);

      var userRoles = await _user.GetRolesAsync(user);
      if (userRoles.Count == 0)
      {
        await _user.AddToRoleAsync(user, Roles.Student);
      }

      await _accountEmail.SendAccountConfirmation(
        new EmailAddress(user.Email!)
        {
          Name = user.FullName
        },
        await _tokens.GenerateAccountConfirmationLink(user),
        (ClientRoutes.ResendConfirm + $"?vm={new { UserId = user.Id }.ObjectToBase64UrlJson()}")
        .ToLocalUrlString(request)
      );

      return result;
    }

    var entity = new ApplicationUser
    {
      UserName = model.Email,
      Email = model.Email,
      FullName = model.FullName,
      UICulture = request.UiCulture
    };

    var createResult = await _user.CreateAsync(entity, model.Password);
    if (createResult.Succeeded)
    {
      await _user.AddToRoleAsync(entity, Roles.Student);
      await _accountEmail.SendAccountConfirmation(
        new EmailAddress(entity.Email)
        {
          Name = entity.FullName
        },
        await _tokens.GenerateAccountConfirmationLink(entity),
        (ClientRoutes.ResendConfirm + $"?vm={new { UserId = entity.Id }.ObjectToBase64UrlJson()}")
        .ToLocalUrlString(request)
      );

      return result;
    }

    errors.Add("Registration failed. Please try again later.");
    return result with
    {
      Errors = errors
    };
  }

  /// <summary>
  /// Confirm a user account.
  /// </summary>
  /// <param name="model">Model.</param>
  /// <returns>Result.</returns>
  public async Task<AccountResultModel> Confirm(UserTokenModel model)
  {
    var errors = new List<string>();
    var user = await _user.FindByIdAsync(model.UserId) ?? throw new KeyNotFoundException();

    var result = await _user.ConfirmEmailAsync(user, model.Token);
    if (result.Errors.Any())
    {
      errors.AddRange(result.Errors.Select(x => x.Description));
      return new AccountResultModel(errors);
    }

    return new AccountResultModel(errors, await _userProfile.BuildProfile(user));
  }

  /// <summary>
  /// Resend the account confirmation email to a user.
  /// </summary>
  /// <param name="userIdOrEmail">User id or email.</param>
  /// <param name="request">Request.</param>
  public async Task ConfirmResend(string userIdOrEmail, RequestContextModel request)
  {
    var user = (await _user.FindByIdAsync(userIdOrEmail)
                ?? await _user.FindByEmailAsync(userIdOrEmail))
               ?? throw new KeyNotFoundException();

    if (user.Email is null)
    {
      throw new InvalidOperationException();
    }

    if (await IsPendingOnlyEmailConfirmation(user))
    {
      await _accountEmail.SendAccountConfirmation(
        new EmailAddress(user.Email)
        {
          Name = user.FullName
        },
        await _tokens.GenerateAccountConfirmationLink(user),
        (ClientRoutes.ResendConfirm + $"?vm={new { UserId = user.Id }.ObjectToBase64UrlJson()}")
        .ToLocalUrlString(request)
      );
    }
    else
    {
      await _accountEmail.SendUserInvite(
        new EmailAddress(user.Email)
        {
          Name = user.FullName
        },
        await _tokens.GenerateAccountActivationLink(user));
    }
  }

  /// <summary>
  /// Activate a user account.
  /// </summary>
  /// <param name="model">Model.</param>
  /// <returns>Result.</returns>
  public async Task<AccountResultModel> Activate(AnonymousSetAccountActivateModel model)
  {
    var errors = new List<string>();
    var user = await _user.FindByIdAsync(model.Credentials.UserId) ?? throw new KeyNotFoundException();

    var isTokenValid = await _user.VerifyUserTokenAsync(user, "Default", "ActivateAccount", model.Credentials.Token);
    if (!isTokenValid)
    {
      errors.Add("Invalid activation token.");
      return new AccountResultModel(errors, IsInvalidActivationToken: true);
    }

    if (user.EmailConfirmed)
    {
      errors.Add("Account already activated.");
      return new AccountResultModel(errors, IsInvalidActivationToken: true);
    }

    var hashedPassword = _user.PasswordHasher.HashPassword(user, model.Data.Password);
    user.PasswordHash = hashedPassword;
    user.EmailConfirmed = true;
    user.FullName = model.Data.FullName;

    await _user.UpdateAsync(user);

    return new AccountResultModel(errors, await _userProfile.BuildProfile(user));
  }

  private async Task<bool> IsPendingOnlyEmailConfirmation(ApplicationUser user)
    => !user.EmailConfirmed &&
       await _user.HasPasswordAsync(user) &&
       !string.IsNullOrWhiteSpace(user.PasswordHash) &&
       !string.IsNullOrWhiteSpace(user.FullName);
}
