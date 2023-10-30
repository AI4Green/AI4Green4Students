using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data;

namespace AI4Green4Students.Tests;
public class DataSeeder
{
  private readonly ApplicationDbContext _db;

  public DataSeeder(ApplicationDbContext db)
  {
    _db = db;
  }

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
        Value = "@gmail.com",
        IsBlocked = false
      },
      new RegistrationRule()
      {
        Value = "@mail.com",
        IsBlocked = true
      },
      new RegistrationRule()
      {
        Value = "allowed@mail.com",
        IsBlocked = false
      },
      new RegistrationRule()
      {
        Value = "blocked@gmail.com",
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
        Value = "*",
        IsBlocked = true
      },
      new RegistrationRule()
      {
          Value = "@gmail.com",
          IsBlocked = false
       },
      new RegistrationRule()
      {
        Value="example@mail.com",
        IsBlocked = false
      }
    };

    foreach (var s in seedRegistrationRules)
    {
      _db.Add(s);
    }

    await _db.SaveChangesAsync();

  }

  public async Task SeedDefaultTestExperiment()
  {
    var experiment = new Experiment()
    {
      Title = "Test Experiment"
    };

    _db.Add(experiment);
    await _db.SaveChangesAsync();

    var sections = new List<Section>
    {
      new Section()
      {
        Name = "First Section",
        SortOrder = 1,
        Experiment = experiment,
        Fields = new List<Field>
        {
          new Field()
          {
            Name = "Example Field"
          }
        }
      },
      new Section()
      {
        Name = "Second Section",
        SortOrder = 2,
        Experiment = experiment,
        Approved = true
      }
    };

    foreach (var s in sections)
    _db.Add(s);

    await _db.SaveChangesAsync();

    //now add the responses for the test
    var fieldResponse = new FieldResponse()
    {
      Field = sections.First().Fields.First(),
      Conversation = new Conversation()
      {
        Comments = new List<Comment>()
        {
          new Comment()
          {
            Value = "1st Comment"
          },
          new Comment() 
          {
            Value = "2nd Comment"
          }
        }
      }
    };

    _db.FieldResponses.Add(fieldResponse);
    _db.SaveChanges();
  }
}
