namespace AI4Green4Students.Tests;

using Constants;
using Data;
using Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Models.Field;
using Models.InputType;
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
    var (_, service, model, _, _) = await GetContextModel();

    //Act
    var field = await service.Create(model);

    //Assert
    Assert.Equal(StringConstants.CreatedField, field.Name);
    Assert.Null(field.TriggerField);
  }

  /// <summary>
  /// Create field, with a child trigger field
  /// </summary>
  [Fact]
  public async Task Create_WithTriggers_CreatesFieldWithTrigger()
  {
    //Arrange
    var (_, service, model, _, _) = await GetContextModel();

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
    var triggerField = field.TriggerField is not null ? await service.Get(field.TriggerField.Id) : null;

    //Assert
    Assert.Equal(StringConstants.CreatedField, field.Name);
    Assert.Equal(StringConstants.TriggerCause, field.TriggerField?.Value);
    Assert.Equal(StringConstants.TriggerField, triggerField?.Name);
  }

  /// <summary>
  /// Create a field, with child trigger options
  /// </summary>
  [Fact]
  public async Task Create_WithOptions_CreatesFieldWithOptions()
  {
    //Arrange
    var (_, service, model, _, _) = await GetContextModel();

    model.SelectFieldOptions = new List<string>
    {
      StringConstants.FirstOption, StringConstants.SecondOption, StringConstants.ThirdOption
    };

    //Act
    var field = await service.Create(model);

    //Assert
    var selectFieldOptions = field.SelectFieldOptions?.Select(x => x.Name).ToArray();

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
    var (_, service, model, _, _) = await GetContextModel();

    model.SelectFieldOptions = new List<string>
    {
      StringConstants.FirstOption, StringConstants.SecondOption, StringConstants.ThirdOption
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
    var triggerField = field.TriggerField is not null ? await service.Get(field.TriggerField.Id) : null;

    //Assert
    var selectFieldOptions = field.SelectFieldOptions?.Select(x => x.Name).ToArray();

    Assert.Equal(StringConstants.CreatedField, field.Name);
    Assert.Equal(StringConstants.TriggerCause, field.TriggerField?.Value);
    Assert.Equal(StringConstants.TriggerField, triggerField?.Name);

    Assert.Equal(3, field.SelectFieldOptions?.Count);
    Assert.Contains(StringConstants.FirstOption, string.Join(",", selectFieldOptions ?? []));

  }

  /// <summary>
  /// Save section fields without trigger fields.
  /// </summary>
  [Fact]
  public async Task SaveSectionFields_WithoutTriggerFields_CreatesSectionFieldsSuccessfully()
  {
    // Arrange
    var (_, service, _, inputTypes, sectionId) = await GetContextModel();

    var fieldOne = new CreateSectionFieldModel(
      null,
      inputTypes.First(x => x.Name == InputTypes.Radio).Id,
      true,
      "Field 1",
      string.Empty,
      1,
      false,
      [
        new CreateSelectFieldOptionModel(null, "Option 1"),
        new CreateSelectFieldOptionModel(null, "Option 2")
      ]
      );

    var fieldTwo = new CreateSectionFieldModel(
      null,
      inputTypes.First(x => x.Name == InputTypes.Text).Id,
      false,
      "Field 2",
      string.Empty,
      2,
      false,
      []
      );

    var createModel = new List<CreateSectionFieldModel>
    {
      fieldOne, fieldTwo
    };

    // Act
    await service.SaveSectionFields(sectionId, createModel);

    // Assert
    var list = await service.ListBySection(sectionId);

    Assert.Equal(2, list.Count);
    Assert.Contains(list, x => x.Name == fieldOne.Name && x.SortOrder == 1);
    Assert.Contains(list, x => x.Name == fieldTwo.Name && x.SortOrder == 2);

    var firstField = list.First(x => x.Name == fieldOne.Name);
    Assert.Equal(2, firstField.SelectFieldOptions?.Count);
    Assert.Null(firstField.TriggerField);
  }

  /// <summary>
  /// Save section fields with nested trigger fields
  /// </summary>
  [Fact]
  public async Task SaveSectionFields_WithTriggerFields_CreatesSectionFieldsWithTriggersSuccessfully()
  {
    // Arrange
    var (_, service, _, inputTypes, sectionId) = await GetContextModel();
    var inputTypeId = inputTypes.First(x => x.Name == InputTypes.Radio).Id;

    var childField = new CreateSectionFieldModel(
      null,
      inputTypeId,
      false,
      "Child Field",
      string.Empty,
      0,
      true,
      []
    );
    var field = new CreateSectionFieldModel(
      null,
      inputTypeId,
      true,
      "Parent Field",
      string.Empty,
      1,
      false,
      [
        new CreateSelectFieldOptionModel(null, "Yes"),
        new CreateSelectFieldOptionModel(null, "No")
      ],
      "Yes",
      childField
    );

    var createModel = new List<CreateSectionFieldModel>
    {
      field
    };

    // Act
    await service.SaveSectionFields(sectionId, createModel);

    // Assert
    var list = await service.ListBySection(sectionId);
    var parent = list.First(x => x.Name == field.Name);
    var child = list.First(x => x.Id == parent.TriggerField?.Id);

    Assert.Equal(2, list.Count);
    Assert.NotNull(parent.TriggerField);
    Assert.NotNull(child);
  }

  /// <summary>
  /// Save section fields with deeply nested trigger fields (3 levels)
  /// </summary>
  [Fact]
  public async Task SaveSectionFields_WithDeeplyNestedTriggerFields_CreatesAllLevelsSuccessfully()
  {
    // Arrange
    var (_, service, _, inputTypes, sectionId) = await GetContextModel();
    var inputTypeId = inputTypes.First(x => x.Name == InputTypes.Text).Id;

    var fieldThree = new CreateSectionFieldModel(
      null,
      inputTypeId,
      false,
      "Level 3 Field",
      string.Empty,
      0,
      true,
      []
    );

    var fieldTwo = new CreateSectionFieldModel(
      null,
      inputTypeId,
      false,
      "Level 2 Field",
      string.Empty,
      0,
      true,
      [],
      "Show Level 3",
      fieldThree
    );

    var field = new CreateSectionFieldModel(
      null,
      inputTypeId,
      true,
      "Level 1 Field",
      string.Empty,
      1,
      false,
      [],
      "Show Level 2",
      fieldTwo
    );

    var createModel = new List<CreateSectionFieldModel>
    {
      field
    };

    // Act
    await service.SaveSectionFields(sectionId, createModel);

    // Assert
    var list = await service.ListBySection(sectionId);
    var firstLevel = list.First(x => x.Name == field.Name);
    var secondLevel = list.First(x => x.Id == firstLevel.TriggerField?.Id);
    var thirdLevel = list.First(x => x.Id == secondLevel.TriggerField?.Id);

    Assert.Equal(3, list.Count);
    Assert.NotNull(firstLevel.TriggerField);
    Assert.NotNull(secondLevel.TriggerField);
    Assert.NotNull(thirdLevel);
  }

  /// <summary>
  /// Update existing section fields and preserve trigger relationships
  /// </summary>
  [Fact]
  public async Task SaveSectionFields_UpdateExistingFieldsWithTriggers_UpdatesSuccessfully()
  {
    // Arrange
    var (db, service, _, inputTypes, sectionId) = await GetContextModel();
    var inputTypeId = inputTypes.First(x => x.Name == InputTypes.Text).Id;

    // create initial fields
    var originalChild = new CreateSectionFieldModel(
      null,
      inputTypeId,
      false,
      "Original Child",
      string.Empty,
      0,
      true,
      []
    );

    var originalField = new CreateSectionFieldModel(
      null,
      inputTypeId,
      true,
      "Original Field",
      string.Empty,
      1,
      false,
      [],
      "Original Trigger",
      originalChild
    );

    var createModel = new List<CreateSectionFieldModel>
    {
      originalField
    };

    await service.SaveSectionFields(sectionId, createModel);
    var list = await service.ListBySection(sectionId);
    var parent = list.First(x => x.Name == originalField.Name);

    // update
    var updatedChild = new CreateSectionFieldModel(
      parent.TriggerField?.Id,
      inputTypeId,
      true,
      "Updated Child",
      string.Empty,
      0,
      true,
      []
    );

    var updatedField = new CreateSectionFieldModel(
      parent.Id,
      inputTypeId,
      false,
      "Updated Field",
      string.Empty,
      1,
      false,
      [],
      "Updated Trigger",
      updatedChild
    );

    var updateModel = new List<CreateSectionFieldModel>
    {
      updatedField
    };

    // Act
    await service.SaveSectionFields(sectionId, updateModel);

    // Assert
    list = await service.ListBySection(sectionId);
    Assert.Equal(2, list.Count);

    parent = list.First(x => x.Name == updatedField.Name);
    var child = list.First(x => x.Id == parent.TriggerField?.Id);

    Assert.False(parent.Mandatory);
    Assert.Equal(updatedField.TriggerValue, parent.TriggerField?.Value);
    Assert.NotNull(parent.TriggerField);

    Assert.NotNull(child);
    Assert.Equal(updatedChild.Name, child.Name);
    Assert.True(child.Mandatory);
  }

  /// <summary>
  /// Remove trigger field when updating
  /// </summary>
  [Fact]
  public async Task SaveSectionFields_RemoveTriggerField_ClearsTriggerSuccessfully()
  {
    // Arrange
    var (db, service, _, inputTypes, sectionId) = await GetContextModel();
    var inputTypeId = inputTypes.First(x => x.Name == InputTypes.Text).Id;

    // create
    var childField = new CreateSectionFieldModel(
      null,
      inputTypeId,
      false,
      "Child Field",
      string.Empty,
      0,
      true,
      []
    );

    var parentField = new CreateSectionFieldModel(
      null,
      inputTypeId,
      true,
      "Parent Field",
      string.Empty,
      1,
      false,
      [],
      "Trigger",
      childField
    );

    var createModel = new List<CreateSectionFieldModel>
    {
      parentField
    };

    await service.SaveSectionFields(sectionId, createModel);
    var list = await service.ListBySection(sectionId);
    var parent = list.First(x => x.Name == parentField.Name);

    // update - remove trigger field
    var updatedParentField = new CreateSectionFieldModel(
      parent.Id,
      inputTypeId,
      true,
      parentField.Name,
      string.Empty,
      1,
      false,
      []
    );

    var updatedModels = new List<CreateSectionFieldModel>
    {
      updatedParentField
    };

    // Act
    await service.SaveSectionFields(sectionId, updatedModels);

    // Assert
    list = await service.ListBySection(sectionId);

    Assert.Single(list);
    Assert.Null(list.First().TriggerField);
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
      Name = StringConstants.CreatedField, Section = firstSection.Id, Mandatory = true, InputType = textInput.Id
    };

    return new ContextModel(db, fieldService, model, inputTypes, firstSection.Id);
  }

  private record ContextModel(
    ApplicationDbContext Db,
    FieldService FieldService,
    CreateFieldModel CreateModel,
    List<InputTypeModel> InputTypes,
    int SectionId
  );
}
