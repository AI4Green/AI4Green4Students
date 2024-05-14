using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Models.Field;
using AI4Green4Students.Services;

namespace AI4Green4Students.Tests;
public class FieldServiceTests : IClassFixture<DatabaseFixture>
{
  private readonly DatabaseFixture _databaseFixture;
  
  public FieldServiceTests(DatabaseFixture databaseFixture)
  {
    _databaseFixture = databaseFixture;
  }
  
  private ApplicationDbContext CreateNewDbContext()
  {
    return _databaseFixture.CreateNewContext();
  }

  /// <summary>
  /// Create field, with no field options or triggered fields
  /// </summary>
  [Fact]
  public async void TestCreateField()
  {
    //Arrange
    var dbContext = CreateNewDbContext();
    var fieldService = new FieldService(dbContext);
    var createField = await CommonDataSetup(dbContext);

    //Act
    var field = await fieldService.Create(createField);

    //Assert
    Assert.Equal(StringConstants.CreatedField, field.Name);
    Assert.Equal(string.Empty, field.TriggerValue);
    Assert.Equal(0, field.TriggerId);
  }

  /// <summary>
  /// Create field, with a child trigger field
  /// </summary>
  [Fact]
  public async void TestCreateField_WithTriggers()
  {
    //Arrange
    var dbContext = CreateNewDbContext();
    var fieldService = new FieldService(dbContext);
    var createField = await CommonDataSetup(dbContext);

    createField.TriggerCause = StringConstants.TriggerCause;
    createField.TriggerTarget = new CreateFieldModel
    {
      Name = StringConstants.TriggerField,
      Hidden = true,
      Mandatory = false,
      InputType = createField.InputType,
      Section = createField.Section
    };

    //Act
    var field = await fieldService.Create(createField);
    var triggerField = await fieldService.Get(field.TriggerId);

    //Assert
    Assert.Equal(StringConstants.CreatedField, field.Name);
    Assert.Equal(StringConstants.TriggerCause, field.TriggerValue);
    Assert.Equal(StringConstants.TriggerField, triggerField.Name);
  }

  /// <summary>
  /// Create a field, with child trigger options
  /// </summary>
  [Fact]
  public async void TestCreateField_WithOptions()
  {
    //Arrange
    var dbContext = CreateNewDbContext();
    var fieldService = new FieldService(dbContext);
    var createField = await CommonDataSetup(dbContext);

    createField.SelectFieldOptions = new List<string>
    {
      StringConstants.FirstOption,
      StringConstants.SecondOption,
      StringConstants.ThirdOption
    };

    //Act
    var field = await fieldService.Create(createField);

    //Assert
    var selectFieldOptions = field.SelectFieldOptions.Select(x=>x.Name).ToArray();
    
    Assert.Equal(StringConstants.CreatedField, field.Name);
    Assert.Equal(3, field.SelectFieldOptions.Count);
    Assert.Contains(StringConstants.FirstOption, string.Join(",", selectFieldOptions));
  }

  /// <summary>
  /// Create a field with both trigger options and  child field
  /// </summary>
  [Fact]
  public async void TestCreateField_WithOptionsAndTriggers()
  {
    //Arrange
    var dbContext = CreateNewDbContext();
    var fieldService = new FieldService(dbContext);
    var createField = await CommonDataSetup(dbContext);

    createField.SelectFieldOptions = new List<string>
    {
      StringConstants.FirstOption,
      StringConstants.SecondOption,
      StringConstants.ThirdOption
    };

    createField.TriggerCause = StringConstants.TriggerCause;
    createField.TriggerTarget = new CreateFieldModel()
    {
      Name = StringConstants.TriggerField,
      Hidden = true,
      Mandatory = false,
      InputType = createField.InputType,
      Section = createField.Section
    };

    //Act
    var field = await fieldService.Create(createField);
    var triggerField = await fieldService.Get(field.TriggerId);

    //Assert
    var selectFieldOptions = field.SelectFieldOptions.Select(x=>x.Name).ToArray();
    
    Assert.Equal(StringConstants.CreatedField, field.Name);
    Assert.Equal(StringConstants.TriggerCause, field.TriggerValue);
    Assert.Equal(StringConstants.TriggerField, triggerField.Name);

    Assert.Equal(3, field.SelectFieldOptions.Count);
    Assert.Contains(StringConstants.FirstOption, string.Join(",", selectFieldOptions));

  }

  /// <summary>
  /// Setup the basic data needed by all the tests above, and return a basic create field model. Can have additional properties added relevant to each test.
  /// </summary>
  /// <returns></returns>
  public async Task<CreateFieldModel> CommonDataSetup(ApplicationDbContext dbContext)
  {
    var sectionService = new SectionService(dbContext);
    var inputTypeService = new InputTypeService(dbContext);

    var dataSeeder = new DataSeeder(dbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    var inputTypes = await inputTypeService.List();
    var textInput = inputTypes.First(x => x.Name == InputTypes.Text);

    var sections = await sectionService.List();
    var firstSection = sections.First(x => x.Name == StringConstants.PlanFirstSection);

    return new CreateFieldModel
    {
      Name = StringConstants.CreatedField,
      Section = firstSection.Id,
      Mandatory = true,
      InputType = textInput.Id
    };
  }

}
