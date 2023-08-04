using AI4Green4Students.Constants;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Extensions;
using AI4Green4Students.Models.Account.Email;
using AI4Green4Students.Models.Account.Password;
using AI4Green4Students.Models.Account.Token;
using AI4Green4Students.Models.Emails;
using AI4Green4Students.Services.EmailServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace AI4Green4Students.Services;

public class TokenIssuingService
{
  private readonly ActionContext _actionContext;
  private readonly UserManager<ApplicationUser> _users;
  private readonly AccountEmailService _accountEmail;

  public TokenIssuingService(
      IActionContextAccessor actionContextAccessor,
      UserManager<ApplicationUser> users,
      AccountEmailService accountEmail)
  {
    _users = users;
    _accountEmail = accountEmail;
    _actionContext = actionContextAccessor.ActionContext
      ?? throw new InvalidOperationException("Failed to get the ActionContext");
  }

  /// <summary>
  /// Get AccountConfirmation token, and email the user a link.
  /// When a user register themselves, they are sent an email with a link to confirm their account i.e confirm their email.
  /// </summary>
  /// <param name="user">The user to issue the token for and send the email to.</param>
  public async Task SendAccountConfirmation(ApplicationUser user)
  {
    var link = await GenerateAccountConfirmationLink(user);
    await _accountEmail.SendAccountConfirmation(
        new EmailAddress(user.Email) { Name = user.FullName },
        link: link.EmailConfirmationLink,
        resendLink: (ClientRoutes.ResendConfirm +
            $"?vm={new { UserId = user.Id }.ObjectToBase64UrlJson()}")
            .ToLocalUrlString(_actionContext.HttpContext.Request));
  }
  
  /// <summary>
  /// Generate Account Confirmation token link.
  /// </summary>
  /// <param name="user">The user to issue the token for.</param>
  public async Task<EmailConfirmationLinkModel> GenerateAccountConfirmationLink(ApplicationUser user)
  {
    var token = await _users.GenerateEmailConfirmationTokenAsync(user);
    var vm = new UserTokenModel(user.Id, token);

    var emailConfirmationLink =(ClientRoutes.ConfirmAccount +
                                $"?vm={vm.ObjectToBase64UrlJson()}")
      .ToLocalUrlString(_actionContext.HttpContext.Request);

    return new EmailConfirmationLinkModel(emailConfirmationLink);
  }
  
  /// <summary>
  /// Get Password Reset token link, and email the user a link.
  /// </summary>
  /// <param name="user">The user to issue the token for.</param>
  public async Task SendPasswordReset(ApplicationUser user)
  {
    var link = await GeneratePasswordResetLink(user);
    await _accountEmail.SendPasswordReset(
        new EmailAddress(user.Email) { Name = user.FullName },
        link: link.PasswordResetLink,
        resendLink: (ClientRoutes.ResendResetPassword +
            $"?vm={new { UserId = user.Id }.ObjectToBase64UrlJson()}")
            .ToLocalUrlString(_actionContext.HttpContext.Request));
  }

  /// <summary>
  /// Generate Password Reset token link.
  /// </summary>
  /// <param name="user">The user to issue the token for.</param>
  public async Task<PasswordResetLinkModel> GeneratePasswordResetLink(ApplicationUser user)
  {
    var token = await _users.GeneratePasswordResetTokenAsync(user);
    var vm = new UserTokenModel(user.Id, token);

    var passwordResetLink =(ClientRoutes.ResetPassword +
                                $"?vm={vm.ObjectToBase64UrlJson()}")
      .ToLocalUrlString(_actionContext.HttpContext.Request);

    return new PasswordResetLinkModel(passwordResetLink);
  }
  
  /// <summary>
  /// Get Email Change token link, and email the user a link.
  /// </summary>
  /// <param name="user">The user to issue the token for.</param>
  /// <param name="newEmail">New email address to generate token for and send the token to.</param>
  public async Task SendEmailChange(ApplicationUser user, string newEmail)
  {
    var link = await GenerateEmailChangeLink(user, newEmail);
    // TODO: do we need the new email in the VM?

    await _accountEmail.SendEmailChange(
      new EmailAddress(newEmail)
      {
        Name = user.FullName
      },
      link: link.EmailChangeLink);
  }

  /// <summary>
  /// Generate Email Change token link.
  /// </summary>
  /// <param name="user">The user to issue the token for.</param>
  /// <param name="newEmail">New email address to generate token for.</param>
  public async Task<EmailChangeLinkModel> GenerateEmailChangeLink(ApplicationUser user, string newEmail)
  {
    var token = await _users.GenerateChangeEmailTokenAsync(user, newEmail);
    var vm = new EmailChangeTokenModel(user.Id, token, newEmail);

    var emailChangeLink = (ClientRoutes.ConfirmEmailChange +
                             $"?vm={vm.ObjectToBase64UrlJson()}")
      .ToLocalUrlString(_actionContext.HttpContext.Request);

    return new EmailChangeLinkModel(emailChangeLink);
  }

  /// <summary>
  /// Get Account Activation token link, and send user an invite link
  /// When a user is added by an admin.
  /// Admin enters some details of the users and the email is sent to the user with a link to activate their account.
  /// User then via the link can complete the rest of their details and complete the registration/account activation process.
  /// </summary>
  /// <param name="user">The user to issue the token for.</param>
  public async Task SendUserInvite(ApplicationUser user)
  {
    var link = await GenerateAccountActivationLink(user);
    await _accountEmail.SendUserInvite(
      new EmailAddress(user.Email) { Name = user.FullName },
      link: link.ActivationLink);
  }
  
  /// <summary>
  /// Generate ActivateAccount token link.
  /// </summary>
  /// <param name="user">The user to issue the token for.</param>
  public async Task<AccountActivationLinkModel> GenerateAccountActivationLink(ApplicationUser user)
  {
    var token = await _users.GenerateUserTokenAsync(user, "Default","ActivateAccount");
    var vm = new UserTokenModel(user.Id, token);

    var activationLink = (ClientRoutes.ConfirmAccountActivation +
                          $"?vm={vm.ObjectToBase64UrlJson()}")
      .ToLocalUrlString(_actionContext.HttpContext.Request);

    return new AccountActivationLinkModel(activationLink);
  }
}

