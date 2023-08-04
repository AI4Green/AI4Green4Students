using AI4Green4Students.Auth;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Identity;

namespace AI4Green4Students.Data;

public class UserDataSeeder
{
  private readonly UserManager<ApplicationUser> _users;
    private readonly RegistrationRuleService _registrationRule;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly IConfiguration _config;

    public UserDataSeeder(
      UserManager<ApplicationUser> users,
      RegistrationRuleService registrationRule,
      IPasswordHasher<ApplicationUser> passwordHasher,
      IConfiguration config)
    {
      _users = users;
      _registrationRule = registrationRule;
      _passwordHasher = passwordHasher;
      _config = config;
    }

    /// <summary>
    /// Seed an initial admin user to use for setup if no admin users exist. Also add the email to the allow list.
    /// Or update the password of the existing admin user if one exists
    /// </summary>
    /// <returns></returns>
    public async Task SeedAdminUser()
    {
      // check an actual password has been configured
      var pwd = _config["Root:Password"];
      if (string.IsNullOrEmpty(pwd))
      {
        throw new ApplicationException(@"
A non-empty password must be configured for seeding the initial Admin User.
Please set Root:Password in a settings or user secrets file,
or the environment variable DOTNET_Hosted_AdminPassword");
      }

      // Add the user if they don't exist, else update them,
      var email = _config["Root:EmailAddress"] ?? "admin@local.com"; //use 'admin@local.com' as email if Root:EmailAddress id not configured
      var adminUsers = await _users.GetUsersInRoleAsync(Roles.Admin); // check if there are any admin users
      if (!adminUsers.Any())
      {
        var user = new ApplicationUser
        {
          FullName = "Super Admin",
          Email = email,
          EmailConfirmed = true,
          UserName = email,
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, pwd);
        
        await _users.CreateAsync(user);
        await _users.AddToRoleAsync(user, Roles.Admin);
        await _registrationRule.Create(new CreateRegistrationRuleModel(email, false)); // also add their email to allow list
      } 
      else 
      {
        var user = await _users.FindByEmailAsync(email); // find the user by email
        if (user is not null && await _users.IsInRoleAsync(user, Roles.Admin))
        {
          user.PasswordHash = _passwordHasher.HashPassword(user, pwd);
          await _users.UpdateAsync(user);
        }
      }
    }
}
