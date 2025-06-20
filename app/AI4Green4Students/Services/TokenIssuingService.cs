namespace AI4Green4Students.Services;

using Constants;
using Data.Entities.Identity;
using Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Models.Account.Token;

public class TokenIssuingService
{
  private readonly ActionContext _actionContext;
  private readonly UserManager<ApplicationUser> _users;

  public TokenIssuingService(
    IActionContextAccessor actionContextAccessor,
    UserManager<ApplicationUser> users
  )
  {
    _users = users;
    _actionContext = actionContextAccessor.ActionContext ??
                     throw new InvalidOperationException("Failed to get the ActionContext");
  }

  /// <summary>
  /// Generate an account Confirmation token link.
  /// </summary>
  /// <param name="user">The user to issue the token for.</param>
  public async Task<string> GenerateAccountConfirmationLink(ApplicationUser user)
  {
    var token = await _users.GenerateEmailConfirmationTokenAsync(user);
    var vm = new UserTokenModel(user.Id, token);

    var emailConfirmationLink = (ClientRoutes.ConfirmAccount + $"?vm={vm.ObjectToBase64UrlJson()}")
      .ToLocalUrlString(new RequestContextModel(_actionContext.HttpContext.Request));

    return emailConfirmationLink;
  }

  /// <summary>
  /// Generate Password Reset token link.
  /// </summary>
  /// <param name="user">The user to issue the token for.</param>
  public async Task<string> GeneratePasswordResetLink(ApplicationUser user)
  {
    var token = await _users.GeneratePasswordResetTokenAsync(user);
    var vm = new UserTokenModel(user.Id, token);

    var passwordResetLink = (ClientRoutes.ResetPassword + $"?vm={vm.ObjectToBase64UrlJson()}")
      .ToLocalUrlString(new RequestContextModel(_actionContext.HttpContext.Request));

    return passwordResetLink;
  }

  /// <summary>
  /// Generate Email Change token link.
  /// </summary>
  /// <param name="user">The user to issue the token for.</param>
  /// <param name="newEmail">New email address to generate token for.</param>
  public async Task<string> GenerateEmailChangeLink(ApplicationUser user, string newEmail)
  {
    var token = await _users.GenerateChangeEmailTokenAsync(user, newEmail);
    var vm = new EmailChangeTokenModel(user.Id, token, newEmail);

    var emailChangeLink = (ClientRoutes.ConfirmEmailChange + $"?vm={vm.ObjectToBase64UrlJson()}")
      .ToLocalUrlString(new RequestContextModel(_actionContext.HttpContext.Request));

    return emailChangeLink;
  }

  /// <summary>
  /// Generate ActivateAccount token link.
  /// </summary>
  /// <param name="user">The user to issue the token for.</param>
  public async Task<string> GenerateAccountActivationLink(ApplicationUser user)
  {
    var token = await _users.GenerateUserTokenAsync(user, "Default", "ActivateAccount");
    var vm = new UserTokenModel(user.Id, token);

    var activationLink = (ClientRoutes.ConfirmAccountActivation + $"?vm={vm.ObjectToBase64UrlJson()}")
      .ToLocalUrlString(new RequestContextModel(_actionContext.HttpContext.Request));

    return activationLink;
  }
}
