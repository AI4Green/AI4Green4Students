using System.Security.Claims;
using AI4Green4Students.Auth;
using AI4Green4Students.Constants;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models;
using AI4Green4Students.Models.InputType;
using AI4Green4Students.Models.SectionType;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Data;

public class DataSeeder
{
  private const string _defaultAdminUsername = "admin";

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

      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewProjectExperiments),

      (CustomClaimTypes.SitePermission, SitePermissionClaims.LockProjectGroupNotes),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.AdvanceStage),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.MakeComments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.EditOwnComments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.DeleteOwnComments),

      (CustomClaimTypes.SitePermission, SitePermissionClaims.ApproveFieldResponses)
    });

    // Student
    await SeedRole(Roles.Student, new()
    {
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnProjects),

      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewProjectGroupExperiments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnExperiments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.CreateExperiments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.EditOwnExperiments),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.DeleteOwnExperiments),


      (CustomClaimTypes.SitePermission, SitePermissionClaims.MarkCommentsAsRead),
      (CustomClaimTypes.SitePermission, SitePermissionClaims.AdvanceStage),
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
      new CreateInputType() { Name = InputTypes.ImageFile },
      new CreateInputType() { Name = InputTypes.Multiple },
      new CreateInputType() { Name = InputTypes.ReactionScheme },
      new CreateInputType() { Name = InputTypes.MultiReactionScheme },
      new CreateInputType() { Name = InputTypes.Radio },
      new CreateInputType() { Name = InputTypes.Header },
      new CreateInputType() { Name = InputTypes.Content },
      new CreateInputType() { Name = InputTypes.ChemicalDisposalTable },
      new CreateInputType() { Name = InputTypes.ProjectGroupPlanTable },
      new CreateInputType() { Name = InputTypes.ProjectGroupHazardTable },
      new CreateInputType() { Name = InputTypes.YieldTable },
      new CreateInputType() { Name = InputTypes.MultiYieldTable },
      new CreateInputType() { Name = InputTypes.GreenMetricsTable },
      new CreateInputType() { Name = InputTypes.MultiGreenMetricsTable},
      new CreateInputType() { Name = InputTypes.DateAndTime },
      new CreateInputType() { Name = InputTypes.SortableList},
      new CreateInputType() { Name = InputTypes.FormattedTextInput},
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
      new CreateSectionTypeModel(SectionTypes.Note),
      new CreateSectionTypeModel(SectionTypes.Report),
      new CreateSectionTypeModel(SectionTypes.ProjectGroup),
      new CreateSectionTypeModel(SectionTypes.LiteratureReview)
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
    var existingStageTypes = await _db.StageTypes.AsNoTracking().Select(x => x.Value).ToListAsync();

    var stageTypes = new List<string>
    {
      SectionTypes.LiteratureReview,
      SectionTypes.Plan,
      SectionTypes.Note,
      SectionTypes.Report
    };

    var missingStageTypes = stageTypes.Except(existingStageTypes).Select(value => new StageType { Value = value }).ToList();

    if (missingStageTypes.Any())
    {
      _db.StageTypes.AddRange(missingStageTypes);
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

    var literatureReview = types.SingleOrDefault(x => x.Value == SectionTypes.LiteratureReview);
    var plan = types.SingleOrDefault(x => x.Value == SectionTypes.Plan);
    var note = types.SingleOrDefault(x => x.Value == SectionTypes.Note);
    var report = types.SingleOrDefault(x => x.Value == SectionTypes.Report);

    if (literatureReview is not null)
    {
      var existingLrStages = existingStages.Where(x => x.Type == literatureReview).ToList();
      var draftStage = new Stage { SortOrder = 1, DisplayName = Stages.Draft, Type = literatureReview };
      var inReviewStage = new Stage { SortOrder = 2, DisplayName = Stages.InReview, Type = literatureReview };
      var awaitingChangesStage = new Stage { SortOrder = 3, DisplayName = Stages.AwaitingChanges, Type = literatureReview };
      var approvedStage = new Stage { SortOrder = 99, DisplayName = Stages.Approved, Type = literatureReview };

      awaitingChangesStage.NextStage = inReviewStage; // set AwaitingChanges next stage to InReview

      var seedStages = new List<Stage> { draftStage, inReviewStage, awaitingChangesStage, approvedStage };

      foreach (var s in seedStages)
      {
        if (existingLrStages.Any(x => x.DisplayName == s.DisplayName)) continue;
        _db.Add(s);
      }
    }

    if (plan is not null)
    {
      var existingPlanStages = existingStages.Where(x => x.Type == plan).ToList();
      var draftStage = new Stage { SortOrder = 1, DisplayName = Stages.Draft, Type = plan };
      var inReviewStage = new Stage { SortOrder = 2, DisplayName = Stages.InReview, Type = plan };
      var awaitingChangesStage = new Stage { SortOrder = 3, DisplayName = Stages.AwaitingChanges, Type = plan };
      var approvedStage = new Stage { SortOrder = 99, DisplayName = Stages.Approved, Type = plan };

      awaitingChangesStage.NextStage = inReviewStage;

      var seedStages = new List<Stage> { draftStage, inReviewStage, awaitingChangesStage, approvedStage };

      foreach (var s in seedStages)
      {
        if (existingPlanStages.Any(x => x.DisplayName == s.DisplayName)) continue;
        _db.Add(s);
      }
    }

    if (note is not null)
    {
      var existingNoteStages = existingStages.Where(x => x.Type == note).ToList();
      var draftStage = new Stage { SortOrder = 1, DisplayName = Stages.Draft, Type = note };
      var submitted = new Stage { SortOrder = 95, DisplayName = Stages.Locked, Type = note };

      var seedStages = new List<Stage> { draftStage, submitted };

      foreach (var s in seedStages)
      {
        if (existingNoteStages.Any(x => x.DisplayName == s.DisplayName)) continue;
        _db.Add(s);
      }
    }

    if (report is not null)
    {
      var existingReportStages = existingStages.Where(x => x.Type == report).ToList();
      var draftStage = new Stage { SortOrder = 1, DisplayName = Stages.Draft, Type = report };
      var submitted = new Stage { SortOrder = 5, DisplayName = Stages.Submitted, Type = report };

      var seedStages = new List<Stage> { draftStage, submitted };

      foreach (var s in seedStages)
      {
        if (existingReportStages.Any(x => x.DisplayName == s.DisplayName)) continue;
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
    var LiteratureReviewStage = await _db.StageTypes
                          .Where(x => x.Value == SectionTypes.LiteratureReview)
                          .SingleOrDefaultAsync()
                        ?? throw new KeyNotFoundException();

    var PlanStage = await _db.StageTypes
                          .Where(x => x.Value == SectionTypes.Plan)
                          .SingleOrDefaultAsync()
                        ?? throw new KeyNotFoundException();

    var NoteStage = await _db.StageTypes
                      .Where(x => x.Value == SectionTypes.Note)
                      .SingleOrDefaultAsync()
                    ?? throw new KeyNotFoundException();

    var ReportStage = await _db.StageTypes
                          .Where(x => x.Value == SectionTypes.Report)
                          .SingleOrDefaultAsync()
                        ?? throw new KeyNotFoundException();

    var seedPermissions = new List<StagePermission>
    {
      new StagePermission { MinStageSortOrder = 1, MaxStageSortOrder = 1, Type = LiteratureReviewStage, Key = StagePermissions.OwnerCanEdit  },
      new StagePermission { MinStageSortOrder = 1, MaxStageSortOrder = 1, Type = PlanStage, Key = StagePermissions.OwnerCanEdit  },
      new StagePermission { MinStageSortOrder = 1, MaxStageSortOrder = 1, Type = NoteStage, Key = StagePermissions.OwnerCanEdit  },
      new StagePermission { MinStageSortOrder = 1, MaxStageSortOrder = 1, Type = ReportStage, Key = StagePermissions.OwnerCanEdit  },

      new StagePermission { MinStageSortOrder = 3, MaxStageSortOrder = 3, Type = LiteratureReviewStage, Key = StagePermissions.OwnerCanEditCommented  },
      new StagePermission { MinStageSortOrder = 3, MaxStageSortOrder = 3, Type = PlanStage, Key = StagePermissions.OwnerCanEditCommented  },

      new StagePermission { MinStageSortOrder = 2, MaxStageSortOrder = 99, Type = LiteratureReviewStage, Key = StagePermissions.InstructorCanView  },
      new StagePermission { MinStageSortOrder = 2, MaxStageSortOrder = 99, Type = PlanStage, Key = StagePermissions.InstructorCanView  },
      new StagePermission { MinStageSortOrder = 1, MaxStageSortOrder = 95, Type = NoteStage, Key = StagePermissions.InstructorCanView  },
      new StagePermission { MinStageSortOrder = 5, MaxStageSortOrder = 5, Type = ReportStage, Key = StagePermissions.InstructorCanView  },

      new StagePermission { MinStageSortOrder = 2, MaxStageSortOrder = 2, Type = LiteratureReviewStage, Key = StagePermissions.InstructorCanComment  },
      new StagePermission { MinStageSortOrder = 2, MaxStageSortOrder = 2, Type = PlanStage, Key = StagePermissions.InstructorCanComment  },
    };

    var existingStagePermissions = await _db.StagePermissions.AsNoTracking().Include(x => x.Type).ToListAsync();

    foreach (var seedPermission in seedPermissions)
    {
        var existingPermission = existingStagePermissions.SingleOrDefault(x => x.Type.Value == seedPermission.Type.Value && x.Key == seedPermission.Key);

        if (existingPermission is null)
        {
            _db.Add(seedPermission);
        }
        else
        {
          var permissionToUpdate = await _db.StagePermissions.SingleOrDefaultAsync(x => x.Id == existingPermission.Id);

          if (permissionToUpdate is null) continue;
          permissionToUpdate.MinStageSortOrder = seedPermission.MinStageSortOrder;
          permissionToUpdate.MaxStageSortOrder = seedPermission.MaxStageSortOrder;
          _db.Update(permissionToUpdate);
        }
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
      await _registrationRule.Create(new CreateRegistrationRuleModel(SuperUser.EmailAddress, false)); // also add their email to allow list
    }
    else
    {
      // update username / password
      superAdmin.UserName = username;
      superAdmin.PasswordHash = _passwordHasher.HashPassword(superAdmin, pwd);
      await _users.UpdateAsync(superAdmin);
    }
  }
}
