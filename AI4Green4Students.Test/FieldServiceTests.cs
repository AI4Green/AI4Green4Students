using AI4Green4Students.Config;
using AI4Green4Students.Models.Field;
using AI4Green4Students.Services;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Moq;

namespace AI4Green4Students.Tests;
public class FieldServiceTests : IClassFixture<DatabaseFixture>
{
  private readonly DatabaseFixture _databaseFixture;
  private readonly Mock<AZExperimentStorageService> _mockAZExperimentStorageService;
  
  public FieldServiceTests(DatabaseFixture databaseFixture)
  {
    _databaseFixture = databaseFixture;
    _mockAZExperimentStorageService = new Mock<AZExperimentStorageService>(new Mock<BlobServiceClient>().Object, Options.Create(new AZOptions()));
  }

  /// <summary>
  /// Create field, with no field options or triggered fields
  /// </summary>
  [Fact]
  public async void TestCreateField()
  {
    //Arrange
    var fieldService = new FieldService(_databaseFixture.DbContext);
    var createField = await CommonDataSetup();

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
    var fieldService = new FieldService(_databaseFixture.DbContext);
    var createField = await CommonDataSetup();

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
    //Arrange
    var fieldService = new FieldService(_databaseFixture.DbContext);
    var createField = await CommonDataSetup();

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
    var fieldService = new FieldService(_databaseFixture.DbContext);
    var createField = await CommonDataSetup();

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
  public async Task<CreateFieldModel> CommonDataSetup()
  {
    var reportService = new ReportService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));
    var sectionService = new SectionService(_databaseFixture.DbContext, _mockAZExperimentStorageService.Object, reportService);
    var inputTypeService = new InputTypeService(_databaseFixture.DbContext);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    var inputTypes = await inputTypeService.List();
    var textInput = inputTypes.First(x => x.Name == StringConstants.TextInput);

    var sections = await sectionService.List();
    var firstSection = sections.First(x => x.Name == StringConstants.FirstSection);

    return  new CreateFieldModel()
    {
      Name = StringConstants.CreatedField,
      Section = firstSection.Id,
      Mandatory = true,
      InputType = textInput.Id
    };
  }

}
