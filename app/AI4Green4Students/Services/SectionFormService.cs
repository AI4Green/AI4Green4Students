namespace AI4Green4Students.Services;

using System.Text.Json;
using Constants;
using Data;
using Data.Entities;
using Data.Entities.SectionTypeData;
using Microsoft.EntityFrameworkCore;
using Models.Field;
using Models.Section;
using Models.Section.Form;
using Utilities;

/// <summary>
/// Contains methods to handle and manage section forms.
/// </summary>
public class SectionFormService
{
  private readonly ApplicationDbContext _db;
  private readonly FieldResponseService _fieldResponses;
  private readonly FieldService _fields;
  private readonly SectionService _sections;

  public SectionFormService(
    ApplicationDbContext db,
    SectionService sections,
    FieldService fields,
    FieldResponseService fieldResponses
  )
  {
    _db = db;
    _sections = sections;
    _fields = fields;
    _fieldResponses = fieldResponses;
  }

  /// <summary>
  /// Generate section's summary for a given section type entity.
  /// </summary>
  /// <param name="id">Entity id.</param>
  /// <returns>Section summaries.</returns>
  public async Task<List<SectionSummaryModel>> ListSummary<T>(int id) where T : CoreSectionTypeData
  {
    var sectionType = SectionTypeHelper.GetSectionTypeName<T>();
    var entity = await GetEntityWithProject<T>(id);
    var fieldsResponses = await _fieldResponses.ListBySectionType<T>(id);
    var sections = await _sections.ListBySectionTypeName(entity.Project.ProjectType.Id, sectionType);

    // if field has a trigger target, map child field id to parent field
    var triggerMap = new Dictionary<int, Field>();
    fieldsResponses.ForEach(x =>
    {
      if (x.Field.TriggerTarget is not null)
      {
        triggerMap[x.Field.TriggerTarget.Id] = x.Field;
      }
    });

    var summaries = sections.Select(x =>
      {
        // get valid field responses for the section.
        // e.g. ignore field responses that are not triggered by parent field
        // useful when determining if a section is approved or not
        var validResponses = fieldsResponses.Where(y =>
          y.Field.Section.Id == x.Id &&
          IsFieldTriggeredByParentField(y.Field.Id, triggerMap)
        );

        var fieldResponses = validResponses.ToList();

        var feedback = new SectionFeedbackModel(
          fieldResponses.Count != 0 && fieldResponses.All(z => z.Approved),
          new SectionFeedbackCommentModel(
            fieldResponses.Count,
            fieldsResponses.Sum(z => z.Conversation.Count(comment => !comment.Read))
          )
        );

        return new SectionSummaryModel(x.Id, x.Name, x.SortOrder, feedback);
      })
      .OrderBy(o => o.SortOrder)
      .ToList();

    return summaries;
  }

  /// <summary>
  /// Generate a form model for a given section.
  /// </summary>
  /// <param name="id">Section type entity id. E.g. plan id.</param>
  /// <param name="sectionId">The section id to generate the form for.</param>
  /// <returns>Section form.</returns>
  public async Task<SectionFormModel> GetSectionForm<T>(int id, int sectionId) where T : BaseSectionTypeData
  {
    var fieldsResponses = await _fieldResponses.ListBySection<T>(id, sectionId);
    var section = await _sections.Get(sectionId);
    var sectionFields = await _fields.ListBySection(sectionId);

    var responsesByFieldId = fieldsResponses
      .GroupBy(x => x.Field.Id)
      .ToDictionary(y => y.Key, y => y.ToList());

    var fieldResponsesForm = sectionFields.Select(x =>
    {
      var selectedFieldResponses = responsesByFieldId.GetValueOrDefault(x.Id, new List<FieldResponse>());

      var fieldResponse = selectedFieldResponses.FirstOrDefault();
      var approved = selectedFieldResponses.Any(y => y.Approved);
      var total = selectedFieldResponses.Sum(y => y.Conversation.Count);
      var unread = selectedFieldResponses.Sum(y => y.Conversation.Count(c => !c.Read));
      var latestResponse = selectedFieldResponses
        .Select(y => y.FieldResponseValues.MaxBy(z => z.ResponseDate)?.Value)
        .FirstOrDefault();

      var feedback = new FieldResponseFeedbackModel(approved, new FieldResponseFeedbackCommentModel(total, unread));
      var response = SerializerHelper
        .DeserializeOrDefault<JsonElement>(latestResponse ?? JsonSerializer.Serialize(x.DefaultResponse));

      return new FieldResponseFormModel(fieldResponse?.Id, x, feedback, response);

    }).ToList();

    return new SectionFormModel(section.Id, section.Name, fieldResponsesForm);
  }

  /// <summary>
  /// Save the section form data.
  /// </summary>
  /// <param name="model">Payload to save.</param>
  /// <returns>Section form.</returns>
  public async Task<SectionFormModel> SaveForm<T>(SectionFormPayloadModel model) where T : CoreSectionTypeData
  {
    // Transform the payload model to a submission model.
    // Basically, we are preparing the data to be saved in the database.
    var submission = new SectionFormSubmissionModel
    {
      SectionId = model.SectionId,
      RecordId = model.RecordId,
      FieldResponses = await _fieldResponses.CreateFieldResponseModels(
        model.FieldResponses,
        model.Files,
        model.FileFieldResponses
      ),
      NewFieldResponses = await _fieldResponses.CreateFieldResponseModels(
        model.NewFieldResponses,
        model.NewFiles,
        model.NewFileFieldResponses,
        true
      )
    };

    var existing = await GetEntityWithStage<T>(model.RecordId);
    var fieldResponses = await _fieldResponses.ListBySection<T>(submission.RecordId, submission.SectionId);

    var updatedValues = existing.Stage.DisplayName == Stages.Draft
      ? _fieldResponses.UpdateDraft(submission.FieldResponses, fieldResponses)
      : _fieldResponses.UpdateAwaitingChanges(submission.FieldResponses, fieldResponses);

    foreach (var updatedValue in updatedValues)
    {
      _db.Update(updatedValue);
    }
    await _db.SaveChangesAsync();

    if (submission.NewFieldResponses.Count == 0)
    {
      return await GetSectionForm<T>(model.RecordId, model.SectionId);
    }

    var newFieldResponses = await _fieldResponses.CreateResponses<T>(
      existing.Id,
      existing.Project.Id,
      submission.NewFieldResponses
    );

    existing.FieldResponses.AddRange(newFieldResponses);
    await _db.SaveChangesAsync();

    return await GetSectionForm<T>(submission.RecordId, submission.SectionId);
  }

  /// <summary>
  /// Get entity by id with project and project type included.
  /// </summary>
  /// <param name="id">Entity id.</param>
  /// <returns>Entity.</returns>
  private async Task<T> GetEntityWithProject<T>(int id) where T : CoreSectionTypeData
    => await _db.Set<T>()
      .Where(x => x.Id == id)
      .Include(x => x.Project)
      .ThenInclude(x => x.ProjectType)
      .FirstOrDefaultAsync() ?? throw new KeyNotFoundException();

  /// <summary>
  /// Get entity by id with project and project type included.
  /// </summary>
  /// <param name="id">Entity id.</param>
  /// <returns>Entity.</returns>
  private async Task<T> GetEntityWithStage<T>(int id) where T : CoreSectionTypeData
    => await _db.Set<T>()
      .Where(x => x.Id == id)
      .Include(x => x.Stage)
      .FirstOrDefaultAsync() ?? throw new KeyNotFoundException();

  /// <summary>
  /// Helper method to check if a field is triggered by a parent field.
  /// </summary>
  /// <param name="id">Field id to check</param>
  /// <param name="childToParentMap">Dictionary of child field id to parent field</param>
  /// <returns>Bool</returns>
  private static bool IsFieldTriggeredByParentField(int id, Dictionary<int, Field> childToParentMap)
  {
    if (!childToParentMap.TryGetValue(id, out var parentField))
    {
      return true;
    }
    var parentFieldResponse = parentField
      .FieldResponses.Select(x => x.FieldResponseValues.MaxBy(y => y.ResponseDate)?.Value)
      .SingleOrDefault();

    // we are checking whether a parent field response value is equal to the trigger cause.
    // since field response value is always a JSON string,
    // we need to deserialise it to the correct type before comparison
    switch (parentField.InputType.Name)
    {
      case InputTypes.Multiple:
      case InputTypes.Radio:
        return parentFieldResponse is not null
               && (
                 SerializerHelper
                   .DeserializeOrDefault<List<SelectFieldOptionModel>>(parentFieldResponse)
                   ?.Any(x => x.Name == parentField.TriggerCause) ?? false
               );

      default:
        // shouldn't reach here as we expect only a select (Radio and Multiple) fields to have trigger cause.
        return parentFieldResponse is not null
               && SerializerHelper.DeserializeOrDefault<string>(parentFieldResponse) == parentField.TriggerCause;
    }
  }
}
