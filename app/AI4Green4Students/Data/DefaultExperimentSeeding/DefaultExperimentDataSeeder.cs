using AI4Green4Students.Constants;
using AI4Green4Students.Models.Experiment;
using AI4Green4Students.Models.Field;
using AI4Green4Students.Models.Project;
using AI4Green4Students.Models.Section;
using AI4Green4Students.Services;

namespace AI4Green4Students.Data.DefaultExperimentSeeding;

public class DefaultExperimentDataSeeder
{
  private readonly ProjectService _projects;
  private readonly SectionService _sections;
  private readonly InputTypeService _inputTypes;
  private readonly FieldService _fields;

  public DefaultExperimentDataSeeder(ProjectService projects, SectionService sections, InputTypeService inputTypes,
    FieldService fields)
  {
    _projects = projects;
    _sections = sections;
    _inputTypes = inputTypes;
    _fields = fields;
  }

  /// <summary>
  /// Seed an initial project "AI4Green4Students"
  /// </summary>
  public async Task<ProjectModel> SeedProject()
  {
    var project = new CreateProjectModel("AI4Green");
    return await _projects.Create(project);
  }

  /// <summary>
  /// Initial seed to get everything setup for the default project
  /// </summary>
  /// <returns></returns>
  public async Task SeedDefaultExperiment()
  {
    var project = await SeedProject();
    //todo
    //seed sections
    await SeedDefaultSections(project.Id);
    //seed fields
    await SeedDefaultFields();
  }

  public async Task SeedDefaultSections(int projectId)
  {
    var sections = new List<CreateSectionModel>()
    {
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.LiteratureReviewSection,
        SortOrder = 1
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.ReactionSchemeSection,
        SortOrder = 2
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.CoshhSection,
        SortOrder = 3
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.SafetyDataSection,
        SortOrder = 4
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.ExperimentalProcecureSection,
        SortOrder = 5
      }
    };

    foreach (var s in sections)
      await _sections.Create(s);
  }

  public async Task SeedDefaultFields()
  {
    var sections = await _sections.List();
    var inputTypes = await _inputTypes.List();

    var reactionSchemeSection = sections.Single(x => x.Name == DefaultExperimentConstants.ReactionSchemeSection);
    var literatureReviewSection = sections.Single(x => x.Name == DefaultExperimentConstants.LiteratureReviewSection);
    var coshhFormSection = sections.Single(x => x.Name == DefaultExperimentConstants.CoshhSection);
    var experimentalProcedureSection =
      sections.Single(x => x.Name == DefaultExperimentConstants.ExperimentalProcecureSection);
    var safetyDataSection = sections.Single(x => x.Name == DefaultExperimentConstants.SafetyDataSection);

    var fields = new List<CreateFieldModel>()
    {
      //Reaction Scheme section seeding
      new CreateFieldModel()
      {
        Section = reactionSchemeSection.Id,
        Name = DefaultExperimentConstants.ReactionSchemeField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ReactionScheme).Id
      },
      //Literature Review Section seeding
      new CreateFieldModel()
      {
        Section = literatureReviewSection.Id,
        Name = DefaultExperimentConstants.LiteratureReviewTextField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id,
        Mandatory = false
      },
      new CreateFieldModel()
      {
        Section = literatureReviewSection.Id,
        Name = DefaultExperimentConstants.LiteratureReviewFileUpload,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.File).Id
      },
      //COSHH Section seeding
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.SubstancesUsedField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.SubstanceTable).Id
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.SafetyRiskImplicationsField,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Header).Id
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.FireRiskField,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Radio).Id,
        TriggerCause = "Yes",
        TriggerTarget = new CreateFieldModel
        {
          Section = coshhFormSection.Id,
          Name = DefaultExperimentConstants.FireRiskPreventionField,
          InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id,
          Hidden = true
        }
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.ThermalField,
        SortOrder = 3,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Radio).Id,
        TriggerCause = "Yes",
        TriggerTarget = new CreateFieldModel
        {
          Section = coshhFormSection.Id,
          Name = DefaultExperimentConstants.ThermalPreventionField,
          InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id,
          Hidden = true
        }
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.MalodorousField,
        SortOrder = 4,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Radio).Id,
        TriggerCause = "Yes",
        TriggerTarget = new CreateFieldModel
        {
          Section = coshhFormSection.Id,
          Name = DefaultExperimentConstants.MalodorousPreventionField,
          InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id,
          Hidden = true
        }
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.AdditionalSafetyField,
        SortOrder = 5,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Header).Id
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.ControlMeasuresField,
        SortOrder = 6,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Multiple).Id,
        SelectFieldOptions = new List<string>()
        {
          DefaultExperimentConstants.FumehoodFieldOption,
          DefaultExperimentConstants.EyeProtectionFieldOption,
          DefaultExperimentConstants.NitrileGlovesFieldOption,
          DefaultExperimentConstants.RubberGlovesFieldOption,
          DefaultExperimentConstants.HeavyGlovesFieldOption,
          DefaultExperimentConstants.ScreensFieldOption,
          DefaultExperimentConstants.DustMaskFieldOption,
          DefaultExperimentConstants.InertAtmosFieldOption,
          DefaultExperimentConstants.SpillageTrayFieldOption
        }
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.PrimaryContainmentField,
        SortOrder = 7,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Multiple).Id,
        SelectFieldOptions = new List<string>()
        {
          DefaultExperimentConstants.OpenContainmentFieldOption,
          DefaultExperimentConstants.MultiNeckFieldOption,
          DefaultExperimentConstants.FlaskCondensorFieldOption,
          DefaultExperimentConstants.SealedFlaskFieldOption
        }
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.OtherRisksField,
        SortOrder = 8,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.EmergencyProceduresField,
        SortOrder = 9,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.RiskCategoryField,
        SortOrder = 10,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Multiple).Id,
        SelectFieldOptions = new List<string>()
        {
          DefaultExperimentConstants.AFieldOption,
          DefaultExperimentConstants.BFieldOption,
          DefaultExperimentConstants.CFieldOption,
          DefaultExperimentConstants.DFieldOption
        }
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.AdditionalControlsField,
        SortOrder = 11,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.WasteDisposalField,
        SortOrder = 12,
        InputType = inputTypes.Single(x => x.Name == InputTypes.SubstanceTable).Id
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.EmergencyProceduresMultiField,
        SortOrder = 13,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Multiple).Id,
        SelectFieldOptions = new List<string>()
        {
          DefaultExperimentConstants.FireExtinguisherFieldOption,
          DefaultExperimentConstants.Co2FieldOption,
          DefaultExperimentConstants.DryPowderFieldOption,
          DefaultExperimentConstants.SpillageFieldOption,
          DefaultExperimentConstants.SpillKitFieldOption,
          DefaultExperimentConstants.EvacuateAreaFieldOption,
          DefaultExperimentConstants.WashDownAreaFieldOption
        }
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.EmergencyTreatmentField,
        SortOrder = 14,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.ECStandardProtocolField,
        SortOrder = 15,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Header).Id
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.MESExposureField,
        SortOrder = 16,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Content).Id,
        DefaultValue = DefaultExperimentConstants.MESExposureFieldContent
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.LungsField,
        SortOrder = 17,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Content).Id,
        DefaultValue = DefaultExperimentConstants.LungsFieldContent
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.IfSwallowedField,
        SortOrder = 18,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Content).Id,
        DefaultValue = DefaultExperimentConstants.IfSwallowedFieldContent
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.IfUnconsciousField,
        SortOrder = 19,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Content).Id,
        DefaultValue = DefaultExperimentConstants.IfUnconsciousFieldContent
      },
      //Safety Data Section seeding
      new CreateFieldModel()
      {
        Section = safetyDataSection.Id,
        Name = DefaultExperimentConstants.SafetyDataField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id,
      },
      //Experimental Procedure Section seeding
      new CreateFieldModel()
      {
        Section = experimentalProcedureSection.Id,
        Name = DefaultExperimentConstants.ExperimentalProcedureField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      }
    };

    foreach (var f in fields)
      await _fields.Create(f);
  }
}
