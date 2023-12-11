using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities.Identity;

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
        Value =StringConstants.MailDomain,
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

    var student = new ApplicationUser()
    {
      FullName = StringConstants.StudentUser
    };
    _db.Add(student);

    var projectGroup = new ProjectGroup()
    {
      Name = StringConstants.FirstProjectGroup,
      Project = project,
      Students = new List<ApplicationUser> { student }
    };


    var experiment = new Experiment()
    {
      Title = StringConstants.FirstExperiment,
      ProjectGroup = projectGroup,
      Owner = student
    };

    var inputType = new InputType()
    {
      Name = StringConstants.TextInput
    };

    _db.Add(experiment);
    _db.Add(inputType);
    await _db.SaveChangesAsync();

    var sections = new List<Section>
    {
      new Section()
      {
        Name = StringConstants.FirstSection,
        SortOrder = 1,
        Project = project,
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
                Experiment = experiment,
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
                Conversation = new Conversation()
                {
                  Comments = new List<Comment>()
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
          }
        }
      }
      },
      new Section()
      {
        Name = StringConstants.SecondSection,
        SortOrder = 2,
        Project = project,
        Fields = new List<Field>
        {
          new Field()
          {
            Name = StringConstants.SecondField,
            FieldResponses = new List<FieldResponse> 
            {
              new FieldResponse()
              {
                Experiment = experiment,
                Approved = true
              }
            }
          }
        }
      }
    };
  
    foreach (var s in sections)
    _db.Add(s);

    await _db.SaveChangesAsync();
  }

#endregion

}
