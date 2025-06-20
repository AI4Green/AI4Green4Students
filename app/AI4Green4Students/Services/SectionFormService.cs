namespace AI4Green4Students.Services;

using System.Text.Json;
using Constants;
using Data;
using Data.Entities;
using Data.Entities.SectionTypeData;
using Microsoft.EntityFrameworkCore;
using Models.Field;
using Models.Section;
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
  private readonly StageService _stages;

  public SectionFormService(
    ApplicationDbContext db,
    SectionService sections,
    FieldService fields,
    FieldResponseService fieldResponses,
    StageService stages
  )
  {
    _db = db;
    _sections = sections;
    _fields = fields;
    _fieldResponses = fieldResponses;
    _stages = stages;
  }

  /// <summary>
  /// Generate section's summary for a given section type entity.
  /// </summary>
  /// <param name="id">Entity id.</param>
  /// <returns>Section summaries.</returns>
  public async Task<List<SectionSummaryModel>> ListSummary<T>(int id) where T : CoreSectionTypeData
  {
    var sectionType = SectionTypeHelper.GetSectionTypeName<T>();
    var entity = await GetEntity<T>(id);
    var fieldsResponses = await _fieldResponses.ListBySectionType<T>(id);
    var sections = await _sections.ListBySectionTypeName(sectionType, entity.Project.Id);

    // if field has a trigger target, map child field id to parent field
    var triggerMap = new Dictionary<int, Field>();
    fieldsResponses.ForEach(fr =>
    {
      if (fr.Field.TriggerTarget is not null)
      {
        triggerMap[fr.Field.TriggerTarget.Id] = fr.Field;
      }
    });

    var permissions = await _stages.ListPermissions(entity.Stage.SortOrder, sectionType);
    var summaries = sections.Select(section =>
      {
        // get valid field responses for the section.
        // e.g. ignore field responses that are not triggered by parent field
        // useful when determining if a section is approved or not
        var validFieldResponses = fieldsResponses
          .Where(fr => fr.Field.Section.Id == section.Id && IsFieldTriggeredByParentField(fr.Field.Id, triggerMap));

        var fieldResponses = validFieldResponses.ToList();
        return new SectionSummaryModel
        {
          Id = section.Id,
          Name = section.Name,
          Approved = fieldResponses.Count != 0 && fieldResponses.All(fr => fr.Approved),
          Comments = fieldsResponses
            .Where(x => x.Field.Section.Id == section.Id)
            .Sum(x => x.Conversation.Count(comment => !comment.Read)),
          SortOrder = section.SortOrder,
          SectionType = section.SectionType,
          Stage = entity.Stage.Value,
          Permissions = permissions
        };
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
  public async Task<SectionFormModel> GetSectionForm<T>(int id, int sectionId) where T : CoreSectionTypeData
  {
    var fieldsResponses = await _fieldResponses.ListBySection<T>(id, sectionId);
    var section = await _sections.Get(sectionId);
    var sectionFields = await _fields.ListBySection(sectionId);

    return new SectionFormModel
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
            ? x.SelectFieldOptions.Select(option => new SelectFieldOptionModel(option)).ToList()
            : null,
          Trigger = x.TriggerCause != null && x.TriggerTarget != null
            ? new TriggerFormModel
            {
              Value = x.TriggerCause,
              Target = x.TriggerTarget.Id
            }
            : null,
          FieldResponseId = fieldsResponses.FirstOrDefault(y => y.Field.Id == x.Id)?.Id,
          FieldResponse = SerializerHelper.DeserializeOrDefault<JsonElement>(
            // direct deserialisation should work as we expect Value to be always a valid JSON string,
            // but just to ensure we correctly handle invalid JSON strings
            fieldsResponses
              .Where(y => y.Field.Id == x.Id)
              .Select(y => y.FieldResponseValues.MaxBy(z => z.ResponseDate)?.Value)
              .SingleOrDefault() ?? JsonSerializer.Serialize(x.DefaultResponse)
          ), // default response if no response found
          IsApproved = fieldsResponses.Any(y => y.Field.Id == x.Id && y.Approved),
          Comments = fieldsResponses
            .Where(y => y.Field.Id == x.Id)
            .Sum(y => y.Conversation.Count),
          UnreadComments = fieldsResponses
            .Where(y => y.Field.Id == x.Id)
            .Sum(y => y.Conversation.Count(comment => !comment.Read))
        })
        .ToList()
    };
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
      FieldResponses = await _fieldResponses.GenerateFieldResponseSubmissionModel(
        model.FieldResponses,
        model.Files,
        model.FileFieldResponses
      ),
      NewFieldResponses = await _fieldResponses.GenerateFieldResponseSubmissionModel(
        model.NewFieldResponses,
        model.NewFiles,
        model.NewFileFieldResponses,
        true
      )
    };

    var existing = await GetEntity<T>(model.RecordId);
    var fieldResponses = await _fieldResponses.ListBySection<T>(submission.RecordId, submission.SectionId);

    var updatedValues =
      existing.Stage.Value == Stages.Draft
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
      SectionTypeHelper.GetSectionTypeName<T>(),
      submission.NewFieldResponses
    );

    existing.FieldResponses.AddRange(newFieldResponses);
    await _db.SaveChangesAsync();

    return await GetSectionForm<T>(submission.RecordId, submission.SectionId);
  }

  /// <summary>
  /// Get entity by id.
  /// </summary>
  /// <param name="id">Entity id.</param>
  /// <returns>Entity.</returns>
  private async Task<T> GetEntity<T>(int id) where T : CoreSectionTypeData
    => await _db.Set<T>()
      .Where(x => x.Id == id)
      .Include(x => x.Project)
      .Include(x => x.Owner)
      .Include(x => x.Stage)
      .SingleOrDefaultAsync() ?? throw new KeyNotFoundException();

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
