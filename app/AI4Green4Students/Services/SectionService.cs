using System.Text.Json;
using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Field;
using AI4Green4Students.Models.Section;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class SectionService
{
  private readonly ApplicationDbContext _db;
  private readonly LiteratureReviewService _literatureReviews;
  private readonly PlanService _plans;

  public SectionService(
    ApplicationDbContext db,
    LiteratureReviewService literatureReviews,
    PlanService plans)
  {
    _db = db;
    _literatureReviews = literatureReviews;
    _plans = plans;
  }

  /// <summary>
  /// Get all sections including their type.
  /// </summary>
  /// <returns>Sections list</returns>
  public async Task<List<SectionModel>> List()
    => await _db.Sections.AsNoTracking()
      .Include(x => x.SectionType)
      .Select(x => new SectionModel(x)).ToListAsync();

  /// <summary>
  /// Get all sections of a specific type.
  /// </summary>
  /// <param name="sectionTypeId">Section type id</param>
  /// <returns>Sections list of a specific type</returns>
  public async Task<List<SectionModel>> ListBySectionType(int sectionTypeId)
    => await _db.Sections.AsNoTracking()
      .Where(x => x.SectionType.Id == sectionTypeId)
      .Include(x => x.SectionType)
      .Select(x => new SectionModel(x))
      .ToListAsync();

  /// <summary>
  /// Create a new section. Section are associated to a project.
  /// If a section name already exists, the existing section is updated.
  /// </summary>
  /// <param name="model">DTO model for creating a new section</param>
  /// <returns>Newly created section</returns>
  public async Task<SectionModel> Create(CreateSectionModel model)
  {
    var isExistingValue = await _db.Sections
      .Where(x => EF.Functions.ILike(x.Name, model.Name))
      .Include(x => x.Project)
      .FirstOrDefaultAsync();

    if (isExistingValue is not null)
      return await Set(isExistingValue.Id, model); // Update existing Section if it exists

    // Else, create new Section
    var entity = new Section()
    {
      Name = model.Name,
      Project = _db.Projects.SingleOrDefault(x => x.Id == model.ProjectId)
                ?? throw new KeyNotFoundException(),
      SectionType = _db.SectionTypes.SingleOrDefault(x => x.Id == model.SectionTypeId)
                    ?? throw new KeyNotFoundException(),
      SortOrder = model.SortOrder,
    };

    await _db.Sections.AddAsync(entity);
    await _db.SaveChangesAsync();

    return await Get(entity.Id);
  }

  /// <summary>
  /// Update an existing section.
  /// </summary>
  /// <param name="id">Id of the section to update</param>
  /// <param name="model">DTO model for updating a section</param>
  /// <returns>Updated section</returns>
  public async Task<SectionModel> Set(int id, CreateSectionModel model)
  {
    var entity = await _db.Sections
                   .Where(x => x.Id == id)
                   .FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException(); // if section does not exist

    entity.Project = _db.Projects.SingleOrDefault(x => x.Id == model.ProjectId)
                     ?? throw new KeyNotFoundException();
    entity.SectionType = _db.SectionTypes.SingleOrDefault(x => x.Id == model.SectionTypeId)
                         ?? throw new KeyNotFoundException();
    entity.Name = model.Name;
    entity.SortOrder = model.SortOrder;

    _db.Sections.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }

  /// <summary>
  /// Get a section by its id.
  /// </summary>
  /// <param name="id">Id of the section to get</param>
  /// <returns>Section matching the id</returns>
  public async Task<SectionModel> Get(int id)
    =>
      await _db.Sections
        .AsNoTracking()
        .Where(x => x.Id == id)
        .Include(x => x.SectionType)
        .Select(x => new SectionModel(x))
        .SingleOrDefaultAsync()
      ?? throw new KeyNotFoundException();

  /// <summary>
  /// Get a list of literature review sections summaries.
  /// Includes each section's status, such as approval status and number of comments.
  /// </summary>
  /// <param name="literatureReviewId">Id of the literature review to be used when processing the summaries</param>
  /// <param name="sectionTypeId">
  /// Id if the section type
  /// Ensures that only sections matching the section type are returned
  /// </param>
  /// <returns>Section summaries list of a literature review</returns>
  public async Task<List<SectionSummaryModel>> ListSummariesByLiteratureReview(int literatureReviewId, int sectionTypeId)
  {
    var sections = await ListBySectionType(sectionTypeId);
    var literatureReviewFieldResponses = await _literatureReviews.GetLiteratureReviewFieldResponses(literatureReviewId);
    return GetSummaryModel(sections, literatureReviewFieldResponses);
  }
  
  /// <summary>
  /// Get a list of plan sections summaries.
  /// Includes each section's status, such as approval status and number of comments.
  /// </summary>
  /// <param name="planId">Id of the plan to be used when processing the summaries</param>
  /// <param name="sectionTypeId">
  /// Id if the section type
  /// Ensures that only sections matching the section type are returned
  /// </param>
  /// <returns>Section summaries list of a plan</returns>
  public async Task<List<SectionSummaryModel>> ListSummariesByPlan(int planId, int sectionTypeId)
  {
    var sections = await ListBySectionType(sectionTypeId);
    var planFieldResponses = await _plans.GetPlanFieldResponses(planId);
    return GetSummaryModel(sections, planFieldResponses);
  }

  /// <summary>
  /// Get a list of report sections summaries.
  /// Includes each section's status, such as approval status and number of comments.
  /// </summary>
  /// <param name="reportId">Id of the report to be used when processing the summaries</param>
  /// <param name="sectionTypeId">
  /// Id if the section type
  /// Ensures that only sections matching the section type are returned
  /// </param>
  /// <returns>Section summaries list of a report</returns>
  public async Task<List<SectionSummaryModel>> ListSummariesByReport(int reportId, int sectionTypeId)
  {
    var sections = await ListBySectionType(sectionTypeId);
    var reportFieldResponses = await GetReportFieldResponses(reportId);
    return GetSummaryModel(sections, reportFieldResponses);
  }

  /// <summary>
  /// Get a literature review section including its fields, last field response and comments.
  /// </summary>
  /// <param name="sectionId">Id of the section to get</param>
  /// <param name="literatureReviewId">Id of the literature review to get the field responses for</param>
  /// <returns>Literature review section with its fields, fields response and more.</returns>
  public async Task<SectionFormModel> GetLiteratureReviewFormModel(int sectionId, int literatureReviewId)
  {
    var section = await Get(sectionId);
    var sectionFields = await GetSectionFields(sectionId);
    var literatureReviewFieldResponses = await _literatureReviews.GetLiteratureReviewFieldResponses(literatureReviewId);
    return GetFormModel(section, sectionFields, literatureReviewFieldResponses);
  }
  
  /// <summary>
  /// Get a plan section including its fields, last field response and comments.
  /// </summary>
  /// <param name="sectionId">Id of the section to get</param>
  /// <param name="planId">Id of the plan to get the field responses for</param>
  /// <returns>Plan section with its fields, fields response and more.</returns>
  public async Task<SectionFormModel> GetPlanFormModel(int sectionId, int planId)
  {
    var section = await Get(sectionId);
    var sectionFields = await GetSectionFields(sectionId);
    var planFieldResponses = await _plans.GetPlanFieldResponses(planId);
    return GetFormModel(section, sectionFields, planFieldResponses);
  }

  /// <summary>
  /// Get a report section including its fields, last field response and comments.
  /// </summary>
  /// <param name="sectionId">Id of the section to get</param>
  /// <param name="reportId">Id of the plan to get the field responses for</param>
  /// <returns>Report section with its fields, fields response and more.</returns>
  public async Task<SectionFormModel> GetReportFormModel(int sectionId, int reportId)
  {
    var section = await Get(sectionId);
    var sectionFields = await GetSectionFields(sectionId);
    var reportFieldResponses = await GetReportFieldResponses(reportId);
    return GetFormModel(section, sectionFields, reportFieldResponses);
  }

  public async Task<SectionFormModel> SavePlan(SectionFormSubmissionModel model)
  {
    var planStage = _db.Plans.AsNoTracking().Where(x => x.Id == model.RecordId)
      .Include(pl => pl.Stage).Single()
      .Stage;

    var section = await GetSection(model.SectionId);

    var selectedFieldResponses = section.Fields
      .SelectMany(f => f.FieldResponses)
      .Where(fr => fr.PlanFieldResponses.Any(pfr => pfr.PlanId == model.RecordId));

    //check for the stage of the plan - this will define how we handle field values.
    //if its a draft, we can just save the existing (first and only) set of values
    //we can also save every single value from the form, as they're all eligible for submission
    if (planStage.DisplayName == PlanStages.Draft)
    {
      UpdateDraftFieldResponses(model, selectedFieldResponses);
    }
    //if its awaiting changes, we need to update the latest response value - a new response value will have been created when a comment was left
    // we're only interested in the fields which have been commented on - the others can't be changed so we can ignore them
    else if(planStage.DisplayName == PlanStages.AwaitingChanges)
    {
      UpdateAwaitingChangesFieldResponses(model, selectedFieldResponses);
    }

    await _db.SaveChangesAsync();
    return await GetPlanFormModel(model.SectionId, model.RecordId);
  }
  
  public async Task<SectionFormModel> SaveLiteratureReview(SectionFormSubmissionModel model)
  {
    var stage = _db.LiteratureReviews.AsNoTracking()
      .Where(x => x.Id == model.RecordId)
      .Include(x => x.Stage).Single()
      .Stage;

    var section = await GetSection(model.SectionId);

    var selectedFieldResponses = section.Fields
      .SelectMany(f => f.FieldResponses)
      .Where(fr => fr.LiteratureReviewFieldResponses
        .Any(x => x.LiteratureReviewId == model.RecordId));
    
    if (stage.DisplayName == LiteratureReviewStages.Draft)
    {
      UpdateDraftFieldResponses(model, selectedFieldResponses);
    }
    else if(stage.DisplayName == LiteratureReviewStages.AwaitingChanges)
    {
      UpdateAwaitingChangesFieldResponses(model, selectedFieldResponses);
    }

    await _db.SaveChangesAsync();
    return await GetLiteratureReviewFormModel(model.SectionId, model.RecordId);
  }
  
  private List<SectionSummaryModel> GetSummaryModel(List<SectionModel> sections, List<FieldResponse> fieldsResponses)
    => sections.Select(section => new SectionSummaryModel
      {
        Id = section.Id,
        Name = section.Name,
        Approved = fieldsResponses.Any(x => x.Field.Section.Id == section.Id) &&
                   fieldsResponses.Where(x => x.Field.Section.Id == section.Id).All(x => x.Approved),
        Comments = fieldsResponses
          .Where(x => x.Field.Section.Id == section.Id)
          .Sum(x => x.Conversation.Count(comment => !comment.Read)),
        SortOrder = section.SortOrder,
        SectionType = section.SectionType
      }).OrderBy(o => o.SortOrder)
      .ToList();

  private SectionFormModel GetFormModel(SectionModel section, List<Field> sectionFields,
    List<FieldResponse> fieldsResponses)
    => new SectionFormModel
    {
      Id = section.Id,
      Name = section.Name,
      FieldResponses = sectionFields.Select(x => new FieldResponseFormModel
      {
        Id = x.Id,
        Name = x.Name,
        Mandatory = x.Mandatory,
        Hidden = x.Hidden,
        SortOrder = x.SortOrder,
        FieldType = x.InputType.Name,
        DefaultResponse = x.DefaultResponse,
        SelectFieldOptions = x.SelectFieldOptions.Count >= 1
          ? x.SelectFieldOptions
            .Select(option => new SelectFieldOptionModel(option))
            .ToList()
          : null,
        Trigger = (x.TriggerCause != null && x.TriggerTarget != null)
          ? new TriggerFormModel
          {
            Value = x.TriggerCause,
            Target = x.TriggerTarget.Id
          }
          : null,
        FieldResponseId =  fieldsResponses.FirstOrDefault(y=>y.Field.Id == x.Id)?.Id,
        FieldResponse = DeserialiseSafely( 
          // direct deserialisation should work as we expect Value to be always a valid json string,
          // but just to ensure we correctly handle invalid json strings
          fieldsResponses
            .Where(y => y.Field.Id == x.Id)
            .Select(y => y.FieldResponseValues
              .OrderByDescending(z => z.ResponseDate)
              .FirstOrDefault()?.Value)
            .SingleOrDefault()),
        IsApproved = fieldsResponses.Any(y => y.Field.Id == x.Id && y.Approved),
        Comments = fieldsResponses
          .Where(y => y.Field.Id == x.Id)
          .Sum(y => y.Conversation.Count(comment => !comment.Read)),
      }).ToList()
    };

  private async Task<List<Field>> GetSectionFields(int sectionId)
    => await _db.Sections.Where(x => x.Id == sectionId)
         .Include(section => section.Fields)
         .ThenInclude(fields => fields.FieldResponses)
         .Include(section => section.Fields)
         .ThenInclude(fields => fields.InputType)
         .Include(section => section.Fields)
         .ThenInclude(fields => fields.SelectFieldOptions)
         .Select(x => x.Fields)
         .SingleAsync()
       ?? throw new KeyNotFoundException();
  
  private async Task<Section> GetSection(int sectionId)
  => await _db.Sections
       .Include(x => x.Fields)
       .ThenInclude(y => y.FieldResponses)
       .ThenInclude(z => z.PlanFieldResponses)
       .Include(x => x.Fields)
       .ThenInclude(y => y.FieldResponses)
       .ThenInclude(z => z.LiteratureReviewFieldResponses)
       .Include(x => x.Fields)
       .ThenInclude(y => y.FieldResponses)
       .ThenInclude(z => z.ReportFieldResponses)
       .Include(x => x.Fields)
       .ThenInclude(y => y.FieldResponses)
       .ThenInclude(z => z.FieldResponseValues)
       .Include(x => x.Fields)
       .ThenInclude(y => y.FieldResponses)
       .ThenInclude(z => z.Conversation)
       .SingleOrDefaultAsync(x => x.Id == sectionId)
     ?? throw new KeyNotFoundException();
  
  private void UpdateDraftFieldResponses(SectionFormSubmissionModel model, IEnumerable<FieldResponse> selectedFieldResponses)
  {
    foreach(var fieldResponseValue in model.FieldResponses)
    {
      var entityToUpdate = selectedFieldResponses.SingleOrDefault(x => x.Id == fieldResponseValue.Id).FieldResponseValues.SingleOrDefault();
      entityToUpdate.Value = fieldResponseValue.Value; // expecting value to be a json string
      _db.Update(entityToUpdate);
    }
  }

  private void UpdateAwaitingChangesFieldResponses(SectionFormSubmissionModel model, IEnumerable<FieldResponse> selectedFieldResponses)
  {
    foreach (var fieldResponseValue in model.FieldResponses)
    {
      var entityToUpdate = selectedFieldResponses.SingleOrDefault(x => x.Id == fieldResponseValue.Id && x.Approved == false)
        .FieldResponseValues.OrderByDescending(x => x.ResponseDate).FirstOrDefault();

      if(entityToUpdate != null)
      {
        entityToUpdate.Value = fieldResponseValue.Value;
        _db.Update(entityToUpdate);
      }
    }
  }

  /// <summary>
  /// Transform json string into a list of FieldResponseSubmissionModel,
  /// but also keep each field response value as json string.
  /// </summary>
  /// <param name="fieldResponses"> json string containing section field responses.</param>
  /// <returns></returns>
  public List<FieldResponseSubmissionModel> GetFieldResponses(string fieldResponses)
  {
    var initialFieldResponses = JsonSerializer.Deserialize<List<FieldResponseHelperModel>>(fieldResponses,
      new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    return initialFieldResponses.Select(item => new FieldResponseSubmissionModel
    {
      Id = item.Id,
      Value = item.Value.GetRawText() // keep value json string
    }).ToList();
  }
  
  /// <summary>
  /// Deserialise a json string. Ensures only valid json strings are deserialised.
  /// </summary>
  /// <param name="jsonString"> json string to deserialise </param>
  /// <returns> deserialised json element or null if invalid or empty </returns>
  private JsonElement? DeserialiseSafely(string jsonString)
  {
    if (string.IsNullOrWhiteSpace(jsonString)) return null;

    try
    {
      return JsonSerializer.Deserialize<JsonElement>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
    catch (JsonException)
    {
      try
      {
        // try to parse plain strng
        using var doc = JsonDocument.Parse($"\"{jsonString}\"");
        return doc.RootElement.Clone();
      }
      catch (JsonException)
      {
      }
    }
    return null;
  }
  
  private async Task<List<FieldResponse>> GetReportFieldResponses(int reportId)
    => await _db.Reports
         .AsNoTracking()
         .Where(x => x.Id == reportId)
         .SelectMany(x => x.ReportFieldResponses
           .Select(y => y.FieldResponse))
         .Include(x => x.Conversation)
         .ToListAsync()
       ?? throw new KeyNotFoundException();
  
}

