using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using AI4Green4Students.Auth;
using Microsoft.EntityFrameworkCore;
using AI4Green4Students.Constants;
using AI4Green4Students.Data.Entities.SectionTypeData;
using System.Text.Json;

namespace AI4Green4Students.Tests;

public class DataSeeder
{
  private readonly ApplicationDbContext _db;

  public DataSeeder(ApplicationDbContext db)
  {
    _db = db;
  }

  #region# Registration Rules

  /*
//seed a bunch of new rules to cover all the scenarios:
//blocked mail.com domain
//allowed specific mail.com email
//blocked specific gmail email
//allowed gmail domain
*/
  public async Task SeedDefaultRules()
  {
    var seedRegistrationRules = new List<RegistrationRule>
    {
      new RegistrationRule()
      {
        Value = StringConstants.GmailDomain,
        IsBlocked = false
      },
      new RegistrationRule()
      {
        Value = StringConstants.MailDomain,
        IsBlocked = true
      },
      new RegistrationRule()
      {
        Value = StringConstants.AllowedMailEmail,
        IsBlocked = false
      },
      new RegistrationRule()
      {
        Value = StringConstants.BlockedGmailEmail,
        IsBlocked = true
      }
    };

    foreach (var s in seedRegistrationRules)
    {
      _db.Add(s);
    }

    await _db.SaveChangesAsync();
  }

  /*
   * Seed the global blocker, but allow all gmail domain and a single mail.com to register
   */
  public async Task SeedGlobalBlock()
  {
    var seedRegistrationRules = new List<RegistrationRule>
    {
      new RegistrationRule()
      {
        Value = StringConstants.Wildcard,
        IsBlocked = true
      },
      new RegistrationRule()
      {
        Value = StringConstants.GmailDomain,
        IsBlocked = false
      }
    };

    foreach (var s in seedRegistrationRules)
    {
      _db.Add(s);
    }

    await _db.SaveChangesAsync();
  }

  #endregion#

  public async Task SeedDefaultTestExperiment()
  {
    await SeedDefaultProject();
    await SeedDefaultProjectGroup();
    await SeedDefaultUsers();
    await SeedStudentProjectGroup();
    await SeedDefaultSectionAndInputType();
    await SeedDefaultSections();
    await SeedDefaultFields();
    await SeedDefaultStages();
    await SeedDefaultPlans();
    await SeedDefaultFieldResponses();
  }

  public async Task SeedDefaultProject()
  {
    var projectOne = new Project { Name = StringConstants.FirstProject };

    _db.Add(projectOne);
    await _db.SaveChangesAsync();
  }

  public async Task SeedDefaultProjectGroup()
  {
    var projects = await _db.Projects.ToListAsync();
    var projectOne = projects.Single(x => x.Name == StringConstants.FirstProject);

    //create a project group for the first project
    var projectGroupOne = new ProjectGroup
    {
      Name = StringConstants.FirstProjectGroup,
      Project = projectOne
    };

    _db.Add(projectGroupOne);
    await _db.SaveChangesAsync();
  }

  public async Task SeedDefaultUsers()
  {
    var roles = new List<IdentityRole> { new(Roles.Student), new(Roles.Instructor) };
    foreach (var role in roles) _db.Add(role);

    var instructorRole = roles.Single(x => x.Name == Roles.Instructor);
    var studentRole = roles.Single(x => x.Name == Roles.Student);

    var users = new List<ApplicationUser>
    {
      new ApplicationUser { FullName = StringConstants.StudentUserOne },
      new ApplicationUser { FullName = StringConstants.StudentUserTwo },
      new ApplicationUser { FullName = StringConstants.InstructorUser }
    };
    foreach (var user in users) _db.Add(user);

    var studentOne = users.Single(x => x.FullName == StringConstants.StudentUserOne);
    var studentTwo = users.Single(x => x.FullName == StringConstants.StudentUserTwo);
    var instructor = users.Single(x => x.FullName == StringConstants.InstructorUser);

    // add roles to users
    var userRoles = new List<IdentityUserRole<string>>
    {
      new IdentityUserRole<string> { RoleId = studentRole.Id, UserId = studentOne.Id },
      new IdentityUserRole<string> { RoleId = studentRole.Id, UserId = studentTwo.Id },
      new IdentityUserRole<string> { RoleId = instructorRole.Id, UserId = instructor.Id }
    };
    foreach (var userRole in userRoles) _db.Add(userRole);

    await _db.SaveChangesAsync();
  }

  public async Task SeedStudentProjectGroup()
  {
    var projects = await _db.Projects.ToListAsync();
    var pgs = await _db.ProjectGroups.Include(pg => pg.Project).ToListAsync();
    var users = await _db.Users.ToListAsync();

    // Students
    var studentOne = users.Single(x => x.FullName == StringConstants.StudentUserOne);
    var studentTwo = users.Single(x => x.FullName == StringConstants.StudentUserTwo);

    // Project
    var projectOne = projects.Single(x => x.Name == StringConstants.FirstProject);

    // Set the students for the project group
    var projectGroupOne = pgs.Single(x => x.Name == StringConstants.FirstProjectGroup && x.Project.Id == projectOne.Id);
    projectGroupOne.Students = [studentOne, studentTwo];

    await _db.SaveChangesAsync();
  }

  public async Task SeedDefaultSectionAndInputType()
  {
    var sectionTypes = new List<SectionType>
    {
      new SectionType { Name = SectionTypes.Plan },
      new SectionType { Name = SectionTypes.Report },
      new SectionType { Name = SectionTypes.ProjectGroup },
      new SectionType { Name = SectionTypes.LiteratureReview },
      new SectionType { Name = SectionTypes.Note }
    };
    foreach (var sectionType in sectionTypes) _db.Add(sectionType);

    var inputTypes = new List<InputType>
    {
      new InputType { Name = InputTypes.Text },
      new InputType { Name = InputTypes.Description },
      new InputType { Name = InputTypes.Number },
    };
    foreach (var inputType in inputTypes) _db.Add(inputType);

    await _db.SaveChangesAsync();
  }

  public async Task SeedDefaultSections()
  {
    var projects = await _db.Projects.ToListAsync();
    var sectionTypes = await _db.SectionTypes.ToListAsync();

    var projectOne = projects.Single(x => x.Name == StringConstants.FirstProject);

    var sectionTypePlan = sectionTypes.Single(x => x.Name == SectionTypes.Plan);
    var planSections = new List<Section>
    {
      new Section { Name = StringConstants.PlanFirstSection, SortOrder = 1, Project = projectOne, SectionType = sectionTypePlan },
      new Section { Name = StringConstants.PlanSecondSection, SortOrder = 2 , Project = projectOne, SectionType = sectionTypePlan },
    };
    foreach (var section in planSections) _db.Add(section);

    var sectionTypeReport = sectionTypes.Single(x => x.Name == SectionTypes.Report);
    var reportSections = new List<Section>
    {
      new Section { Name = StringConstants.ReportFirstSection, SortOrder = 1, Project = projectOne, SectionType = sectionTypeReport },
    };
    foreach (var section in reportSections) _db.Add(section);

    var sectionTypeNote = sectionTypes.Single(x => x.Name == SectionTypes.Note);
    var noteSections = new List<Section>
    {
      new Section { Name = StringConstants.NoteFirstSection, SortOrder = 1, Project = projectOne, SectionType = sectionTypeNote },
      new Section { Name = StringConstants.NoteSecondSection, SortOrder = 2, Project = projectOne, SectionType = sectionTypeNote }
    };
    foreach (var section in noteSections) _db.Add(section);

    await _db.SaveChangesAsync();
  }

  public async Task SeedDefaultFields()
  {
    var projects = await _db.Projects.ToListAsync();
    var sections = await _db.Sections.Include(section => section.SectionType).ToListAsync();

    var inputTypes = await _db.InputTypes.ToListAsync();
    var textInput = inputTypes.Single(x => x.Name == InputTypes.Text);
    var numberInput = inputTypes.Single(x => x.Name == InputTypes.Number);
    var description = inputTypes.Single(x => x.Name == InputTypes.Description);

    var projectOne = projects.Single(x => x.Name == StringConstants.FirstProject);

    var planSections = sections.Where(x => x.SectionType.Name == SectionTypes.Plan && x.Project.Id == projectOne.Id).ToList();
    var reportSections = sections.Where(x => x.SectionType.Name == SectionTypes.Report && x.Project.Id == projectOne.Id).ToList();
    var noteSections = sections.Where(x => x.SectionType.Name == SectionTypes.Note && x.Project.Id == projectOne.Id).ToList();

    // plans sections fields
    var planFirstSection = planSections.Single(x => x.Name == StringConstants.PlanFirstSection);
    var planSecondSection = planSections.Single(x => x.Name == StringConstants.PlanSecondSection);

    var planSectionsFields = new List<Field>
    {
      // plan first section fields
      new Field { Name = StringConstants.FirstField, InputType = textInput, Section = planFirstSection, Mandatory = false },
      new Field { Name = StringConstants.SecondField, InputType = textInput, Section = planFirstSection },
      new Field { Name = StringConstants.ThirdField, InputType = description, Section = planFirstSection},

      // plan second section fields
      new Field { Name = StringConstants.FirstField, InputType = textInput, Section = planSecondSection },
      new Field { Name = StringConstants.SecondField, InputType = textInput, Section = planSecondSection }
    };

    foreach (var field in planSectionsFields) _db.Add(field);

    await _db.SaveChangesAsync();
  }

  public async Task SeedDefaultStages()
  {
    var planStageType = new StageType { Value = SectionTypes.Plan };
    _db.Add(planStageType);

    var firstStage = new Stage { Value = Stages.Draft, DisplayName = Stages.Draft, SortOrder = 1, Type = planStageType };
    var thirdStage = new Stage { Value = Stages.AwaitingChanges, DisplayName = Stages.AwaitingChanges, SortOrder = 5, Type = planStageType};
    var secondStage = new Stage { Value = Stages.InReview, DisplayName = Stages.InReview, SortOrder = 2, Type = planStageType, NextStage = thirdStage};
    var fourthStage = new Stage { Value = Stages.Approved, DisplayName = Stages.Approved, SortOrder = 10, Type = planStageType};

    _db.Add(firstStage);
    _db.Add(secondStage);
    _db.Add(thirdStage);
    _db.Add(fourthStage);

    var noteStageType = new StageType { Value = SectionTypes.Note };
    _db.Add(noteStageType);

    var noteDraftStage = new Stage { Value = Stages.Draft, DisplayName = Stages.Draft, SortOrder = 1, Type = noteStageType };
    _db.Add(noteDraftStage);

    await _db.SaveChangesAsync();
  }

  public async Task SeedDefaultPlans()
  {
    var projects = await _db.Projects.ToListAsync();
    var users = await _db.Users.ToListAsync();
    var stages = await _db.Stages
      .Include(stage => stage.Type)
      .Where(x => x.Type.Value == SectionTypes.Plan || x.Type.Value == SectionTypes.Note)
      .ToListAsync();

    var planStages = stages.Where(x => x.Type.Value == SectionTypes.Plan).ToList();
    var noteStages = stages.Where(x => x.Type.Value == SectionTypes.Note).ToList();

    // plan stages
    var planDraftStage = planStages.Single(x => x.Value == Stages.Draft);
    var inReviewStage = planStages.Single(x => x.Value == Stages.InReview);
    var awaitingChangesStage = planStages.Single(x => x.Value == Stages.AwaitingChanges);

    // note stages
    var noteDraftStage = noteStages.Single(x => x.Value == Stages.Draft);

    // Students
    var studentOne = users.Single(x => x.FullName == StringConstants.StudentUserOne);
    var studentTwo = users.Single(x => x.FullName == StringConstants.StudentUserTwo);

    // Seed plans for the first project for the students
    // studentOne plans
    var projectOne = projects.Single(x => x.Name == StringConstants.FirstProject);
    var studentOnePlans = new List<Plan>
    {
      new Plan
      {
        Title = StringConstants.PlanOne, Project = projectOne, Owner = studentOne, Stage = inReviewStage,
        Note = new Note { Project = projectOne, Owner = studentOne, Stage = noteDraftStage }
      },
      new Plan
      {
        Title = StringConstants.PlanTwo, Project = projectOne, Owner = studentOne, Stage = planDraftStage,
        Note = new Note { Project = projectOne, Owner = studentOne, Stage = noteDraftStage }
      },
      new Plan
      {
        Title = StringConstants.PlanThree, Project = projectOne, Owner = studentOne, Stage = awaitingChangesStage,
        Note = new Note { Project = projectOne, Owner = studentOne, Stage = noteDraftStage }
      }
    };

    // studentTwo plans
    var studentTwoPlans = new List<Plan>
    {
      new Plan
      {
        Title = StringConstants.PlanOne, Project = projectOne, Owner = studentTwo, Stage = planDraftStage,
        Note = new Note { Project = projectOne, Owner = studentOne, Stage = noteDraftStage }
      },
      new Plan
      {
        Title = StringConstants.PlanThree, Project = projectOne, Owner = studentTwo, Stage = planDraftStage,
        Note = new Note { Project = projectOne, Owner = studentOne, Stage = noteDraftStage }
      },
    };
    foreach (var plan in studentOnePlans.Concat(studentTwoPlans)) _db.Add(plan);

    await _db.SaveChangesAsync();
  }

  // Seed plan field responses
  public async Task SeedDefaultFieldResponses()
  {
    var sections = await _db.Sections.Where(x => x.SectionType.Name == SectionTypes.Plan).ToListAsync();
    var plans = await _db.Plans.Include(x => x.Owner).ToListAsync();
    var fields = await _db.Fields.Where(x => x.Section.SectionType.Name == SectionTypes.Plan).ToListAsync();

    var planOne = plans.Single(x => x is { Title: StringConstants.PlanOne, Owner.FullName: StringConstants.StudentUserOne });

    var planSectionOne = sections.Single(x => x.Name == StringConstants.PlanFirstSection);
    var planSectionOneFields = fields.Where(x => x.Section.Id == planSectionOne.Id).ToList();

    var planSectionOneFieldResponses = new List<FieldResponse>
    {
      CreateFieldResponse(
        planSectionOneFields.Single(x => x.Name == StringConstants.FirstField),
        false,
        new List<FieldResponseValue>
        {
          new FieldResponseValue { Value = JsonSerializer.Serialize(StringConstants.SecondResponse), ResponseDate = DateTimeOffset.UtcNow },
          new FieldResponseValue { Value = JsonSerializer.Serialize(StringConstants.FirstResponse), ResponseDate = DateTimeOffset.MinValue }
        },
        new List<Comment>
        {
          new Comment { Value = StringConstants.FirstComment },
          new Comment { Value = StringConstants.SecondComment }
        }
      ),
      CreateFieldResponse(
        planSectionOneFields.Single(x => x.Name == StringConstants.SecondField),
        true,
        new List<FieldResponseValue>
        {
          new FieldResponseValue { Value = JsonSerializer.Serialize(StringConstants.ApprovedResponse), ResponseDate = DateTimeOffset.UtcNow }
        },
        new List<Comment>()
      ),
      CreateFieldResponse(
        planSectionOneFields.Single(x => x.Name == StringConstants.ThirdField),
        true,
        new List<FieldResponseValue>
        {
          new FieldResponseValue { Value = JsonSerializer.Serialize(StringConstants.ApprovedResponse), ResponseDate = DateTimeOffset.UtcNow },
          new FieldResponseValue { Value = JsonSerializer.Serialize(StringConstants.FirstResponse), ResponseDate = DateTimeOffset.MinValue }
        },
        new List<Comment>
        {
          new Comment { Value = StringConstants.FirstComment, CommentDate = DateTimeOffset.UtcNow }
        }
      )
    };

    foreach (var fieldResponse in planSectionOneFieldResponses)
      planOne.FieldResponses.Add(fieldResponse);
    await _db.SaveChangesAsync();


    var planSectionTwo = sections.Single(x => x.Name == StringConstants.PlanSecondSection);
    var planSectionTwoFields = fields.Where(x => x.Section.Id == planSectionTwo.Id).ToList();

    var planSectionTwoFieldResponses = new List<FieldResponse>
    {
      CreateFieldResponse(
        planSectionTwoFields.Single(x => x.Name == StringConstants.FirstField),
        true,
        new List<FieldResponseValue>
        {
          new FieldResponseValue { Value = JsonSerializer.Serialize(StringConstants.DefaultResponse), ResponseDate = DateTimeOffset.UtcNow }
        },
        new List<Comment>
        {
          new Comment { Value = StringConstants.FirstComment, CommentDate = DateTimeOffset.UtcNow },
          new Comment { Value = StringConstants.SecondComment, CommentDate = DateTimeOffset.UtcNow }
        }
      ),
      CreateFieldResponse(
        planSectionTwoFields.Single(x => x.Name == StringConstants.SecondField),
        true,
        new List<FieldResponseValue>
        {
          new FieldResponseValue { Value = JsonSerializer.Serialize(StringConstants.DefaultResponse), ResponseDate = DateTimeOffset.UtcNow }
        },
        new List<Comment>()
      )
    };

    foreach (var fieldResponse in planSectionTwoFieldResponses)
      planOne.FieldResponses.Add(fieldResponse);
    await _db.SaveChangesAsync();
  }

  private FieldResponse CreateFieldResponse(Field field, bool approved, List<FieldResponseValue> responseValues, List<Comment> comments)
  {
    return new FieldResponse
    {
      Approved = approved,
      Field = field,
      FieldResponseValues = responseValues,
      Conversation = comments
    };
  }
}
