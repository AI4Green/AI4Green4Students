using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using AI4Green4Students.Auth;
using Microsoft.EntityFrameworkCore;
using AI4Green4Students.Constants;
using AI4Green4Students.Data.Entities.SectionTypeData;

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

  #region Project -> ProjectGroup -> Experiment -> Field -> FieldResponse -> Conversation -> Comment

  public async Task SeedDefaultTestExperiment()
  {
    var project = new Project()
    {
      Name = StringConstants.FirstProject
    };

    _db.Add(project);

    var studentOne = new ApplicationUser()
    {
      FullName = StringConstants.StudentUserOne
    };
    var studentTwo = new ApplicationUser()
    {
      FullName = StringConstants.StudentUserTwo
    };
    
    _db.Add(studentOne);
    _db.Add(studentTwo);

    var instructor = new ApplicationUser()
    {
      FullName = StringConstants.InstructorUser
    };
    _db.Add(instructor);

    var studentRole = new IdentityRole(Roles.Student);
    _db.Add(studentRole);

    var instructorRole = new IdentityRole(Roles.Instructor);
    _db.Add(instructorRole);

    var studentUserOneRole = new IdentityUserRole<string>
    {
      RoleId = studentRole.Id,
      UserId = studentOne.Id
    };
    var studentUserTwoRole = new IdentityUserRole<string>
    {
      RoleId = studentRole.Id,
      UserId = studentTwo.Id
    };

    var instructorUserRole = new IdentityUserRole<string>
    {
      RoleId = instructorRole.Id,
      UserId = instructorRole.Id
    };

    _db.Add(studentUserOneRole);
    _db.Add(studentUserTwoRole);
    _db.Add(instructorUserRole);

    var projectGroupOne = new ProjectGroup()
    {
      Name = StringConstants.FirstProjectGroup,
      Project = project,
      Students = new List<ApplicationUser> { studentOne, studentTwo }
    };
    
    _db.Add(projectGroupOne);

    //stages needed for plan and reports
    var planStageType = new StageType
    {
      Value = StringConstants.PlanStageType
    };

    var firstStage = new Stage
    {
      Value = PlanStages.Draft,
      DisplayName = PlanStages.Draft,
      SortOrder = 1,
      Type = planStageType
    };

    var secondStage = new Stage
    {
      Value = PlanStages.InReview,
      DisplayName = PlanStages.InReview,
      SortOrder = 2,
      Type = planStageType
    };

    var thirdStage = new Stage
    {
      Value = PlanStages.AwaitingChanges,
      DisplayName = PlanStages.AwaitingChanges,
      SortOrder = 3,
      Type = planStageType,
      NextStage = secondStage
    };

    _db.Add(planStageType);
    _db.Add(firstStage);
    _db.Add(secondStage);
    _db.Add(thirdStage);

    var firstPlan = new Plan
    {
      Project = project,
      Owner = studentOne,
      Stage = secondStage,
      Note = new Note()
    };
    
    var secondPlan = new Plan
    {
      Project = project,
      Owner = studentOne,
      Stage = firstStage,
      Note = new Note()
    };
    
    var thirdPlan = new Plan
    {
      Project = project,
      Owner = studentTwo,
      Stage = firstStage,
      Note = new Note()
    };

    var inputType = new InputType
    {
      Name = StringConstants.TextInput
    };

    var sectionTypePlan = new SectionType { Name = StringConstants.SectionTypePlan };

    _db.Add(firstPlan);
    _db.Add(secondPlan);
    _db.Add(thirdPlan);
    _db.Add(sectionTypePlan);
    _db.Add(inputType);
    await _db.SaveChangesAsync();

    var sections = new List<Section>
    {
      new Section()
      {
        Name = StringConstants.FirstSection,
        SortOrder = 1,
        Project = project,
        SectionType = sectionTypePlan,
        Fields = new List<Field>
        {
          new Field()
          {
            Name = StringConstants.FirstField,
            InputType = inputType,
            FieldResponses = new List<FieldResponse>()
            {
              new FieldResponse()
              {
                Approved = false,
                FieldResponseValues = new List<FieldResponseValue>()
                {
                  new FieldResponseValue()
                  {
                    ResponseDate = DateTime.Now,
                    Value = StringConstants.FirstResponse
                  },
                  new FieldResponseValue()
                  {
                    ResponseDate = DateTime.MinValue,
                    Value = StringConstants.SecondResponse
                  }
                },
                Conversation = new List<Comment>()
                {
                  new Comment()
                  {
                    Value = StringConstants.FirstComment
                  },
                  new Comment()
                  {
                    Value = StringConstants.SecondComment
                  }
                }
              }
            }
          },
          new Field()
          {
            Name = StringConstants.UnansweredField,
            InputType = inputType,
            FieldResponses = new List<FieldResponse>()
            {
              new FieldResponse()
              {
                Approved = false,
                FieldResponseValues = new List<FieldResponseValue>()
                {
                  new FieldResponseValue()
                   {
                    ResponseDate = DateTime.Now,
                    Value = string.Empty
                   }
                  }
                }
               }
          }
      }
      },
      new Section()
      {
        Name = StringConstants.SecondSection,
        SortOrder = 2,
        Project = project,
        SectionType = sectionTypePlan,
        Fields = new List<Field>
        {
          new Field()
          {
            Name = StringConstants.SecondField,
            InputType = inputType,
            FieldResponses = new List<FieldResponse>
            {
              new FieldResponse()
              {
                Approved = true
              }
            }
          }
        }
      } };

    foreach (var section in sections)
    {
      _db.Add(section);

      // create a PlanFieldResponse for each FieldResponse and link it with the Plan
      foreach (var field in section.Fields)
      {
        foreach (var fieldResponse in field.FieldResponses)
        {
          firstPlan.FieldResponses.Add(fieldResponse);
        }
      }
    }

    await _db.SaveChangesAsync();
  }

  #endregion
}
