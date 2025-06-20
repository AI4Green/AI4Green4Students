namespace AI4Green4Students.Services;

using System.Globalization;
using Config;
using Constants;
using Data;
using Data.Entities.Identity;
using EmailServices;
using Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models.Account.Email;
using Models.Account.Password;
using Models.Emails;
using Models.User;

public class UserService
{
  private readonly AccountEmailService _accountEmail;
  private readonly ApplicationDbContext _db;
  private readonly RegistrationOptions _registerConfig;
  private readonly RegistrationRuleService _registrationRules;
  private readonly RoleManager<IdentityRole> _roles;
  private readonly TokenIssuingService _tokens;
  private readonly UserManager<ApplicationUser> _user;
  private readonly UserProfileService _userProfile;

  public UserService(
    ApplicationDbContext db,
    UserManager<ApplicationUser> user,
    AccountEmailService accountEmail,
    RoleManager<IdentityRole> roles,
    IOptions<RegistrationOptions> registerConfig,
    RegistrationRuleService registrationRules,
    TokenIssuingService tokens,
    UserProfileService userProfile
  )
  {
    _db = db;
    _user = user;
    _accountEmail = accountEmail;
    _roles = roles;
    _registerConfig = registerConfig.Value;
    _registrationRules = registrationRules;
    _tokens = tokens;
    _userProfile = userProfile;
  }

  /// <summary>
  /// Checks if registration rules are in place and
  /// the provided Email Address satisfies Registration Rules as a LOWERCASE string (!)
  /// Or if the rules are disabled, simply returns true without hitting the db)
  /// </summary>
  /// <param name="email">The email address to check</param>
  /// <returns></returns>
  public async Task<bool> CanRegister(string email) =>
    _registerConfig.UseRules && await _registrationRules.ValidEmail(email) || _registerConfig is { UseRules: false };

  /// <summary>
  /// Set a User's UI Culture
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="cultureName"></param>
  /// <returns></returns>
  public async Task SetUICulture(string userId, string cultureName)
  {
    // verify it's a real culture name
    var culture = CultureInfo.GetCultureInfoByIetfLanguageTag(cultureName);

    var user = await _db.Users.FindAsync(userId) ?? throw new KeyNotFoundException();

    user.UICulture = culture.Name;

    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// List users.
  /// </summary>
  /// <returns>Users.</returns>
  public async Task<List<UserModel>> List()
  {
    var users = await _user.Users.ToListAsync();
    var list = new List<UserModel>();
    foreach (var user in users)
    {
      var roles = await _user.GetRolesAsync(user);
      list.Add(new UserModel(
        user.Id,
        user.Email!,
        user.FullName,
        user.EmailConfirmed,
        user.UICulture,
        roles.ToList()
      ));
    }
    return list;
  }

  /// <summary>
  /// Get a user.
  /// </summary>
  /// <param name="id">User id.</param>
  /// <returns>User.</returns>
  public async Task<UserModel> Get(string id)
  {
    var user = await _user.FindByIdAsync(id) ?? throw new KeyNotFoundException("User not found.");
    var roles = await _user.GetRolesAsync(user);
    var model = new UserModel(
      user.Id,
      user.Email!,
      user.FullName,
      user.EmailConfirmed,
      user.UICulture,
      roles.ToList()
    );

    return model;
  }

  /// <summary>
  /// Delete a user.
  /// </summary>
  /// <param name="id">User id.</param>
  /// <param name="notify">Whether to notify the user.</param>
  public async Task Delete(string id, bool notify = false)
  {
    var user = await _user.FindByIdAsync(id) ?? throw new KeyNotFoundException("User not found.");
    var result = await _user.DeleteAsync(user);
    if (!result.Succeeded)
    {
      throw new InvalidOperationException("Failed to delete.");
    }

    if (notify && !string.IsNullOrEmpty(user.Email))
    {
      await _accountEmail.SendDeleteUpdate(new EmailAddress(user.Email)
      {
        Name = user.FullName
      });
    }
  }

  /// <summary>
  /// Update user roles.
  /// </summary>
  /// <param name="id">User id.</param>
  /// <param name="roles">Roles to assign.</param>
  public async Task UpdateRoles(string id, List<string> roles)
  {
    if (roles.Count < 1)
    {
      throw new ArgumentException("Minimum of one role required.");
    }

    var validRoles = await _roles.Roles.ToListAsync();
    var areRolesValid = roles.All(x =>
      validRoles.Any(y => y.NormalizedName != null && y.NormalizedName.Equals(x, StringComparison.OrdinalIgnoreCase))
    );

    if (!areRolesValid)
    {
      throw new ArgumentException("Invalid roles.");
    }

    var user = await _user.FindByIdAsync(id) ?? throw new KeyNotFoundException("User not found.");
    var userRoles = await _user.GetRolesAsync(user);
    await _user.RemoveFromRolesAsync(user, userRoles);
    await _user.AddToRolesAsync(user, roles);
  }

  /// <summary>
  /// Request a password reset.
  /// </summary>
  /// <param name="userIdOrEmail">The user ID or email address.</param>
  /// <param name="request">Request.</param>
  public async Task RequestPasswordReset(string userIdOrEmail, RequestContextModel request)
  {
    var user = (await _user.FindByIdAsync(userIdOrEmail)
                ?? await _user.FindByEmailAsync(userIdOrEmail))
               ?? throw new KeyNotFoundException();

    await _accountEmail.SendPasswordReset(
      new EmailAddress(user.Email!)
      {
        Name = user.FullName
      },
      await _tokens.GeneratePasswordResetLink(user),
      (ClientRoutes.ResendResetPassword + $"?vm={new { UserId = user.Id }.ObjectToBase64UrlJson()}")
      .ToLocalUrlString(request));
  }

  /// <summary>
  /// Reset a user's password.
  /// </summary>
  /// <param name="model">The model containing the user ID and token.</param>
  /// <returns>Reset password result.</returns>
  public async Task<PasswordResultModel> ResetPassword(AnonymousSetPasswordModel model)
  {
    var errors = new List<string>();
    var user = await _user.FindByIdAsync(model.Credentials.UserId) ?? throw new KeyNotFoundException();

    var result = await _user.ResetPasswordAsync(user, model.Credentials.Token, model.Data.Password);
    if (result.Errors.Any())
    {
      errors.AddRange(result.Errors.Select(x => x.Description));
      return new PasswordResultModel(errors);
    }

    if (user.EmailConfirmed)
    {
      return new PasswordResultModel(errors, await _userProfile.BuildProfile(user));
    }

    return new PasswordResultModel(errors, IsUnconfirmedAccount: true);
  }

  /// <summary>
  /// Start the process of changing a user's email address.
  /// </summary>
  /// <param name="id">User id.</param>
  /// <param name="email">New email to set.</param>
  public async Task RequestEmailChange(string id, string email)
  {
    var user = await _user.FindByIdAsync(id) ?? throw new KeyNotFoundException();
    if (!user.EmailConfirmed)
    {
      throw new InvalidOperationException("Current email is not confirmed yet.");
    }

    if (!await CanRegister(email))
    {
      throw new InvalidOperationException($"Email '{email}' cannot be used.");
    }

    var isNewEmailExist = await _user.FindByEmailAsync(email);
    if (isNewEmailExist is not null)
    {
      throw new InvalidOperationException($"Email '{email}' already in use.");
    }

    await _accountEmail.SendEmailChange(
      new EmailAddress(email)
      {
        Name = user.FullName
      },
      await _tokens.GenerateEmailChangeLink(user, email)
    );
  }

  /// <summary>
  /// Confirm a user's email change.
  /// </summary>
  /// <param name="model">The model containing the user ID and token.</param>
  /// <returns>Email change result.</returns>
  public async Task<EmailResultModel> ConfirmEmailChange(AnonymousSetEmailModel model)
  {
    var errors = new List<string>();

    var user = await _user.FindByIdAsync(model.Credentials.UserId) ?? throw new KeyNotFoundException();
    if (!user.EmailConfirmed)
    {
      errors.Add("Current email is not confirmed yet.");
      return new EmailResultModel(errors, IsUnconfirmedAccount: true);
    }

    var result = await _user.ChangeEmailAsync(user, model.Data.NewEmail, model.Credentials.Token);
    if (result.Errors.Any())
    {
      errors.AddRange(result.Errors.Select(x => x.Description));
      return new EmailResultModel(errors);
    }

    user.UserName = model.Data.NewEmail;
    await _user.UpdateAsync(user);

    return new EmailResultModel(errors, await _userProfile.BuildProfile(user));
  }

  /// <summary>
  /// Invite a user.
  /// </summary>
  /// <param name="model"></param>
  /// <param name="uiCulture"></param>
  /// <returns></returns>
  public async Task<UserInviteResultModel> Invite(UserInviteModel model, string uiCulture)
  {
    var errors = new List<string>();

    if (!await CanRegister(model.Email))
    {
      errors.Add("The email address provided is not eligible for registration.");
      return new UserInviteResultModel(errors, true);
    }

    if (string.IsNullOrEmpty(model.Email) || model.Roles.Count < 1)
    {
      errors.Add("Minimum of one role and email required");
      return new UserInviteResultModel(errors, HasNoRoles: true);
    }

    var validRoles = await _roles.Roles.ToListAsync();
    var areRolesValid = model.Roles.All(x =>
      validRoles.Any(y => y.NormalizedName != null && y.NormalizedName.Equals(x, StringComparison.OrdinalIgnoreCase)));

    if (!areRolesValid)
    {
      errors.Add("Invalid roles provided.");
      return new UserInviteResultModel(errors, HasInvalidRoles: true);
    }

    var user = await _user.FindByEmailAsync(model.Email);
    if (user is not null)
    {
      errors.Add($"The email address '{model.Email} is already registered.");
      return new UserInviteResultModel(errors, IsExistingEmail: true);
    }

    var entity = new ApplicationUser
    {
      UserName = model.Email, Email = model.Email, UICulture = uiCulture
    };

    var createResult = await _user.CreateAsync(entity);
    if (!createResult.Succeeded)
    {
      errors.AddRange(createResult.Errors.Select(x => x.Description));
      return new UserInviteResultModel(errors);
    }
    await _user.AddToRolesAsync(entity, model.Roles);

    await _accountEmail.SendUserInvite(
      new EmailAddress(entity.Email)
      {
        Name = entity.FullName
      },
      await _tokens.GenerateAccountActivationLink(entity));

    return new UserInviteResultModel(errors);
  }

  /// <summary>
  /// Resend an invitation.
  /// </summary>
  /// <param name="userIdOrEmail">User id or email.</param>
  /// <param name="request">Request.</param>
  public async Task InviteResend(string userIdOrEmail, RequestContextModel request)
  {
    var user = (await _user.FindByIdAsync(userIdOrEmail)
                ?? await _user.FindByEmailAsync(userIdOrEmail))
               ?? throw new KeyNotFoundException();

    if (await IsPendingOnlyEmailConfirmation(user))
    {
      await _accountEmail.SendAccountConfirmation(
        new EmailAddress(user.Email!)
        {
          Name = user.FullName
        },
        await _tokens.GenerateAccountConfirmationLink(user),
        (ClientRoutes.ResendConfirm +
         $"?vm={new { UserId = user.Id }.ObjectToBase64UrlJson()}")
        .ToLocalUrlString(request));
    }
    else
    {
      await _accountEmail.SendUserInvite(
        new EmailAddress(user.Email!)
        {
          Name = user.FullName
        },
        await _tokens.GenerateAccountActivationLink(user));
    }
  }

  /// <summary>
  /// Check if the user is pending only email confirmation.
  /// </summary>
  /// <param name="user">User entity.</param>
  /// <returns>Result.</returns>
  private async Task<bool> IsPendingOnlyEmailConfirmation(ApplicationUser user)
    => !user.EmailConfirmed &&
       await _user.HasPasswordAsync(user) &&
       !string.IsNullOrWhiteSpace(user.PasswordHash) &&
       !string.IsNullOrWhiteSpace(user.FullName);
}
