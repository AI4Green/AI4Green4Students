using System.Text.Json;
using AI4Green4Students.Constants;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Field;
using AI4Green4Students.Models.Section;
using AI4Green4Students.Utilities;

namespace AI4Green4Students.Services;

/// <summary>
/// Contains methods to handle and manage section forms.
/// </summary>
public class SectionFormService
{
  private readonly SectionService _sections;
  private readonly FieldService _fields;

  public SectionFormService(SectionService sections, FieldService fields)
  {
    _sections = sections;
    _fields = fields;
  }
  
  /// <summary>
  /// Generate sections summary for a given section type entity.
  /// </summary>
  /// <param name="projectId">Project id</param>
  /// <param name="sectionType">Section type</param>
  /// <param name="fieldsResponses">Field responses</param>
  /// <param name="permissions">Permissions list</param>
  /// <param name="stage">Stage name</param>
  /// <returns>Summary model</returns>
  public async Task<List<SectionSummaryModel>> GetSummaryModel(
    int projectId,
    string sectionType,
    List<FieldResponse> fieldsResponses,
    List<string> permissions,
    string stage)
  {
    var sections = await _sections.ListBySectionTypeName(sectionType, projectId);
    
    var triggerMap = new Dictionary<int, Field>();
    fieldsResponses.ForEach(fr =>
    {
      if (fr.Field.TriggerTarget is not null) // if field has a trigger target, map child field id to parent field
        triggerMap[fr.Field.TriggerTarget.Id] = fr.Field; 
    });

    var summaries = sections.Select(section =>
      {
        // get valid field responses for the section.
        // e.g. ignore field responses that are not triggered by parent field
        // useful when determining if a section is approved or not
        var validFieldResponses = fieldsResponses
          .Where(fr => fr.Field.Section.Id == section.Id && IsFieldTriggeredByParentField(fr.Field, triggerMap));

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
          Stage = stage,
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
  /// <param name="sectionId">Section id</param>
  /// <param name="fieldsResponses">Field responses</param>
  /// <returns>Form model</returns>
  public async Task<SectionFormModel> GetFormModel(int sectionId, List<FieldResponse> fieldsResponses)
  {
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
        FieldResponseId = fieldsResponses.FirstOrDefault(y => y.Field.Id == x.Id)?.Id,
        FieldResponse = SerializerHelper.DeserializeOrDefault<JsonElement>(
          // direct deserialisation should work as we expect Value to be always a valid json string,
          // but just to ensure we correctly handle invalid json strings
          fieldsResponses
            .Where(y => y.Field.Id == x.Id)
            .Select(y => y.FieldResponseValues.MaxBy(z => z.ResponseDate)?.Value)
            .SingleOrDefault() ?? JsonSerializer.Serialize(x.DefaultResponse)), // default response if no response found
        IsApproved = fieldsResponses.Any(y => y.Field.Id == x.Id && y.Approved),
        Comments = fieldsResponses
          .Where(y => y.Field.Id == x.Id)
          .Sum(y => y.Conversation.Count(comment => !comment.Read)),
      }).ToList()
    };
  }
  
  /// <summary>
  /// Helper method to check if a field is triggered by a parent field.
  /// </summary>
  /// <param name="field">Field to check</param>
  /// <param name="childFieldsAndParentFields">Dictionary of child fields and their parent fields</param>
  /// <returns>Bool</returns>
  private bool IsFieldTriggeredByParentField(Field field, Dictionary<int, Field> childFieldsAndParentFields)
  {
    if (!childFieldsAndParentFields.TryGetValue(field.Id, out Field? parentField)) return true;
    var parentFieldResponse = parentField.FieldResponses
      .Select(x => x.FieldResponseValues.MaxBy(y => y.ResponseDate)?.Value)
      .SingleOrDefault();

    // we are checking whether parent field response value is equal to the trigger cause.
    // since field response value is always a json string, we need to deserialise it to the correct type before comparison
    switch (parentField.InputType.Name)
    {
      case InputTypes.Multiple:
      case InputTypes.Radio:
        return parentFieldResponse is not null && 
               (SerializerHelper.DeserializeOrDefault<List<SelectFieldOptionModel>>(parentFieldResponse)?
                 .Any(x => x.Name == parentField.TriggerCause) ?? false);

      default:
        // shouldn't reach here as we expect only select (Radio and Multiple) fields to have trigger cause.
        return parentFieldResponse is not null &&
               SerializerHelper.DeserializeOrDefault<string>(parentFieldResponse) == parentField.TriggerCause;
    }
  }
}
