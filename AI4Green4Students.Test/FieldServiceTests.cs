namespace AI4Green4Students.Tests;

using Constants;
using Data;
using Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Models.Field;
using Services;

public class FieldServiceTests : IClassFixture<TestHostFixture>, IAsyncLifetime
{
  private readonly TestHostFixture _fixture;

  public FieldServiceTests(TestHostFixture fixture) => _fixture = fixture;
  public async Task InitializeAsync() => await _fixture.InitializeServices();
  public async Task DisposeAsync() => await _fixture.DropTestDatabase();


  /// <summary>
  /// Create field, with no field options or triggered fields
  /// </summary>
  [Fact]
  public async Task Create_WithNoOptionsOrTriggers_CreatesFieldWithNoTrigger()
  {
    //Arrange
    var (_, service, model) = await GetContextModel();

    //Act
    var field = await service.Create(model);

    //Assert
    Assert.Equal(StringConstants.CreatedField, field.Name);
    Assert.Null(field.TriggerValue);
    Assert.False(field.TriggerId.HasValue);
  }

  /// <summary>
  /// Create field, with a child trigger field
  /// </summary>
  [Fact]
  public async Task Create_WithTriggers_CreatesFieldWithTrigger()
  {
    //Arrange
    var (_, service, model) = await GetContextModel();

    model.TriggerCause = StringConstants.TriggerCause;
    model.TriggerTarget = new CreateFieldModel
    {
      Name = StringConstants.TriggerField,
      Hidden = true,
      Mandatory = false,
      InputType = model.InputType,
      Section = model.Section
    };

    //Act
    var field = await service.Create(model);
    var triggerField = field.TriggerId is not null ? await service.Get(field.TriggerId.Value) : null;

    //Assert
    Assert.Equal(StringConstants.CreatedField, field.Name);
    Assert.Equal(StringConstants.TriggerCause, field.TriggerValue);
    Assert.Equal(StringConstants.TriggerField, triggerField?.Name);
  }

  /// <summary>
  /// Create a field, with child trigger options
  /// </summary>
  [Fact]
  public async Task Create_WithOptions_CreatesFieldWithOptions()
  {
    //Arrange
    var (_, service, model) = await GetContextModel();

    model.SelectFieldOptions = new List<string>
    {
      StringConstants.FirstOption,
      StringConstants.SecondOption,
      StringConstants.ThirdOption
    };

    //Act
    var field = await service.Create(model);

    //Assert
    var selectFieldOptions = field.SelectFieldOptions?.Select(x=>x.Name).ToArray();

    Assert.Equal(StringConstants.CreatedField, field.Name);
    Assert.Equal(3, field.SelectFieldOptions?.Count);
    Assert.Contains(StringConstants.FirstOption, string.Join(",", selectFieldOptions ?? []));
  }

  /// <summary>
  /// Create a field with both trigger options and  child field
  /// </summary>
  [Fact]
  public async Task Create_WithOptionsAndTriggers_CreatesFieldWithOptionsAndTrigger()
  {
    //Arrange
    var (_, service, model) = await GetContextModel();

    model.SelectFieldOptions = new List<string>
    {
      StringConstants.FirstOption,
      StringConstants.SecondOption,
      StringConstants.ThirdOption
    };

    model.TriggerCause = StringConstants.TriggerCause;
    model.TriggerTarget = new CreateFieldModel
    {
      Name = StringConstants.TriggerField,
      Hidden = true,
      Mandatory = false,
      InputType = model.InputType,
      Section = model.Section
    };

    //Act
    var field = await service.Create(model);
    var triggerField = field.TriggerId is not null ? await service.Get(field.TriggerId.Value) : null;

    //Assert
    var selectFieldOptions = field.SelectFieldOptions?.Select(x=>x.Name).ToArray();

    Assert.Equal(StringConstants.CreatedField, field.Name);
    Assert.Equal(StringConstants.TriggerCause, field.TriggerValue);
    Assert.Equal(StringConstants.TriggerField, triggerField?.Name);

    Assert.Equal(3, field.SelectFieldOptions?.Count);
    Assert.Contains(StringConstants.FirstOption, string.Join(",", selectFieldOptions ?? []));

  }

  private async Task<ContextModel> GetContextModel()
  {
    var db = _fixture.GetServiceProvider().GetRequiredService<ApplicationDbContext>();
    var fieldService = _fixture.GetServiceProvider().GetRequiredService<FieldService>();

    var dataSeeder = new DataSeeder(db);
    await dataSeeder.SeedDefaultTestExperiment();

    var sectionService = _fixture.GetServiceProvider().GetRequiredService<SectionService>();
    var inputTypeService = _fixture.GetServiceProvider().GetRequiredService<InputTypeService>();

    var inputTypes = await inputTypeService.List();
    var textInput = inputTypes.First(x => x.Name == InputTypes.Text);

    var sections = await sectionService.List();
    var firstSection = sections.First(x => x.Name == StringConstants.PlanFirstSection);

    // basic field model to be used in the tests
    var model = new CreateFieldModel
    {
      Name = StringConstants.CreatedField,
      Section = firstSection.Id,
      Mandatory = true,
      InputType = textInput.Id
    };

    return new ContextModel(db, fieldService, model);
  }

  private record ContextModel(
    ApplicationDbContext Db,
    FieldService FieldService,
    CreateFieldModel CreateModel
  );
}
