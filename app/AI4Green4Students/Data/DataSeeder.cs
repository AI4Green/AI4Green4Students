namespace AI4Green4Students.Data;

using System.Security.Claims;
using Auth;
using Constants;
using Entities;
using Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.InputType;
using Models.SectionType;
using Services;

public class DataSeeder
{
  private const string _defaultAdminUsername = "admin";
  private readonly IConfiguration _config;

  private readonly ApplicationDbContext _db;
  private readonly InputTypeService _inputTypes;
  private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
  private readonly RegistrationRuleService _registrationRules;
  private readonly RoleManager<IdentityRole> _roles;
  private readonly SectionTypeService _sectionTypes;
  private readonly UserManager<ApplicationUser> _users;

  public DataSeeder(
    ApplicationDbContext db,
    RoleManager<IdentityRole> roles,
    RegistrationRuleService registrationRules,
    UserManager<ApplicationUser> users,
    IPasswordHasher<ApplicationUser> passwordHasher,
    IConfiguration config,
    InputTypeService inputTypes,
    SectionTypeService sectionTypes)
  {
    _db = db;
    _roles = roles;
    _registrationRules = registrationRules;
    _users = users;
    _passwordHasher = passwordHasher;
    _config = config;
    _inputTypes = inputTypes;
    _sectionTypes = sectionTypes;
  }

  /// <summary>
  /// Seed roles with permissions.
  /// </summary>
  public async Task SeedRoles()
  {
    await SeedRole(Roles.Demonstrator, new List<(string type, string value)>());

    await SeedRole(Roles.Instructor, new List<(string type, string value)>
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
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewProjectExperiments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.LockProjectGroupNotes),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.AdvanceStage),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.MakeComments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.EditOwnComments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.DeleteOwnComments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ApproveFieldResponses)
    });

    await SeedRole(Roles.Student, new List<(string type, string value)>
    {
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnProjects),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewProjectGroupExperiments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnExperiments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.CreateExperiments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.EditOwnExperiments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.DeleteOwnExperiments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.MarkCommentsAsRead),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.AdvanceStage)
    });
  }

  /// <summary>
  /// Seed an initials set of registration rules (allow and blocklists) using the registration allow/blocklist config.
  /// </summary>
  public async Task SeedRegistrationRules()
  {
    await UpdateRegistrationRulesConfig("Registration:AllowList", _config, false);
    await UpdateRegistrationRulesConfig("Registration:BlockList", _config, true);
  }

  /// <summary>
  /// Seeds the input types available to fields
  /// </summary>
  /// <returns></returns>
  public async Task SeedInputTypes()
  {
    var inputList = new List<CreateInputType>
    {
      new CreateInputType(InputTypes.Text),
      new CreateInputType(InputTypes.Description),
      new CreateInputType(InputTypes.Number),
      new CreateInputType(InputTypes.File),
      new CreateInputType(InputTypes.ImageFile),
      new CreateInputType(InputTypes.Multiple),
      new CreateInputType(InputTypes.ReactionScheme),
      new CreateInputType(InputTypes.MultiReactionScheme),
      new CreateInputType(InputTypes.Radio),
      new CreateInputType(InputTypes.Header),
      new CreateInputType(InputTypes.Content),
      new CreateInputType(InputTypes.ChemicalDisposalTable),
      new CreateInputType(InputTypes.ProjectGroupPlanTable),
      new CreateInputType(InputTypes.ProjectGroupHazardTable),
      new CreateInputType(InputTypes.YieldTable),
      new CreateInputType(InputTypes.MultiYieldTable),
      new CreateInputType(InputTypes.GreenMetricsTable),
      new CreateInputType(InputTypes.MultiGreenMetricsTable),
      new CreateInputType(InputTypes.DateAndTime),
      new CreateInputType(InputTypes.SortableList),
      new CreateInputType(InputTypes.FormattedTextInput)
    };

    foreach (var inputType in inputList)
    {
      await _inputTypes.Create(inputType);
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
      new CreateSectionTypeModel(SectionTypes.Note),
      new CreateSectionTypeModel(SectionTypes.Report),
      new CreateSectionTypeModel(SectionTypes.ProjectGroup),
      new CreateSectionTypeModel(SectionTypes.LiteratureReview)
    };

    foreach (var sectionType in list)
    {
      await _sectionTypes.Create(sectionType);
    }
  }

  /// <summary>
  /// Seeds the stage types. E.g. Literature Review, Plan, Note, Report.
  /// </summary>
  public async Task SeedStageType()
  {
    var existing = await _db.StageTypes.AsNoTracking().Select(x => x.Value).ToListAsync();

    var stages = new List<string>
    {
      SectionTypes.LiteratureReview,
      SectionTypes.Plan,
      SectionTypes.Note,
      SectionTypes.Report
    };

    var newStages = stages.Except(existing).Select(value => new StageType
    {
      Value = value
    }).ToList();

    if (newStages.Count != 0)
    {
      _db.StageTypes.AddRange(newStages);
      await _db.SaveChangesAsync();
    }
  }

  /// <summary>
  /// Seeds the stages.
  /// </summary>
  public async Task SeedStage()
  {
    var types = (await _db.StageTypes.ToListAsync()).ToDictionary(x => x.Value);
    var existingStages = await _db.Stages.Include(x => x.Type).ToListAsync();

    var stageConfigs = new Dictionary<string, List<StageConfigModel>>
    {
      [SectionTypes.LiteratureReview] = new List<StageConfigModel>
      {
        new StageConfigModel(1, Stages.Draft),
        new StageConfigModel(2, Stages.InReview),
        new StageConfigModel(3, Stages.AwaitingChanges, Stages.InReview),
        new StageConfigModel(99, Stages.Approved)
      },
      [SectionTypes.Plan] = new List<StageConfigModel>
      {
        new StageConfigModel(1, Stages.Draft),
        new StageConfigModel(2, Stages.InReview),
        new StageConfigModel(3, Stages.AwaitingChanges, Stages.InReview),
        new StageConfigModel(99, Stages.Approved)
      },
      [SectionTypes.Note] = new List<StageConfigModel>
      {
        new StageConfigModel(1, Stages.Draft),
        new StageConfigModel(2, Stages.InProgress),
        new StageConfigModel(5, Stages.FeedbackRequested),
        new StageConfigModel(10, Stages.InProgressPostFeedback),
        new StageConfigModel(95, Stages.Locked)
      },
      [SectionTypes.Report] = new List<StageConfigModel>
      {
        new StageConfigModel(1, Stages.Draft), new StageConfigModel(5, Stages.Submitted)
      }
    };

    var stagesToAdd = new List<Stage>();
    var stagesToUpdate = new List<Stage>();

    // create or update stages including sort order
    foreach (var (type, configs) in stageConfigs)
    {
      if (!types.TryGetValue(type, out var sectionType))
      {
        continue;
      }

      var existing = existingStages.Where(x => x.Type.Id == sectionType.Id).ToList();

      foreach (var config in configs)
      {
        var stage = existing.FirstOrDefault(x => x.DisplayName == config.DisplayName);
        if (stage is not null)
        {
          if (stage.SortOrder == config.SortOrder)
          {
            continue;
          }
          stage.SortOrder = config.SortOrder;
          stagesToUpdate.Add(stage);
        }
        else
        {
          stagesToAdd.Add(new Stage
          {
            SortOrder = config.SortOrder,
            DisplayName = config.DisplayName,
            Value = config.DisplayName,
            Type = sectionType
          });
        }
      }
    }

    if (stagesToAdd.Count > 0)
    {
      _db.AddRange(stagesToAdd);
    }

    if (stagesToUpdate.Count > 0)
    {
      _db.UpdateRange(stagesToUpdate);
    }

    await _db.SaveChangesAsync();

    // update next stage property
    var allStages = await _db.Stages.Include(x => x.Type).ToListAsync();
    var nextStageUpdates = new List<Stage>();

    foreach (var (type, configs) in stageConfigs)
    {
      if (!types.TryGetValue(type, out var sectionType))
      {
        continue;
      }

      foreach (var config in configs.Where(x => !string.IsNullOrEmpty(x.NextStageName)))
      {
        var currentStage =
          allStages.FirstOrDefault(x => x.Type.Id == sectionType.Id && x.DisplayName == config.DisplayName);
        var nextStage =
          allStages.FirstOrDefault(x => x.Type.Id == sectionType.Id && x.DisplayName == config.NextStageName);

        if (currentStage is null || nextStage is null || currentStage.NextStage?.Id == nextStage.Id)
        {
          continue;
        }
        currentStage.NextStage = nextStage;
        nextStageUpdates.Add(currentStage);
      }
    }

    if (nextStageUpdates.Count > 0)
    {
      _db.UpdateRange(nextStageUpdates);
      await _db.SaveChangesAsync();
    }
  }

  /// <summary>`
  /// Seeds the stage permissions
  /// </summary>
  /// <returns></returns>
  public async Task SeedStagePermission()
  {
    // Get all stage types in one query
    var stageTypes = await _db.StageTypes.ToListAsync();
    var stageTypeLookup = stageTypes.ToDictionary(x => x.Value);

    // Define permission configurations declaratively
    var permissionConfigs = new Dictionary<string, List<StagePermissionConfigModel>>
    {
      [SectionTypes.LiteratureReview] = new List<StagePermissionConfigModel>
      {
        new StagePermissionConfigModel(1, 1, StagePermissions.OwnerCanEdit),
        new StagePermissionConfigModel(3, 3, StagePermissions.OwnerCanEditCommented),
        new StagePermissionConfigModel(2, 99, StagePermissions.InstructorCanView),
        new StagePermissionConfigModel(2, 2, StagePermissions.InstructorCanComment)
      },

      [SectionTypes.Plan] = new List<StagePermissionConfigModel>
      {
        new StagePermissionConfigModel(1, 1, StagePermissions.OwnerCanEdit),
        new StagePermissionConfigModel(3, 3, StagePermissions.OwnerCanEditCommented),
        new StagePermissionConfigModel(2, 99, StagePermissions.InstructorCanView),
        new StagePermissionConfigModel(2, 2, StagePermissions.InstructorCanComment)
      },

      [SectionTypes.Note] = new List<StagePermissionConfigModel>
      {
        new StagePermissionConfigModel(2, 10, StagePermissions.OwnerCanEdit),
        new StagePermissionConfigModel(2, 95, StagePermissions.InstructorCanView)
      },

      [SectionTypes.Report] = new List<StagePermissionConfigModel>
      {
        new StagePermissionConfigModel(1, 1, StagePermissions.OwnerCanEdit),
        new StagePermissionConfigModel(5, 5, StagePermissions.InstructorCanView)
      }
    };

    // Get existing permissions in one query
    var existingPermissions = await _db.StagePermissions
      .AsNoTracking()
      .Include(x => x.Type)
      .ToListAsync();

    var permissionsToAdd = new List<StagePermission>();
    var permissionsToUpdate = new List<StagePermission>();

    foreach (var (sectionTypeValue, configs) in permissionConfigs)
    {
      if (!stageTypeLookup.TryGetValue(sectionTypeValue, out var stageType))
      {
        continue;
      }

      foreach (var config in configs)
      {
        var existingPermission = existingPermissions.FirstOrDefault(x =>
          x.Type.Value == sectionTypeValue && x.Key == config.PermissionKey);

        if (existingPermission == null)
        {
          // Create new permission
          permissionsToAdd.Add(new StagePermission
          {
            MinStageSortOrder = config.MinStageSortOrder,
            MaxStageSortOrder = config.MaxStageSortOrder,
            Type = stageType,
            Key = config.PermissionKey
          });
        }
        else
        {
          permissionsToUpdate.Add(new StagePermission
          {
            Id = existingPermission.Id,
            MinStageSortOrder = config.MinStageSortOrder,
            MaxStageSortOrder = config.MaxStageSortOrder,
            Type = stageType,
            Key = config.PermissionKey
          });
        }
      }
    }

    // Batch operations
    if (permissionsToAdd.Count > 0)
    {
      _db.AddRange(permissionsToAdd);
    }

    if (permissionsToUpdate.Count > 0)
    {
      _db.UpdateRange(permissionsToUpdate);
    }

    await _db.SaveChangesAsync();
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

    // prep username
    var configuredUsername = _config["Root:Username"];
    var username = string.IsNullOrWhiteSpace(configuredUsername) ? _defaultAdminUsername : configuredUsername;
    username = $"@{username}"; // Prefix the username to show it's not an email
    // Add the user if they don't exist, else update them,
    var superAdmin = await _users.FindByEmailAsync(SuperUser.EmailAddress);
    if (superAdmin is null)
    {
      var user = new ApplicationUser
      {
        FullName = "Super Admin",
        UserName = username,
        Email = SuperUser.EmailAddress,
        EmailConfirmed = true
      };
      user.PasswordHash = _passwordHasher.HashPassword(user, pwd);

      await _users.CreateAsync(user);
      await _users.AddToRoleAsync(user, Roles.Instructor);
      await _registrationRules.Create(new CreateRegistrationRuleModel(SuperUser.EmailAddress,
        false)); // also add their email to allow list
    }
    else
    {
      // update username / password
      superAdmin.UserName = username;
      superAdmin.PasswordHash = _passwordHasher.HashPassword(superAdmin, pwd);
      await _users.UpdateAsync(superAdmin);
    }
  }

  /// <summary>
  /// Ensure an individual role exists and has the specified claims.
  /// </summary>
  /// <param name="roleName">The name of the role to ensure is present.</param>
  /// <param name="claims">The claims the role should have.</param>
  /// <returns></returns>
  private async Task SeedRole(string roleName, List<(string type, string value)> claims)
  {
    var role = await _roles.FindByNameAsync(roleName);

    if (role is null)
    {
      role = new IdentityRole
      {
        Name = roleName
      };
      await _roles.CreateAsync(role);
    }

    var existingClaims = (await _roles.GetClaimsAsync(role)).ToDictionary(x => $"{x.Type}{x.Value}");
    foreach (var (type, value) in claims)
    {
      if (!existingClaims.ContainsKey($"{type}{value}"))
      {
        await _roles.AddClaimAsync(role, new Claim(type, value));
      }
    }
  }

  /// <summary>
  /// Helper function for the SeedRegistrationRules
  /// </summary>
  /// <param name="key">Config key</param>
  /// <param name="config"></param>
  /// <param name="isBlocked">Values blocked if true or else allowed</param>
  private async Task UpdateRegistrationRulesConfig(string key, IConfiguration config, bool isBlocked)
  {
    var configuredList = config.GetSection(key)
      .GetChildren()
      .Select(x => x.Value)
      .ToList();

    if (configuredList.Count >= 1)
    {
      foreach (var value in configuredList)
      {
        if (!string.IsNullOrWhiteSpace(value)) // only add value if not empty
        {
          await _registrationRules.Create(new CreateRegistrationRuleModel(value, isBlocked));
        }
      }
    }
  }

  /// <summary>
  /// Stage config model.
  /// </summary>
  private record StageConfigModel(int SortOrder, string DisplayName, string? NextStageName = null);

  /// <summary>
  /// Stage permission config model.
  /// </summary>
  private record StagePermissionConfigModel(int MinStageSortOrder, int MaxStageSortOrder, string PermissionKey);
}
