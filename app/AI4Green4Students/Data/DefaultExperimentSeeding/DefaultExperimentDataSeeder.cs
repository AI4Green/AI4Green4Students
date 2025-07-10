namespace AI4Green4Students.Data.DefaultExperimentSeeding;

using Auth;
using Constants;
using Entities;
using Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Field;
using Models.Project;
using Models.Section;
using Services;

public class DefaultExperimentDataSeeder
{
  private readonly ApplicationDbContext _db;
  private readonly FieldService _fields;
  private readonly InputTypeService _inputTypes;
  private readonly SectionService _sections;
  private readonly SectionTypeService _sectionTypes;
  private readonly UserManager<ApplicationUser> _users;

  public DefaultExperimentDataSeeder(
    ApplicationDbContext db,
    SectionService sections,
    InputTypeService inputTypes,
    FieldService fields,
    SectionTypeService sectionTypes,
    UserManager<ApplicationUser> users
  )
  {
    _db = db;
    _sections = sections;
    _inputTypes = inputTypes;
    _fields = fields;
    _sectionTypes = sectionTypes;
    _users = users;
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
    await SeedProjectGroupSections(project.Id, projectGroupSectionType.Id);
    await SeedPlanSections(project.Id, planSectionType.Id);
    await SeedtLiteratureReviewSections(project.Id, literatureReviewSectionType.Id);
    await SeedNoteSections(project.Id, noteSectionType.Id);
    await SeedReportSections(project.Id, reportSectionType.Id);

    //seed fields
    await SeedFields(project.Id);
  }

  /// <summary>
  /// Seed project.
  /// </summary>
  private async Task<ProjectModel> SeedProject()
  {
    var user = await _users.FindByEmailAsync(SuperUser.EmailAddress);
    if (user is null)
    {
      throw new ApplicationException($"{SuperUser.EmailAddress} not found");
    }

    var entity = new Project { Name = "AI4Green4Students", Instructors = new List<ApplicationUser> { user } };

    var project = await _db.Projects
      .Include(x => x.Instructors)
      .Where(x => EF.Functions.ILike(x.Name, entity.Name))
      .FirstOrDefaultAsync();

    if (project is null)
    {
      _db.Projects.Add(entity);
      await _db.SaveChangesAsync();
      return new ProjectModel { Id = entity.Id, Name = entity.Name };
    }

    foreach (var instructor in entity.Instructors)
    {
      if (project.Instructors.All(x => x.Id != instructor.Id))
      {
        project.Instructors.Add(instructor);
      }
    }

    project.Name = entity.Name;
    _db.Projects.Update(project);
    await _db.SaveChangesAsync();
    return new ProjectModel { Id = project.Id, Name = project.Name };
  }

  /// <summary>
  /// Seed project group sections.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="sectionTypeId">Section type (e.g. project group, plan) id.</param>
  private async Task SeedProjectGroupSections(int id, int sectionTypeId)
    => await _sections.Create(new CreateSectionModel
    {
      ProjectId = id,
      Name = DefaultExperimentConstants.ProjectGroupSummarySection,
      SortOrder = 1,
      SectionTypeId = sectionTypeId
    });

  /// <summary>
  /// Seed literature review sections.
  /// </summary>
  /// <param name="id"></param>
  /// <param name="sectionTypeId"></param>
  private async Task SeedtLiteratureReviewSections(int id, int sectionTypeId)
    => await _sections.Create(new CreateSectionModel
    {
      ProjectId = id,
      Name = DefaultExperimentConstants.LiteratureReviewSection,
      SortOrder = 1,
      SectionTypeId = sectionTypeId
    });

  /// <summary>
  /// Seed plan sections.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="sectionTypeId">Section type id.</param>
  private async Task SeedPlanSections(int id, int sectionTypeId)
  {
    var sections = new List<CreateSectionModel>
    {
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.ReactionSchemeSection,
        SortOrder = 2,
        SectionTypeId = sectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.CoshhSection,
        SortOrder = 3,
        SectionTypeId = sectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.SafetyDataSection,
        SortOrder = 4,
        SectionTypeId = sectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.ExperimentalProcecureSection,
        SortOrder = 5,
        SectionTypeId = sectionTypeId
      }
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
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.MetadataSection,
        SortOrder = 1,
        SectionTypeId = sectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.ReactionSchemeSection,
        SortOrder = 2,
        SectionTypeId = sectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.YieldAndGreenMetricsCalcSection,
        SortOrder = 3,
        SectionTypeId = sectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.ReactionDescriptionSection,
        SortOrder = 4,
        SectionTypeId = sectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.WorkupDescriptionSection,
        SortOrder = 5,
        SectionTypeId = sectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.TLCAnalysisSection,
        SortOrder = 6,
        SectionTypeId = sectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.ProductCharacterisatonSection,
        SortOrder = 7,
        SectionTypeId = sectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.ObeservationAndInferencesSection,
        SortOrder = 8,
        SectionTypeId = sectionTypeId
      }
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
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.AbstractSection,
        SortOrder = 1,
        SectionTypeId = sectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.IntroductionSection,
        SortOrder = 2,
        SectionTypeId = sectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.ResultsAndDiscussionSection,
        SortOrder = 3,
        SectionTypeId = sectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.ConclusionSection,
        SortOrder = 4,
        SectionTypeId = sectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.ExperimentalSection,
        SortOrder = 5,
        SectionTypeId = sectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.ReferencesSection,
        SortOrder = 6,
        SectionTypeId = sectionTypeId
      },
      new CreateSectionModel
      {
        ProjectId = id,
        Name = DefaultExperimentConstants.SupportingInfoSection,
        SortOrder = 7,
        SectionTypeId = sectionTypeId
      }
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
    return sections.First(x => x.ProjectId == id && x.Name == name && x.SectionType.Name == sectionType);
  }
}
