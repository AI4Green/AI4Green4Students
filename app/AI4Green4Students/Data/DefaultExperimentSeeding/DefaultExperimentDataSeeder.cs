using AI4Green4Students.Constants;
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
  private readonly SectionTypeService _sectionTypes;

  public DefaultExperimentDataSeeder(ProjectService projects, SectionService sections, InputTypeService inputTypes,
    FieldService fields, SectionTypeService sectionTypes)
  {
    _projects = projects;
    _sections = sections;
    _inputTypes = inputTypes;
    _fields = fields;
    _sectionTypes = sectionTypes;
  }

  /// <summary>
  /// Seed an initial project "AI4Green4Students"
  /// </summary>
  public async Task<ProjectModel> SeedProject()
  {
    var project = new CreateProjectModel
    {
      Name = "AI4Green4Students",
    };
    return await _projects.Create(project);
  }

  /// <summary>
  /// Initial seed to get everything setup for the default project
  /// </summary>
  /// <returns></returns>
  public async Task SeedDefaultExperiment()
  {
    var project = await SeedProject();

    var sectionTypes = await _sectionTypes.List();
    // get section types
    var projectGroupSectionType = sectionTypes.Single(x => x.Name == SectionTypes.ProjectGroup);
    var literatureReviewSectionType = sectionTypes.Single(x => x.Name == SectionTypes.LiteratureReview);
    var planSectionType = sectionTypes.Single(x => x.Name == SectionTypes.Plan);
    var noteSectionType = sectionTypes.Single(x => x.Name == SectionTypes.Note);
    var reportSectionType = sectionTypes.Single(x => x.Name == SectionTypes.Report);

    //seed sections
    await SeedDefaultProjectGroupSections(project.Id, projectGroupSectionType.Id);
    await SeedDefaultPlanSections(project.Id, planSectionType.Id);
    await SeedDefaultLiteratureReviewSections(project.Id, literatureReviewSectionType.Id);
    await SeedDefaultNoteSections(project.Id, noteSectionType.Id);
    await SeedDefaultReportSections(project.Id, reportSectionType.Id);

    //seed fields
    await SeedDefaultFields(project.Id);
  }

  public async Task SeedDefaultProjectGroupSections(int projectId, int projectGroupSectionTypeId)
  {
    await _sections.Create(new CreateSectionModel
    {
      ProjectId = projectId,
      Name = DefaultExperimentConstants.ProjectGroupSummarySection,
      SortOrder = 1,
      SectionTypeId = projectGroupSectionTypeId
    });
  }
  
  public async Task SeedDefaultLiteratureReviewSections(int projectId, int literatureReviewSectionTypeId)
  {
    await _sections.Create(new CreateSectionModel
    {
      ProjectId = projectId,
      Name = DefaultExperimentConstants.LiteratureReviewSection,
      SortOrder = 1,
      SectionTypeId = literatureReviewSectionTypeId
    });
  }
  
  public async Task SeedDefaultPlanSections(int projectId, int planSectionTypeId)
  {
    var planSections = new List<CreateSectionModel>()
    {
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.ReactionSchemeSection,
        SortOrder = 2,
        SectionTypeId = planSectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.CoshhSection,
        SortOrder = 3,
        SectionTypeId = planSectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.SafetyDataSection,
        SortOrder = 4,
        SectionTypeId = planSectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.ExperimentalProcecureSection,
        SortOrder = 5,
        SectionTypeId = planSectionTypeId
      }
    };

    foreach (var s in planSections)
      await _sections.Create(s);
  }
  
  public async Task SeedDefaultNoteSections(int projectId, int noteSectionTypeId)
  {
    var noteSections = new List<CreateSectionModel>()
    {
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.MetadataSection,
        SortOrder = 1,
        SectionTypeId = noteSectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.ReactionSchemeSection,
        SortOrder = 2,
        SectionTypeId = noteSectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.YieldAndGreenMetricsCalcSection,
        SortOrder = 3,
        SectionTypeId = noteSectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.ReactionDescriptionSection,
        SortOrder = 4,
        SectionTypeId = noteSectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.WorkupDescriptionSection,
        SortOrder = 5,
        SectionTypeId = noteSectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.TLCAnalysisSection,
        SortOrder = 6,
        SectionTypeId = noteSectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.ProductCharacterisatonSection,
        SortOrder = 7,
        SectionTypeId = noteSectionTypeId
      },
      new CreateSectionModel
      { 
        ProjectId = projectId, 
        Name = DefaultExperimentConstants.ObeservationAndInferencesSection, 
        SortOrder = 8, 
        SectionTypeId = noteSectionTypeId
      }
    };

    foreach (var s in noteSections)
      await _sections.Create(s);
  }
  
  public async Task SeedDefaultReportSections(int projectId, int reportSectionTypeId)
  {
    var reportSections = new List<CreateSectionModel>
    {
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.AbstractSection,
        SortOrder = 1,
        SectionTypeId = reportSectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.IntroductionSection,
        SortOrder = 2,
        SectionTypeId = reportSectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.ResultsAndDiscussionSection,
        SortOrder = 3,
        SectionTypeId = reportSectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.ConclusionSection,
        SortOrder = 4,
        SectionTypeId = reportSectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.ExperimentalSection,
        SortOrder = 5,
        SectionTypeId = reportSectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = projectId,
        Name = DefaultExperimentConstants.ReferencesSection,
        SortOrder = 6,
        SectionTypeId = reportSectionTypeId
      },
      new CreateSectionModel
      { 
        ProjectId = projectId, 
        Name = DefaultExperimentConstants.SupportingInfoSection, 
        SortOrder = 7, 
        SectionTypeId = reportSectionTypeId
      }
    };

    foreach (var s in reportSections)
      await _sections.Create(s);
  }
  

  public async Task SeedDefaultFields(int projectId)
  {
    // Get sections matching the names and section type.
    var pgSummarySection = await GetSection(projectId, DefaultExperimentConstants.ProjectGroupSummarySection, SectionTypes.ProjectGroup);
    var literatureReviewSection = await GetSection(projectId, DefaultExperimentConstants.LiteratureReviewSection, SectionTypes.LiteratureReview);
    
    // Plan sections
    var planReactionSchemeSection = await GetSection(projectId, DefaultExperimentConstants.ReactionSchemeSection, SectionTypes.Plan);
    var coshhFormSection = await GetSection(projectId, DefaultExperimentConstants.CoshhSection, SectionTypes.Plan);
    var experimentalProcedureSection = await GetSection(projectId, DefaultExperimentConstants.ExperimentalProcecureSection, SectionTypes.Plan);
    var safetyDataSection = await GetSection(projectId, DefaultExperimentConstants.SafetyDataSection, SectionTypes.Plan);

    // Note sections
    var metadataSection = await GetSection(projectId, DefaultExperimentConstants.MetadataSection, SectionTypes.Note);
    var yieldAndGreenMetricsCalcSection = await GetSection(projectId, DefaultExperimentConstants.YieldAndGreenMetricsCalcSection, SectionTypes.Note);
    var labnoteReactionSchemeSection = await GetSection(projectId, DefaultExperimentConstants.ReactionSchemeSection, SectionTypes.Note);
    var reactionDescriptionSection = await GetSection(projectId, DefaultExperimentConstants.ReactionDescriptionSection, SectionTypes.Note);
    var workupDescriptionSection = await GetSection(projectId, DefaultExperimentConstants.WorkupDescriptionSection, SectionTypes.Note);
    var tlcAnalysisSection = await GetSection(projectId, DefaultExperimentConstants.TLCAnalysisSection, SectionTypes.Note);
    var productCharacterisationSection = await GetSection(projectId, DefaultExperimentConstants.ProductCharacterisatonSection, SectionTypes.Note);
    var observationAndInferencesSection = await GetSection(projectId, DefaultExperimentConstants.ObeservationAndInferencesSection, SectionTypes.Note);
    
    // Report sections
    var abstractSection = await GetSection(projectId, DefaultExperimentConstants.AbstractSection, SectionTypes.Report);
    var introductionSection = await GetSection(projectId, DefaultExperimentConstants.IntroductionSection, SectionTypes.Report);
    var resAndDiscussionSection = await GetSection(projectId, DefaultExperimentConstants.ResultsAndDiscussionSection, SectionTypes.Report);
    var conclusionSection = await GetSection(projectId, DefaultExperimentConstants.ConclusionSection, SectionTypes.Report);
    var experimentalSection = await GetSection(projectId, DefaultExperimentConstants.ExperimentalSection, SectionTypes.Report);
    var referencesSection = await GetSection(projectId, DefaultExperimentConstants.ReferencesSection, SectionTypes.Report);
    var supportingInfoSection = await GetSection(projectId, DefaultExperimentConstants.SupportingInfoSection, SectionTypes.Report);
    
    var inputTypes = await _inputTypes.List(); // get input types
    var fields = new List<CreateFieldModel>()
    {
      //Project group summary section seeding
      new CreateFieldModel()
      {
        Section = pgSummarySection.Id,
        Name = DefaultExperimentConstants.PGGroupPlanField,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ProjectGroupPlanTable).Id,
        Mandatory = false
      },
      new CreateFieldModel()
      {
        Section = pgSummarySection.Id,
        Name = DefaultExperimentConstants.PGHazardSummaryField,
        SortOrder = 3,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ProjectGroupHazardTable).Id
      },
      new CreateFieldModel()
      {
        Section = pgSummarySection.Id,
        Name = DefaultExperimentConstants.PGNotes,
        SortOrder = 4,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel()
      {
        Section = pgSummarySection.Id,
        Name = DefaultExperimentConstants.PGLiteratureSummaryField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      
      //Reaction Scheme section seeding for plan
      new CreateFieldModel()
      {
        Section = planReactionSchemeSection.Id,
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
        Name = DefaultExperimentConstants.SafetyRiskImplicationsField,
        SortOrder = 1,
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
        InputType = inputTypes.Single(x => x.Name == InputTypes.ChemicalDisposalTable).Id
      },
      new CreateFieldModel()
      {
        Section = coshhFormSection.Id,
        Name = DefaultExperimentConstants.EmergencyProceduresMultiField,
        SortOrder = 13,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Multiple).Id,
        SelectFieldOptions = new List<string>()
        {
          DefaultExperimentConstants.Co2FireExtinguisherFieldOption,
          DefaultExperimentConstants.DryPowderFireExtinguisherFieldOption,
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
      },
      
      //Metadata Section seeding for lab note
      new CreateFieldModel()
      {
        Section = metadataSection.Id,
        Name = DefaultExperimentConstants.ReactionNameField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Text).Id
      },
      new CreateFieldModel()
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
      new CreateFieldModel()
      {
        Section = metadataSection.Id,
        Name = DefaultExperimentConstants.TemperatureField,
        SortOrder = 3,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Number).Id
      },
      new CreateFieldModel()
      {
        Section = metadataSection.Id,
        Name = DefaultExperimentConstants.StartDateAndTimeField,
        SortOrder = 4,
        InputType = inputTypes.Single(x => x.Name == InputTypes.DateAndTime).Id
      },
      new CreateFieldModel()
      {
        Section = metadataSection.Id,
        Name = DefaultExperimentConstants.EndDateAndTimeField,
        SortOrder = 5,
        InputType = inputTypes.Single(x => x.Name == InputTypes.DateAndTime).Id
      },
      new CreateFieldModel()
      {
        Section = metadataSection.Id,
        Name = DefaultExperimentConstants.DurationField,
        SortOrder = 6,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Number).Id
      },
      
      //Yield and Green Metrics Section seeding
      new CreateFieldModel()
      {
        Section = yieldAndGreenMetricsCalcSection.Id,
        Name = DefaultExperimentConstants.YieldCalculationField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.YieldTable).Id
      },
      new CreateFieldModel()
      {
        Section = yieldAndGreenMetricsCalcSection.Id,
        Name = DefaultExperimentConstants.GreenMetricsCalculationField,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.GreenMetricsTable).Id
      },
      
      //Reaction Scheme section seeding for lab note
      new CreateFieldModel()
      {
        Section = labnoteReactionSchemeSection.Id,
        Name = DefaultExperimentConstants.ReactionSchemeField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ReactionScheme).Id
      },
      
      //Reaction Description Section seeding
      new CreateFieldModel()
      {
        Section = reactionDescriptionSection.Id,
        Name = DefaultExperimentConstants.HypothesisField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel()
      {
        Section = reactionDescriptionSection.Id,
        Name = DefaultExperimentConstants.ObjectiviesField,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel()
      {
        Section = reactionDescriptionSection.Id,
        Name = DefaultExperimentConstants.ReactionDescriptionField,
        SortOrder = 3,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      
      //Workup Description Section seeding
      new CreateFieldModel()
      {
        Section = workupDescriptionSection.Id,
        Name = DefaultExperimentConstants.WorkupDescriptionField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      
      //TLC Analysis Section seeding
      new CreateFieldModel()
      {
        Section = tlcAnalysisSection.Id,
        Name = DefaultExperimentConstants.TLCAnalysisField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel()
      {
        Section = tlcAnalysisSection.Id,
        Name = DefaultExperimentConstants.TLCAnalysisImgUploadField,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ImageFile).Id
      },
      
      //Product Characterisation Section seeding
      new CreateFieldModel()
      {
        Section = productCharacterisationSection.Id,
        Name = DefaultExperimentConstants.ProductCharacterisationField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel()
      {
        Section = productCharacterisationSection.Id,
        Name = DefaultExperimentConstants.ProductCharImgUploadField,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ImageFile).Id
      },
      
      //Observation and Inferences Section seeding
      new CreateFieldModel()
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
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      
      //Introduction Section seeding
      new CreateFieldModel
      {
        Section = introductionSection.Id,
        Name = DefaultExperimentConstants.IntroductionField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel
      {
        Section = introductionSection.Id,
        Name = DefaultExperimentConstants.ImageUploadField,
        SortOrder = 2,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ImageFile).Id
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
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
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
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
      },
      new CreateFieldModel
      {
        Section = resAndDiscussionSection.Id,
        Name = DefaultExperimentConstants.ImageUploadField,
        SortOrder = 4,
        InputType = inputTypes.Single(x => x.Name == InputTypes.ImageFile).Id
      },
      
      //Conclusion Section seeding
      new CreateFieldModel
      {
        Section = conclusionSection.Id,
        Name = DefaultExperimentConstants.ConclusionField,
        SortOrder = 1,
        InputType = inputTypes.Single(x => x.Name == InputTypes.Description).Id
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
        InputType = inputTypes.Single(x => x.Name == InputTypes.ImageFile).Id
      }
      #endregion
    };

    foreach (var f in fields)
      await _fields.Create(f);

    //TODO: Handling fields that are no longer needed. delete them? If yes, how do we handle the related data? maybe mark them as inactive?
  }

  private async Task<SectionModel> GetSection(int projectId, string sectionName, string sectionType)
  {
    var sections = await _sections.List();
    return sections.Single(x => x.ProjectId == projectId && x.Name == sectionName && x.SectionType.Name == sectionType);
  }
}
