namespace AI4Green4Students.Data.DefaultExperimentSeeding;

using Constants;
using Entities;
using Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Field;
using Models.Section;
using Services;

public class DefaultExperimentDataSeeder
{
  private readonly ApplicationDbContext _db;
  private readonly FieldService _fields;
  private readonly InputTypeService _inputTypes;
  private readonly SectionService _sections;
  private readonly SectionTypeService _sectionTypes;

  public DefaultExperimentDataSeeder(
    ApplicationDbContext db,
    SectionService sections,
    InputTypeService inputTypes,
    FieldService fields,
    SectionTypeService sectionTypes
  )
  {
    _db = db;
    _sections = sections;
    _inputTypes = inputTypes;
    _fields = fields;
    _sectionTypes = sectionTypes;
  }

  /// <summary>
  /// Initial seed to get everything setup for the default project
  /// </summary>
  /// <returns></returns>
  public async Task SeedDefaultExperiment()
  {
    var projectTypeId = await SeedProjectType();

    var sectionTypes = await _sectionTypes.List();
    // get section types
    var projectGroupSectionType = sectionTypes.Single(x => x.Name == SectionTypes.ProjectGroup);
    var literatureReviewSectionType = sectionTypes.Single(x => x.Name == SectionTypes.LiteratureReview);
    var planSectionType = sectionTypes.Single(x => x.Name == SectionTypes.Plan);
    var noteSectionType = sectionTypes.Single(x => x.Name == SectionTypes.Note);
    var reportSectionType = sectionTypes.Single(x => x.Name == SectionTypes.Report);

    //seed sections
    await SeedProjectGroupSections(projectTypeId, projectGroupSectionType.Id);
    await SeedPlanSections(projectTypeId, planSectionType.Id);
    await SeedLiteratureReviewSections(projectTypeId, literatureReviewSectionType.Id);
    await SeedNoteSections(projectTypeId, noteSectionType.Id);
    await SeedReportSections(projectTypeId, reportSectionType.Id);

    //seed fields
    await SeedFields(projectTypeId);
  }

  /// <summary>
  /// Seed default project type.
  /// </summary>
  private async Task<int> SeedProjectType()
  {
    var readyStage = await _db.Stages
      .Where(x => x.Type.Value == Defaults.ProjectTypeStage && x.DisplayName == Stages.Ready)
      .FirstOrDefaultAsync()
      ?? throw new KeyNotFoundException("Stage not found");

    var entity = new ProjectType
    {
      Name = Defaults.ProjectTypeName, Description = "Default project type.", Stage = readyStage
    };

    var projectType = await _db.ProjectTypes
      .Where(x => EF.Functions.ILike(x.Name, entity.Name))
      .FirstOrDefaultAsync();

    if (projectType is not null)
    {
      return projectType.Id;
    }

    _db.ProjectTypes.Add(entity);
    await _db.SaveChangesAsync();
    return entity.Id;
  }

  /// <summary>
  /// Seed project group sections.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="sectionTypeId">Section type (e.g. project group, plan) id.</param>
  private async Task SeedProjectGroupSections(int id, int sectionTypeId)
  {
    var model = new CreateSectionModel(DefaultExperimentConstants.ProjectGroupSummarySection, id, sectionTypeId, 1);
    await _sections.Create(model);
  }

  /// <summary>
  /// Seed literature review sections.
  /// </summary>
  /// <param name="id"></param>
  /// <param name="sectionTypeId"></param>
  private async Task SeedLiteratureReviewSections(int id, int sectionTypeId)
  {
    var model = new CreateSectionModel(DefaultExperimentConstants.LiteratureReviewSection, id, sectionTypeId, 1);
    await _sections.Create(model);
  }

  /// <summary>
  /// Seed plan sections.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="sectionTypeId">Section type id.</param>
  private async Task SeedPlanSections(int id, int sectionTypeId)
  {
    var sections = new List<CreateSectionModel>
    {
      new CreateSectionModel(DefaultExperimentConstants.ReactionSchemeSection, id, sectionTypeId, 2),
      new CreateSectionModel(DefaultExperimentConstants.CoshhSection, id, sectionTypeId, 3),
      new CreateSectionModel(DefaultExperimentConstants.SafetyDataSection, id, sectionTypeId, 4),
      new CreateSectionModel(DefaultExperimentConstants.ExperimentalProcecureSection, id, sectionTypeId, 5)
    };

    foreach (var section in sections)
    {
      await _sections.Create(section);
    }
  }

  /// <summary>
  /// Seed note sections.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="sectionTypeId">Section type id.</param>
  private async Task SeedNoteSections(int id, int sectionTypeId)
  {
    var sections = new List<CreateSectionModel>
    {
      new CreateSectionModel(DefaultExperimentConstants.MetadataSection, id, sectionTypeId, 1),
      new CreateSectionModel(DefaultExperimentConstants.ReactionSchemeSection, id, sectionTypeId, 2),
      new CreateSectionModel(DefaultExperimentConstants.YieldAndGreenMetricsCalcSection, id, sectionTypeId, 3),
      new CreateSectionModel(DefaultExperimentConstants.ReactionDescriptionSection, id, sectionTypeId, 4),
      new CreateSectionModel(DefaultExperimentConstants.WorkupDescriptionSection, id, sectionTypeId, 5),
      new CreateSectionModel(DefaultExperimentConstants.TLCAnalysisSection, id, sectionTypeId, 6),
      new CreateSectionModel(DefaultExperimentConstants.ProductCharacterisatonSection, id, sectionTypeId, 7),
      new CreateSectionModel(DefaultExperimentConstants.ObeservationAndInferencesSection, id, sectionTypeId, 8)
    };

    foreach (var section in sections)
    {
      await _sections.Create(section);
    }
  }

  /// <summary>
  /// Seed report sections.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="sectionTypeId">Section type id.</param>
  private async Task SeedReportSections(int id, int sectionTypeId)
  {
    var sections = new List<CreateSectionModel>
    {
      new CreateSectionModel(DefaultExperimentConstants.AbstractSection, id, sectionTypeId, 1),
      new CreateSectionModel(DefaultExperimentConstants.IntroductionSection, id, sectionTypeId, 2),
      new CreateSectionModel(DefaultExperimentConstants.ResultsAndDiscussionSection, id, sectionTypeId, 3),
      new CreateSectionModel(DefaultExperimentConstants.ConclusionSection, id, sectionTypeId, 4),
      new CreateSectionModel(DefaultExperimentConstants.ExperimentalSection, id, sectionTypeId, 5),
      new CreateSectionModel(DefaultExperimentConstants.ReferencesSection, id, sectionTypeId, 6),
      new CreateSectionModel(DefaultExperimentConstants.SupportingInfoSection, id, sectionTypeId, 7)
    };

    foreach (var section in sections)
    {
      await _sections.Create(section);
    }
  }

  /// <summary>
  /// Seed fields
  /// </summary>
  /// <param name="id">Project id.</param>
  private async Task SeedFields(int id)
  {
    // Get sections matching the names and section type.
    var pgSummarySection = await GetSection(
      id,
      DefaultExperimentConstants.ProjectGroupSummarySection,
      SectionTypes.ProjectGroup
    );

    var literatureReviewSection = await GetSection(
      id,
      DefaultExperimentConstants.LiteratureReviewSection,
      SectionTypes.LiteratureReview
    );

    // Plan sections
    var planReactionSchemeSection = await GetSection(
      id,
      DefaultExperimentConstants.ReactionSchemeSection,
      SectionTypes.Plan
    );

    var coshhFormSection = await GetSection(
      id,
      DefaultExperimentConstants.CoshhSection,
      SectionTypes.Plan
    );

    var experimentalProcedureSection = await GetSection(
      id,
      DefaultExperimentConstants.ExperimentalProcecureSection,
      SectionTypes.Plan
    );

    var safetyDataSection = await GetSection(
      id,
      DefaultExperimentConstants.SafetyDataSection,
      SectionTypes.Plan
    );

    // Note sections
    var metadataSection = await GetSection(id, DefaultExperimentConstants.MetadataSection, SectionTypes.Note);
    var yieldAndGreenMetricsCalcSection = await GetSection(
      id,
      DefaultExperimentConstants.YieldAndGreenMetricsCalcSection,
      SectionTypes.Note
    );

    var labnoteReactionSchemeSection = await GetSection(
      id,
      DefaultExperimentConstants.ReactionSchemeSection,
      SectionTypes.Note
    );

    var reactionDescriptionSection = await GetSection(
      id,
      DefaultExperimentConstants.ReactionDescriptionSection,
      SectionTypes.Note
    );

    var workupDescriptionSection = await GetSection(
      id,
      DefaultExperimentConstants.WorkupDescriptionSection,
      SectionTypes.Note
    );

    var tlcAnalysisSection = await GetSection(id, DefaultExperimentConstants.TLCAnalysisSection, SectionTypes.Note);

    var productCharacterisationSection = await GetSection(
      id,
      DefaultExperimentConstants.ProductCharacterisatonSection,
      SectionTypes.Note
    );

    var observationAndInferencesSection = await GetSection(
      id,
      DefaultExperimentConstants.ObeservationAndInferencesSection,
      SectionTypes.Note
    );

    // Report sections
    var abstractSection = await GetSection(id, DefaultExperimentConstants.AbstractSection, SectionTypes.Report);
    var introductionSection = await GetSection(id, DefaultExperimentConstants.IntroductionSection, SectionTypes.Report);

    var resAndDiscussionSection = await GetSection(
      id,
      DefaultExperimentConstants.ResultsAndDiscussionSection,
      SectionTypes.Report
      );

    var conclusionSection = await GetSection(id, DefaultExperimentConstants.ConclusionSection, SectionTypes.Report);
    var experimentalSection = await GetSection(id, DefaultExperimentConstants.ExperimentalSection, SectionTypes.Report);
    var referencesSection = await GetSection(id, DefaultExperimentConstants.ReferencesSection, SectionTypes.Report);
    var supportingInfoSection = await GetSection(id, DefaultExperimentConstants.SupportingInfoSection, SectionTypes.Report);

    var inputTypes = await _inputTypes.List();
    var fields = new List<CreateFieldModel>
    {
      //Project group summary section seeding
      new CreateFieldModel
      {
        Section = pgSummarySection.Id,
        Name = DefaultExperimentConstants.PGGroupPlanField,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ProjectGroupPlanTable).Id,
        Mandatory = false
      },
      new CreateFieldModel
      {
        Section = pgSummarySection.Id,
        Name = DefaultExperimentConstants.PGHazardSummaryField,
        SortOrder = 3,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ProjectGroupHazardTable).Id
      },
      new CreateFieldModel
      {
        Section = pgSummarySection.Id,
        Name = DefaultExperimentConstants.PGNotes,
        SortOrder = 4,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel
      {
        Section = pgSummarySection.Id,
        Name = DefaultExperimentConstants.PGLiteratureSummaryField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },

      //Reaction Scheme section seeding for plan
      new CreateFieldModel
      {
        Section = planReactionSchemeSection.Id,
        Name = DefaultExperimentConstants.ReactionSchemeField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ReactionScheme).Id
      },

      //Literature Review Section seeding
      new CreateFieldModel
      {
        Section = literatureReviewSection.Id,
        Name = DefaultExperimentConstants.LiteratureReviewTextField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id,
        Mandatory = false
      },
      new CreateFieldModel
      {
        Section = literatureReviewSection.Id,
        Name = DefaultExperimentConstants.LiteratureReviewFileUpload,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.File).Id
      },

      //COSHH Section seeding
      new CreateFieldModel
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.SafetyRiskImplicationsField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Header).Id
      },
      new CreateFieldModel
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
      new CreateFieldModel
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
      new CreateFieldModel
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
      new CreateFieldModel
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.AdditionalSafetyField,
        SortOrder = 5,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Header).Id
      },
      new CreateFieldModel
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.ControlMeasuresField,
        SortOrder = 6,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Multiple).Id,
        SelectFieldOptions = new List<string>
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
      new CreateFieldModel
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.PrimaryContainmentField,
        SortOrder = 7,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Multiple).Id,
        SelectFieldOptions = new List<string>
          {
            DefaultExperimentConstants.OpenContainmentFieldOption,
            DefaultExperimentConstants.MultiNeckFieldOption,
            DefaultExperimentConstants.FlaskCondensorFieldOption,
            DefaultExperimentConstants.SealedFlaskFieldOption
          }
      },
      new CreateFieldModel
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.OtherRisksField,
        SortOrder = 8,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.EmergencyProceduresField,
        SortOrder = 9,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.RiskCategoryField,
        SortOrder = 10,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Multiple).Id,
        SelectFieldOptions = new List<string>
          {
            DefaultExperimentConstants.AFieldOption,
            DefaultExperimentConstants.BFieldOption,
            DefaultExperimentConstants.CFieldOption,
            DefaultExperimentConstants.DFieldOption
          }
      },
      new CreateFieldModel
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.AdditionalControlsField,
        SortOrder = 11,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.WasteDisposalField,
        SortOrder = 12,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ChemicalDisposalTable).Id
      },
      new CreateFieldModel
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.EmergencyProceduresMultiField,
        SortOrder = 13,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Multiple).Id,
        SelectFieldOptions = new List<string>
          {
            DefaultExperimentConstants.Co2FireExtinguisherFieldOption,
            DefaultExperimentConstants.DryPowderFireExtinguisherFieldOption,
            DefaultExperimentConstants.SpillKitFieldOption,
            DefaultExperimentConstants.EvacuateAreaFieldOption,
            DefaultExperimentConstants.WashDownAreaFieldOption
          }
      },
      new CreateFieldModel
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.EmergencyTreatmentField,
        SortOrder = 14,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.ECStandardProtocolField,
        SortOrder = 15,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Header).Id
      },
      new CreateFieldModel
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.MESExposureField,
        SortOrder = 16,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Content).Id,
        DefaultValue = DefaultExperimentConstants.MESExposureFieldContent
      },
      new CreateFieldModel
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.LungsField,
        SortOrder = 17,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Content).Id,
        DefaultValue = DefaultExperimentConstants.LungsFieldContent
      },
      new CreateFieldModel
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.IfSwallowedField,
        SortOrder = 18,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Content).Id,
        DefaultValue = DefaultExperimentConstants.IfSwallowedFieldContent
      },
      new CreateFieldModel
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.IfUnconsciousField,
        SortOrder = 19,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Content).Id,
        DefaultValue = DefaultExperimentConstants.IfUnconsciousFieldContent
      },
      //Safety Data Section seeding
      new CreateFieldModel
      {
        Section = safetyDataSection.Id,
        Name = DefaultExperimentConstants.SafetyDataField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      //Experimental Procedure Section seeding
      new CreateFieldModel
      {
        Section = experimentalProcedureSection.Id,
        Name = DefaultExperimentConstants.ExperimentalProcedureField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },

      //Metadata Section seeding for lab note
      new CreateFieldModel
      {
        Section = metadataSection.Id,
        Name = DefaultExperimentConstants.ReactionNameField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Text).Id
      },
      new CreateFieldModel
      {
        Section = metadataSection.Id,
        Name = DefaultExperimentConstants.StatusField,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Radio).Id,
        SelectFieldOptions = new List<string>
          {
            DefaultExperimentConstants.StatusSuccessfulFieldOption,
            DefaultExperimentConstants.StatusUnsuccessfulFieldOption
          }
      },
      new CreateFieldModel
      {
        Section = metadataSection.Id,
        Name = DefaultExperimentConstants.TemperatureField,
        SortOrder = 3,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Number).Id
      },
      new CreateFieldModel
      {
        Section = metadataSection.Id,
        Name = DefaultExperimentConstants.StartDateAndTimeField,
        SortOrder = 4,
        InputType = inputTypes.Single(x => x.Name == InputTypes.DateAndTime).Id
      },
      new CreateFieldModel
      {
        Section = metadataSection.Id,
        Name = DefaultExperimentConstants.EndDateAndTimeField,
        SortOrder = 5,
        InputType = inputTypes.Single(x => x.Name == InputTypes.DateAndTime).Id
      },
      new CreateFieldModel
      {
        Section = metadataSection.Id,
        Name = DefaultExperimentConstants.DurationField,
        SortOrder = 6,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Number).Id
      },

      //Yield and Green Metrics Section seeding
      new CreateFieldModel
      {
        Section = yieldAndGreenMetricsCalcSection.Id,
        Name = DefaultExperimentConstants.YieldCalculationField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.YieldTable).Id
      },
      new CreateFieldModel
      {
        Section = yieldAndGreenMetricsCalcSection.Id,
        Name = DefaultExperimentConstants.GreenMetricsCalculationField,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.GreenMetricsTable).Id
      },

      //Reaction Scheme section seeding for lab note
      new CreateFieldModel
      {
        Section = labnoteReactionSchemeSection.Id,
        Name = DefaultExperimentConstants.ReactionSchemeField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ReactionScheme).Id
      },

      //Reaction Description Section seeding
      new CreateFieldModel
      {
        Section = reactionDescriptionSection.Id,
        Name = DefaultExperimentConstants.HypothesisField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel
      {
        Section = reactionDescriptionSection.Id,
        Name = DefaultExperimentConstants.ObjectiviesField,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel
      {
        Section = reactionDescriptionSection.Id,
        Name = DefaultExperimentConstants.ReactionDescriptionField,
        SortOrder = 3,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },

      //Workup Description Section seeding
      new CreateFieldModel
      {
        Section = workupDescriptionSection.Id,
        Name = DefaultExperimentConstants.WorkupDescriptionField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },

      //TLC Analysis Section seeding
      new CreateFieldModel
      {
        Section = tlcAnalysisSection.Id,
        Name = DefaultExperimentConstants.TLCAnalysisField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel
      {
        Section = tlcAnalysisSection.Id,
        Name = DefaultExperimentConstants.TLCAnalysisImgUploadField,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ImageFile).Id
      },

      //Product Characterisation Section seeding
      new CreateFieldModel
      {
        Section = productCharacterisationSection.Id,
        Name = DefaultExperimentConstants.ProductCharacterisationField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel
      {
        Section = productCharacterisationSection.Id,
        Name = DefaultExperimentConstants.ProductCharImgUploadField,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ImageFile).Id
      },

      //Observation and Inferences Section seeding
      new CreateFieldModel
      {
        Section = observationAndInferencesSection.Id,
        Name = DefaultExperimentConstants.ObeservationAndInferencesField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },

      #region# Report fields seeding

      //Abstract Section seeding
      new CreateFieldModel
      {
        Section = abstractSection.Id,
        Name = DefaultExperimentConstants.AbstractField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.FormattedTextInput).Id
      },

      //Introduction Section seeding
      new CreateFieldModel
      {
        Section = introductionSection.Id,
        Name = DefaultExperimentConstants.IntroductionField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.FormattedTextInput).Id
      },
      new CreateFieldModel
      {
        Section = introductionSection.Id,
        Name = DefaultExperimentConstants.ImageUploadField,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ImageFile).Id,
        Mandatory = false
      },

      //Experimental Section seeding
      new CreateFieldModel
      {
        Section = experimentalSection.Id,
        Name = DefaultExperimentConstants.MultiReactionSchemeField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.MultiReactionScheme).Id
      },
      new CreateFieldModel
      {
        Section = experimentalSection.Id,
        Name = DefaultExperimentConstants.ProcedureField,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.FormattedTextInput).Id
      },

      //Results and Discussion Section seeding
      new CreateFieldModel
      {
        Section = resAndDiscussionSection.Id,
        Name = DefaultExperimentConstants.MultiYieldTableField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.MultiYieldTable).Id
      },
      new CreateFieldModel
      {
        Section = resAndDiscussionSection.Id,
        Name = DefaultExperimentConstants.MultiGreenMetricsField,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.MultiGreenMetricsTable).Id
      },
      new CreateFieldModel
      {
        Section = resAndDiscussionSection.Id,
        Name = DefaultExperimentConstants.DiscussionField,
        SortOrder = 3,
        InputType = inputTypes.Single(x => x.Name == InputTypes.FormattedTextInput).Id
      },
      new CreateFieldModel
      {
        Section = resAndDiscussionSection.Id,
        Name = DefaultExperimentConstants.ImageUploadField,
        SortOrder = 4,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ImageFile).Id,
        Mandatory = false
      },

      //Conclusion Section seeding
      new CreateFieldModel
      {
        Section = conclusionSection.Id,
        Name = DefaultExperimentConstants.ConclusionField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.FormattedTextInput).Id
      },

      //References Section seeding
      new CreateFieldModel
      {
        Section = referencesSection.Id,
        Name = DefaultExperimentConstants.ReferencesField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.SortableList).Id
      },

      //Supporting Information Section seeding
      new CreateFieldModel
      {
        Section = supportingInfoSection.Id,
        Name = DefaultExperimentConstants.SupportInformationField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ImageFile).Id,
        Mandatory = false
      }

      #endregion
    };

    foreach (var f in fields)
    {
      await _fields.Create(f);
    }

    //TODO: Handling fields that are no longer needed. delete them? If yes, how do we handle the related data? maybe mark them as inactive?
  }

  /// <summary>
  /// Get section.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="name">Section name.</param>
  /// <param name="sectionType">Section type (e.g. Plan).</param>
  /// <returns></returns>
  private async Task<SectionModel> GetSection(int id, string name, string sectionType)
  {
    var sections = await _sections.List();
    return sections.First(x => x.ProjectType.Id == id && x.Name == name && x.SectionType.Name == sectionType);
  }
}
