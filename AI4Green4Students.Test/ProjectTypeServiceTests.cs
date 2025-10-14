namespace AI4Green4Students.Tests;

using Constants;
using Data;
using Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Field;
using Models.InputType;
using Models.ProjectType;
using Services;

public class ProjectTypeServiceTests : IClassFixture<TestHostFixture>, IAsyncLifetime
{
  private readonly TestHostFixture _fixture;
  private const string TargetName = "Target Project Type";
  private const string TargetDescription = "Test target project";
  public ProjectTypeServiceTests(TestHostFixture fixture) => _fixture = fixture;
  public async Task InitializeAsync() => await _fixture.InitializeServices();
  public async Task DisposeAsync() => await _fixture.DropTestDatabase();

  /// <summary>
  /// Import sections to a target project type.
  /// </summary>
  [Fact]
  public async Task Import_WithSections_CopiesSectionsSuccessfully()
  {
    // Arrange
    var (db, projectTypeService, sectionService, _, _) = await GetContextModel();

    var source = await db.ProjectTypes.SingleAsync(x => x.Name == ProjectTypeDefaults.Name);
    var target = await projectTypeService.Create(new CreateProjectTypeModel(TargetName, TargetDescription));

    // Act
    await projectTypeService.Import(target.Id, source.Id);

    // Assert
    var targetSections = await sectionService.ListByProjectType(target.Id);
    var sourceSections = await sectionService.ListByProjectType(source.Id);

    Assert.Equal(sourceSections.Count, targetSections.Count);
    Assert.Contains(targetSections, x => x.Name == StringConstants.PlanFirstSection);
    Assert.Contains(targetSections, x => x.Name == StringConstants.PlanSecondSection);
  }

  /// <summary>
  /// Import sections and fields (no triggers).
  /// </summary>
  [Fact]
  public async Task Import_WithSectionsAndFields_CopiesFieldsSuccessfully()
  {
    // Arrange
    var (db, projectTypeService, sectionService, fieldService, _) = await GetContextModel();

    var source = await db.ProjectTypes.SingleAsync(x => x.Name == ProjectTypeDefaults.Name);
    var target = await projectTypeService.Create(new CreateProjectTypeModel(TargetName, TargetDescription));

    // Act
    await projectTypeService.Import(target.Id, source.Id);

    // Assert
    var targetSections = await sectionService.ListByProjectType(target.Id);
    var planSection = targetSections.First(x => x.Name == StringConstants.PlanFirstSection);
    var fields = await fieldService.ListBySection(planSection.Id);

    Assert.Equal(3, fields.Count);
    Assert.Contains(fields, x => x.Name == StringConstants.FirstField);
    Assert.Contains(fields, x => x.Name == StringConstants.SecondField);
    Assert.Contains(fields, x => x.Name == StringConstants.ThirdField);

    // check fields are not shared between source and target
    var sourceSections = await sectionService.ListByProjectType(source.Id);
    var sourcePlanSection = sourceSections.First(x => x.Name == StringConstants.PlanFirstSection);
    var sourcePlanSectionFields = await fieldService.ListBySection(sourcePlanSection.Id);

    Assert.DoesNotContain(fields, x => sourcePlanSectionFields.Any(y => y.Id == x.Id));
  }

  /// <summary>
  /// Import sections with trigger fields
  /// </summary>
  [Fact]
  public async Task Import_WithTriggerFields_CopiesNestedFieldsSuccessfully()
  {
    // Arrange
    var (db, projectTypeService, sectionService, fieldService, inputTypes) = await GetContextModel();
    var source = await db.ProjectTypes.SingleAsync(x => x.Name == ProjectTypeDefaults.Name);

    // add trigger field to source
    var sourceSections = await sectionService.ListByProjectType(source.Id);
    var sourceSection = sourceSections.First(s => s.Name == StringConstants.PlanFirstSection);

    var childField = new CreateSectionFieldModel(
      null,
      inputTypes.First(x => x.Name == InputTypes.Text).Id,
      false,
      "Triggered Child Field",
      string.Empty,
      0,
      true,
      []
    );

    var parentField = new CreateSectionFieldModel(
      null,
      inputTypes.First(x => x.Name == InputTypes.Radio).Id,
      true,
      "Parent Field with Trigger",
      string.Empty,
      4,
      false,
      [
        new CreateSelectFieldOptionModel(null, "Yes"),
        new CreateSelectFieldOptionModel(null, "No")
      ],
      "Yes",
      childField
    );

    await fieldService.SaveSectionFields(sourceSection.Id, [parentField]);

    var targetProjectType = await projectTypeService.Create(new CreateProjectTypeModel(TargetName, TargetDescription));

    // Act
    await projectTypeService.Import(targetProjectType.Id, source.Id);

    // Assert
    var sourceFields = await fieldService.ListBySection(sourceSection.Id);
    var targetSections = await sectionService.ListByProjectType(targetProjectType.Id);
    var targetSection = targetSections.First(s => s.Name == StringConstants.PlanFirstSection);
    var targetFields = await fieldService.ListBySection(targetSection.Id);

    Assert.Equal(sourceFields.Count, targetFields.Count);

    var importedParent = targetFields.FirstOrDefault(x => x.Name == parentField.Name);
    Assert.NotNull(importedParent);
    Assert.NotNull(importedParent.TriggerField);
    Assert.Equal("Yes", importedParent.TriggerField.Value);

    var importedChild = targetFields.FirstOrDefault(x => x.Id == importedParent.TriggerField.Id);
    Assert.NotNull(importedChild);
    Assert.Equal(childField.Name, importedChild.Name);
    Assert.True(importedChild.Hidden);
    Assert.False(importedChild.Mandatory);
  }

  /// <summary>
  /// Import sections with deeply nested trigger fields (3 levels)
  /// </summary>
  [Fact]
  public async Task Import_WithDeeplyNestedTriggerFields_CopiesAllLevelsSuccessfully()
  {
    // Arrange
    var (db, projectTypeService, sectionService, fieldService, inputTypes) = await GetContextModel();

    var source = await db.ProjectTypes.SingleAsync(x => x.Name == ProjectTypeDefaults.Name);
    var sourceSections = await sectionService.ListByProjectType(source.Id);
    var sourceSection = sourceSections.First(s => s.Name == StringConstants.PlanFirstSection);

    var inputTypeId = inputTypes.First(x => x.Name == InputTypes.Text).Id;

    // Build 3-level nested structure
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
      "Trigger Level 3",
      fieldThree
    );

    var field = new CreateSectionFieldModel(
      null,
      inputTypeId,
      true,
      "Level 1 Field",
      string.Empty,
      4,
      false,
      [],
      "Trigger Level 2",
      fieldTwo
    );

    await fieldService.SaveSectionFields(sourceSection.Id, [field]);

    var target = await projectTypeService.Create(new CreateProjectTypeModel(TargetName, TargetDescription));

    // Act
    await projectTypeService.Import(target.Id, source.Id);

    // Assert
    var sourceFields = await fieldService.ListBySection(sourceSection.Id);
    var targetSections = await sectionService.ListByProjectType(target.Id);
    var targetSection = targetSections.First(x => x.Name == StringConstants.PlanFirstSection);
    var targetFields = await fieldService.ListBySection(targetSection.Id);

    Assert.Equal(sourceFields.Count, targetFields.Count);

    var importedLevelOne = targetFields.FirstOrDefault(x => x.Name == "Level 1 Field");
    Assert.NotNull(importedLevelOne);
    Assert.NotNull(importedLevelOne.TriggerField);
    Assert.Equal(field.TriggerValue, importedLevelOne.TriggerField.Value);

    var importedLevelTwo = targetFields.FirstOrDefault(x => x.Id == importedLevelOne.TriggerField.Id);
    Assert.NotNull(importedLevelTwo);
    Assert.Equal(fieldTwo.Name, importedLevelTwo.Name);
    Assert.NotNull(importedLevelTwo.TriggerField);
    Assert.Equal(fieldTwo.TriggerValue, importedLevelTwo.TriggerField.Value);

    var importedLevelThree = targetFields.FirstOrDefault(x => x.Id == importedLevelTwo.TriggerField.Id);
    Assert.NotNull(importedLevelThree);
    Assert.Equal(fieldThree.Name, importedLevelThree.Name);
    Assert.Null(importedLevelThree.TriggerField);
  }

  private async Task<ContextModel> GetContextModel()
  {
    var db = _fixture.GetServiceProvider().GetRequiredService<ApplicationDbContext>();
    var projectTypeService = _fixture.GetServiceProvider().GetRequiredService<ProjectTypeService>();
    var sectionService = _fixture.GetServiceProvider().GetRequiredService<SectionService>();
    var fieldService = _fixture.GetServiceProvider().GetRequiredService<FieldService>();
    var inputTypeService = _fixture.GetServiceProvider().GetRequiredService<InputTypeService>();

    var dataSeeder = new DataSeeder(db);
    await dataSeeder.SeedDefaultTestExperiment();

    var inputTypes = await inputTypeService.List();

    return new ContextModel(db, projectTypeService, sectionService, fieldService, inputTypes);
  }

  private record ContextModel(
    ApplicationDbContext Db,
    ProjectTypeService ProjectTypeService,
    SectionService SectionService,
    FieldService FieldService,
    List<InputTypeModel> InputTypes
  );
}
