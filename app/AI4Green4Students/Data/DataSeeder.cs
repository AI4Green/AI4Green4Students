using System.Security.Claims;
using AI4Green4Students.Auth;
using AI4Green4Students.Constants;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models;
using AI4Green4Students.Models.InputType;
using AI4Green4Students.Models.Section;
using AI4Green4Students.Models.SectionType;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Data;

public class DataSeeder
{
  private readonly ApplicationDbContext _db;
  private readonly RoleManager<IdentityRole> _roles;
  private readonly RegistrationRuleService _registrationRule;
  private readonly UserManager<ApplicationUser> _users;
  private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
  private readonly IConfiguration _config;
  private readonly InputTypeService _inputTypeService;
  private readonly SectionTypeService _sectionTypeService;

  public DataSeeder(
    ApplicationDbContext db,
    RoleManager<IdentityRole> roles,
    RegistrationRuleService registrationRule,
    UserManager<ApplicationUser> users,
    IPasswordHasher<ApplicationUser> passwordHasher,
    IConfiguration config,
    InputTypeService inputTypeService,
    SectionTypeService sectionTypeService)
  {
    _db = db;
    _roles = roles;
    _registrationRule = registrationRule;
    _users = users;
    _passwordHasher = passwordHasher;
    _config = config;
    _inputTypeService = inputTypeService;
    _sectionTypeService = sectionTypeService;
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
      (CustomClaimTypes.SitePermission, SitePermissionClaims.InviteInstructors),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.InviteStudents),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.InviteUsers),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.EditUsers),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.DeleteUsers),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllUsers),

      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewRoles),

      (CustomClaimTypes.SitePermission, SitePermissionClaims.CreateRegistrationRules),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.EditRegistrationRules),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.DeleteRegistrationRules),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewRegistrationRules),

      (CustomClaimTypes.SitePermission, SitePermissionClaims.CreateProjects),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.EditProjects),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.DeleteProjects),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnProjects),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllProjects),

      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments),

      (CustomClaimTypes.SitePermission, SitePermissionClaims.MakeComments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.EditOwnComments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.DeleteOwnComments)
    });

    // Student
    await SeedRole(Roles.Student, new()
    {
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnProjects),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnExperiments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.CreateExperiments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.EditOwnExperiments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.DeleteOwnExperiments),
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
  /// Seeds the input types available to fields
  /// </summary>
  /// <returns></returns>
  public async Task SeedInputTypes()
  {
    var inputList = new List<CreateInputType>
    {
      new CreateInputType() { Name = InputTypes.Text },
      new CreateInputType() { Name = InputTypes.Description },
      new CreateInputType() { Name = InputTypes.Number },
      new CreateInputType() { Name = InputTypes.File },
      new CreateInputType() { Name = InputTypes.Multiple },
      new CreateInputType() { Name = InputTypes.ReactionScheme },
      new CreateInputType() { Name = InputTypes.Radio },
      new CreateInputType() { Name = InputTypes.Header },
      new CreateInputType() { Name = InputTypes.Content },
      new CreateInputType() { Name = InputTypes.SubstanceTable },
      new CreateInputType() { Name = InputTypes.ChemicalDisposalTable },
    };

    foreach (var inputType in inputList)
    {
      await _inputTypeService.Create(inputType);
    }
  }

  /// <summary>
  /// Seeds the section types
  /// </summary>
  /// <returns></returns>
  public async Task SeedSectionTypes()
  {
    var list = new List<CreateSectionTypeModel>
    {
      new CreateSectionTypeModel(SectionTypes.Plan),
      new CreateSectionTypeModel(SectionTypes.Report),
      new CreateSectionTypeModel(SectionTypes.ProjectGroup),
    };

    foreach (var sectionType in list)
      await _sectionTypeService.Create(sectionType);
  }

  /// <summary>
  /// Seeds the stage types
  /// </summary>
  /// <returns></returns>
  public async Task SeedStageType()
  {
    // only seed an empty table
    if (!await _db.StageTypes
          .AsNoTracking()
          .AnyAsync())
    {
      var seedStages = new List<StageType>
      {
        new StageType { Value = StageTypes.Plan },
        new StageType { Value = StageTypes.Report }
      };

      foreach (var s in seedStages)
        _db.Add(s);

      await _db.SaveChangesAsync();
    }
  }

  /// <summary>
  /// Seeds the stages
  /// </summary>
  /// <returns></returns>
  public async Task SeedStage()
  {
    var types = await _db.StageTypes.ToListAsync();
    var existingStages = await _db.Stages
      .Include(x => x.Type)
      .ToListAsync();

    // Seed stages if there aren't any
    var planType = types.SingleOrDefault(x => x.Value == StageTypes.Plan);
    var report = types.SingleOrDefault(x => x.Value == StageTypes.Report);
    if (planType is not null && !existingStages.Any(x => x.Type == planType))
    {
      var seedStages = new List<Stage>
      {
        new Stage { SortOrder = 1, DisplayName = ReportStages.Draft, Type = planType },

        new Stage { SortOrder = 5, DisplayName = ReportStages.InReview, Type = planType },

        new Stage { SortOrder = 3, DisplayName = ReportStages.AwaitingChanges, Type = planType },
        
        new Stage { SortOrder = 99, DisplayName = ReportStages.Approved, Type = planType },
      };

      foreach (var s in seedStages)
      {
        _db.Add(s);
      }
    }
    if (report is not null && !existingStages.Any(x => x.Type == report))
    {
      var seedStages = new List<Stage>
      {
        new Stage { SortOrder = 1, DisplayName = ReportStages.Draft, Type = report },

        new Stage { SortOrder = 5, DisplayName = ReportStages.InReview, Type = report },

        new Stage { SortOrder = 3, DisplayName = ReportStages.AwaitingChanges, Type = report },
        
        new Stage { SortOrder = 99, DisplayName = ReportStages.Approved, Type = report },

      };

      foreach (var s in seedStages)
      {
        _db.Add(s);
      }
    }

    await _db.SaveChangesAsync();
  }
  
  /// <summary>
  /// Seeds the stage permissions
  /// </summary>
  /// <returns></returns>
  public async Task SeedStagePermission()
  {

    var PlanStage = await _db.StageTypes
                          .Where(x => x.Value == StageTypes.Plan)
                          .SingleOrDefaultAsync() 
                        ?? throw new KeyNotFoundException();

    var ReportStage = await _db.StageTypes
                          .Where(x => x.Value == StageTypes.Report)
                          .SingleOrDefaultAsync()
                        ?? throw new KeyNotFoundException();

    // only seed an empty table
    if (!await _db.StagePermissions
          .AsNoTracking()
          .AnyAsync())
    {
      var seedPermissions = new List<StagePermission>
      {
        new StagePermission { MinStageSortOrder = 1, MaxStageSortOrder = 1, Type = PlanStage, Key = StagePermissions.OwnerCanEdit  },
        new StagePermission { MinStageSortOrder = 1, MaxStageSortOrder = 1, Type = ReportStage, Key = StagePermissions.OwnerCanEdit  },
        
        new StagePermission { MinStageSortOrder = 3, MaxStageSortOrder = 3, Type = PlanStage, Key = StagePermissions.OwnerCanEditCommented  },
        new StagePermission { MinStageSortOrder = 3, MaxStageSortOrder = 3, Type = ReportStage, Key = StagePermissions.OwnerCanEditCommented  },
        
        new StagePermission { MinStageSortOrder = 5, MaxStageSortOrder = 5, Type = PlanStage, Key = StagePermissions.InstructorCanView  }, 
        new StagePermission { MinStageSortOrder = 5, MaxStageSortOrder = 5, Type = ReportStage, Key = StagePermissions.InstructorCanView  },

        new StagePermission { MinStageSortOrder = 5, MaxStageSortOrder = 5, Type = PlanStage, Key = StagePermissions.InstructorCanComment  },
        new StagePermission { MinStageSortOrder = 5, MaxStageSortOrder = 5, Type = ReportStage, Key = StagePermissions.InstructorCanComment  },
      };

      foreach (var s in seedPermissions)
      {
        _db.Add(s);

      }
      await _db.SaveChangesAsync();
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
    var email = _config["Root:EmailAddress"] ??
                "instructor@local.com"; //use 'instructor@local.com' as email if Root:EmailAddress id not configured
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
      await _registrationRule.Create(new CreateRegistrationRuleModel(email,
        false)); // also add their email to allow list
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
}
