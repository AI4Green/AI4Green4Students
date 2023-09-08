using System.Security.Claims;
using AI4Green4Students.Auth;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models;
using AI4Green4Students.Models.Project;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Identity;

namespace AI4Green4Students.Data;

public class DataSeeder
{
  private readonly RoleManager<IdentityRole> _roles;
  private readonly RegistrationRuleService _registrationRule;
  private readonly UserManager<ApplicationUser> _users;
  private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
  private readonly ProjectService _projects;
  private readonly IConfiguration _config;

  public DataSeeder(
    RoleManager<IdentityRole> roles,
    RegistrationRuleService registrationRule,
    UserManager<ApplicationUser> users,
    IPasswordHasher<ApplicationUser> passwordHasher,
    ProjectService projects,
    IConfiguration config)
  {
    _roles = roles;
    _registrationRule = registrationRule;
    _users = users;
    _passwordHasher = passwordHasher;
    _projects = projects;
    _config = config;
  }
  
    /// <summary>
    /// Ensure an individual role exists and has the specified claims
    /// </summary>
    /// <param name="roleName">The name of the role to ensure is present</param>
    /// <param name="claims">The claims the role should have</param>
    /// <returns></returns>
    private async Task SeedRole(string roleName, List<(string type, string value)> claims)
  {
    var role = await _roles.FindByNameAsync(roleName);

    // create the role if it doesn't exist
    if (role is null)
    {
      role = new IdentityRole { Name = roleName };
      await _roles.CreateAsync(role);
    }

    // ensure the role has the claims specified
    //turning this into a dictionary gives us key indexing, not needing to repeatedly enumerate the list
    var existingClaims = (await _roles.GetClaimsAsync(role)).ToDictionary(x => $"{x.Type}{x.Value}");
    foreach (var (type, value) in claims)
    {
      // only add the claim if the role doesn't already functionally have it
      if (!existingClaims.ContainsKey($"{type}{value}"))
        await _roles.AddClaimAsync(role, new Claim(type, value));
    }
  }

  public async Task SeedRoles()
  {
    // Demonstrator
    await SeedRole(Roles.Demonstrator, new()
    {
    });
    
    // Instructor
    await SeedRole(Roles.Instructor, new()
    {
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ManageUsers),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.AddStudentToProject),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.CreateProjects),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.EditProjects),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.DeleteProjects),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnProjects),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllProjects),
    });

    // Student
    await SeedRole(Roles.Student, new()
    {
      (CustomClaimTypes.SitePermission, SitePermissionClaims.AccessTraining),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnProjects),
    });
  }
  
  /// <summary>
  /// Seed an initials set of registration rules (allow and block lists)
  /// using the registration allow/block list config
  /// </summary>
  public async Task SeedRegistrationRules()
  {
    await UpdateRegistrationRulesConfig("Registration:AllowList", _config, false); // allow list
    await UpdateRegistrationRulesConfig("Registration:BlockList", _config, true); // block list
  }
  /// <summary>
  /// Helper function for the SeedRegistrationRules 
  /// </summary>
  /// <param name="key">Config key</param>
  /// <param name="config"></param>
  /// <param name="isBlocked">Values blocked if true or else allowed</param>
  private async Task UpdateRegistrationRulesConfig(string key, IConfiguration config, bool isBlocked)
  {
    var configuredList = config.GetSection(key)?
      .GetChildren()?
      .Select(x => x.Value)?
      .ToList();

    if (configuredList is not null && configuredList.Count >= 1)
    {
      foreach (var value in configuredList)
        if (!string.IsNullOrWhiteSpace(value)) // only add value if not empty
          await _registrationRule.Create(new CreateRegistrationRuleModel(value, isBlocked));

    }
  }
  
  /// <summary>
  /// Seed an initial Instructor user to use for setup if no Instructor users exist. Also add the email to the allow list.
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
    var email = _config["Root:EmailAddress"] ?? "instructor@local.com"; //use 'instructor@local.com' as email if Root:EmailAddress id not configured
    var instructorUsers = await _users.GetUsersInRoleAsync(Roles.Instructor);
    if (!instructorUsers.Any())
    {
      var user = new ApplicationUser
      {
        FullName = "Predefined Instructor",
        Email = email,
        EmailConfirmed = true,
        UserName = email,
      };
      user.PasswordHash = _passwordHasher.HashPassword(user, pwd);
        
      await _users.CreateAsync(user);
      await _users.AddToRoleAsync(user, Roles.Instructor);
      await _registrationRule.Create(new CreateRegistrationRuleModel(email, false)); // also add their email to allow list
    } 
    else 
    {
      var user = await _users.FindByEmailAsync(email); // find the user by email
      if (user is not null && await _users.IsInRoleAsync(user, Roles.Instructor))
      {
        user.PasswordHash = _passwordHasher.HashPassword(user, pwd);
        await _users.UpdateAsync(user);
      }
    }
  }
  
  /// <summary>
  /// Seed an initial project "AI4Green4Students"
  /// </summary>
  public async Task SeedProject()
  {
    var project = new CreateProjectModel("AI4Green");
    await _projects.Create(project);
  }
}
