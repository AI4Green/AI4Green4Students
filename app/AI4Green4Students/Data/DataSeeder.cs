using System.Security.Claims;
using AI4Green4Students.Auth;
using AI4Green4Students.Models;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Identity;

namespace AI4Green4Students.Data;

public class DataSeeder
{
  private readonly RoleManager<IdentityRole> _roles;
  private readonly RegistrationRuleService _registrationRule;
  private readonly IConfiguration _config;

  public DataSeeder(
    RoleManager<IdentityRole> roles,
    RegistrationRuleService registrationRule,
    IConfiguration config)
  {
    _roles = roles;
    _registrationRule = registrationRule;
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
    // TODO populate as claims are made - 

    // Admin
    await SeedRole(Roles.Admin, new()
    {
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ManageUsers),
    });
    
    // Demonstrator
    await SeedRole(Roles.Demonstrator, new()
    {
      // TODO add permissions
    });
    
    
    // Instructor
    await SeedRole(Roles.Instructor, new()
    {
      
    });

    
    // Student
    await SeedRole(Roles.Student, new()
    {
      
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
}
