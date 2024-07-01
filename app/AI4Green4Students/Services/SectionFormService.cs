using System.Text.Json;
using System.Text.Json.Serialization;
using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data.Entities.SectionTypeData;
using AI4Green4Students.Models.Field;
using AI4Green4Students.Models.InputType;
using AI4Green4Students.Models.Section;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

/// <summary>
/// Contains methods to handle and manage section forms.
/// </summary>

public class SectionFormService
{
  private readonly ApplicationDbContext _db;
  private readonly SectionService _sections;
  private readonly FieldService _fields;
  private readonly AZExperimentStorageService _azStorage;
  private static readonly List<string> _filteredFields = [InputTypes.Content, InputTypes.Header];

  public SectionFormService (ApplicationDbContext db, SectionService sections, FieldService fields, AZExperimentStorageService azStorage)
  {
    _db = db;
    _sections = sections;
    _fields = fields;
    _azStorage = azStorage;
  }
  
  /// <summary>
  /// Generate sections summary for a given section type entity.
  /// </summary>
  /// <param name="sectionTypeId">Section type id</param>
  /// <param name="fieldsResponses">Field responses</param>
  /// <param name="permissions">Permissions list</param>
  /// <param name="stage">Stage name</param>
  /// <returns>Summary model</returns>
  public async Task<List<SectionSummaryModel>> GetSummaryModel(
    int sectionTypeId, 
    List<FieldResponse> fieldsResponses,
    List<string> permissions,
    string stage)
  {
    var sections = await _sections.ListBySectionType(sectionTypeId);
    
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
        FieldResponse = DeserialiseSafely<JsonElement>(
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
  /// Create field responses for a given entity.
  /// </summary>
  /// <param name="id">Id of the entity to create field responses for. E.g Plan id</param>
  /// <param name="projectId">Project id. Ensures only fields associated with the project and section type are returned </param>
  /// <param name="sectionType">Section type name (e.g.. Plan, Note_</param>
  /// <param name="fieldResponses">List of field responses to add to the field. (Optional)</param>
  public async Task<List<FieldResponse>> CreateFieldResponse<T>(int id, int projectId, string sectionType,
    List<FieldResponseSubmissionModel>? fieldResponses) where T : BaseSectionTypeData
  {
    var fields = await _fields.ListBySectionType(sectionType, projectId);
    var filteredFields = fields.Where(x => !_filteredFields.Contains(x.InputType.Name)).ToList();

    var newFieldResponses = new List<FieldResponse>();
    foreach (var f in filteredFields)
    {
      var fieldResponseExists = await _db.Set<T>()
        .Where(x => x.Id == id).SelectMany(x => x.FieldResponses).AnyAsync(fr => fr.Field.Id == f.Id);
      
      if (fieldResponseExists) continue;
      
      var fr = new FieldResponse { Field = f, Approved = false };
      await _db.AddAsync(fr);

      var frv = new FieldResponseValue
      {
        FieldResponse = fr,
        Value = fieldResponses?.FirstOrDefault(x => x.Id == f.Id)?.Value
                ?? JsonSerializer.Serialize(f.DefaultResponse)
      };
      await _db.AddAsync(frv);
      newFieldResponses.Add(fr);
    }
    await _db.SaveChangesAsync();
    return newFieldResponses;
  }
  
  /// <summary>
  /// Get a list of field responses for a given section type entity.
  /// TODO: Only include fields from a given project.
  /// </summary>
  /// <param name="id">Id of the section type entity to get field responses for.</param>
  /// <returns>List of field responses </returns>
  public async Task<List<FieldResponse>> ListBySectionType<T>(int id) where T : BaseSectionTypeData
  {
    return await _db.Set<T>()
             .AsNoTracking()
             .Where(x => x.Id == id)
             .SelectMany(x => x.FieldResponses)
             .Where(fr => !_filteredFields.Contains(fr.Field.InputType.Name))
             .Include(x => x.Field.InputType)
             .Include(x => x.FieldResponseValues)
             .Include(x => x.Field)
             .ThenInclude(x => x.Section)
             .Include(x => x.Field)
             .ThenInclude(x => x.TriggerTarget)
             .Include(x => x.Conversation)
             .ToListAsync()
           ?? throw new KeyNotFoundException();
  }
  
  /// <summary>
  /// Get a list of field responses for a given section from a section type.
  /// TODO: Only include fields from a given project.
  /// </summary>
  /// <param name="id">Id of the section type.</param>
  /// <param name="sectionId">Id of the section.</param>
  /// <returns>List of field responses </returns>
  public async Task<List<FieldResponse>> ListBySection<T>(int id, int sectionId) where T : BaseSectionTypeData
  {
    return await _db.Set<T>()
             .Where(x => x.Id == id && x.FieldResponses.Any(y => y.Field.Section.Id == sectionId))
             .SelectMany(x => x.FieldResponses)
             .Where(fr => !_filteredFields.Contains(fr.Field.InputType.Name))
             .Include(x => x.Field)
             .Include(x => x.FieldResponseValues)
             .Include(x => x.Conversation)
             .ToListAsync()
           ?? throw new KeyNotFoundException();
  }
  
  /// <summary>
  /// Get field response for a field from a record. (e.g Plan, Note).
  /// Get the latest field response value.
  /// </summary>
  /// <param name="id"> E.g Plan id</param>
  /// <param name="fieldId">Field id to get response value for</param>
  /// <returns>Field response model</returns>
  public async Task<FieldResponseModel> GetFieldResponse<T>(int id, int fieldId) where T : BaseSectionTypeData
  {
    var fieldResponse = await _db.Set<T>()
                          .Where(x => x.Id == id)
                          .SelectMany(x => x.FieldResponses)
                          .Where(fr => fr.Field.Id == fieldId)
                          .Include(x => x.Field.InputType)
                          .Include(x => x.Field)
                          .Include(x => x.FieldResponseValues)
                          .SingleOrDefaultAsync()
                        ?? throw new KeyNotFoundException();

    return new FieldResponseModel(fieldResponse)
    {
      Value = DeserialiseSafely<JsonElement>(
        // direct deserialisation should work as we expect Value to be always a valid json string,
        // but just to ensure we correctly handle invalid json strings
        fieldResponse.FieldResponseValues.MaxBy(x => x.ResponseDate)?.Value
        ?? JsonSerializer.Serialize(fieldResponse.Field.DefaultResponse))
    };
  }
  
  /// <summary>
  /// Generate updated draft field responses.
  /// </summary>
  /// <param name="fieldResponses">New field responses</param>
  /// <param name="selectedFieldResponses">Existing field responses</param>
  /// <returns>Updated draft field responses</returns>
  public List<FieldResponseValue> UpdateDraftFieldResponses(List<FieldResponseSubmissionModel> fieldResponses, List<FieldResponse> selectedFieldResponses)
  {
    var updatedFieldResponseValues = new List<FieldResponseValue>();
    foreach (var fieldResponseValue in fieldResponses)
    {
      var entityToUpdate = selectedFieldResponses.SingleOrDefault(x => x.Id == fieldResponseValue.Id)
        ?.FieldResponseValues.SingleOrDefault();

      if (entityToUpdate is null) continue;

      entityToUpdate.Value = fieldResponseValue.Value; // expecting value to be a json string
      updatedFieldResponseValues.Add(entityToUpdate);
    }

    return updatedFieldResponseValues;
  }

  /// <summary>
  /// Generate updated awaiting changes field responses.
  /// </summary>
  /// <param name="fieldResponses">New field responses</param>
  /// <param name="selectedFieldResponses">Existing field responses</param>
  /// <returns>Updated awaiting changes field responses</returns>
  public List<FieldResponseValue> UpdateAwaitingChangesFieldResponses(List<FieldResponseSubmissionModel> fieldResponses, List<FieldResponse> selectedFieldResponses)
  {
    var updatedFieldResponseValues = new List<FieldResponseValue>();
    foreach (var fieldResponseValue in fieldResponses)
    {
      var entityToUpdate = selectedFieldResponses.SingleOrDefault(x => x.Id == fieldResponseValue.Id && x.Approved == false)
        ?.FieldResponseValues.OrderByDescending(x => x.ResponseDate).FirstOrDefault();

      if (entityToUpdate is null) continue;

      entityToUpdate.Value = fieldResponseValue.Value;
      updatedFieldResponseValues.Add(entityToUpdate);
    }

    return updatedFieldResponseValues;
  }
  
  
  /// <summary>
  /// Transform json string into a list of FieldResponseSubmissionModel,
  /// but also keep each field response value as json string.
  /// </summary>
  /// <param name="fieldResponses"> json string containing section field responses.</param>
  /// <param name="files"> List of files to upload.</param>
  /// <param name="filesFieldResponses"> json string containing metadata for the existing files.</param>
  /// <returns></returns>
  public async Task<List<FieldResponseSubmissionModel>> GenerateFieldResponses(string fieldResponses, List<IFormFile> files, string filesFieldResponses)
  {
    var initialFieldResponses = DeserialiseSafely<List<FieldResponseHelperModel>>(fieldResponses) ?? [];
    var filesMetadata = await GenerateFileFieldResponses(filesFieldResponses, files);

    var responses = new List<FieldResponseSubmissionModel>();

    responses.AddRange(initialFieldResponses
      .Select(item => new FieldResponseSubmissionModel
      {
        Id = item.Id,
        Value = item.Value.GetRawText() // keep value json string
      }).ToList());

    responses.AddRange(filesMetadata); // add file metadata

    return responses;
  }
  
  /// <summary>
  /// Generate FieldResponseSubmissionModel list using the current files metadata and the files to upload.
  /// </summary>
  /// <param name="filesFieldResponses"> json string containing file type field responses.</param>
  /// <param name="files"> List of files.</param>
  /// <remarks>Each file corresponds to a field response.</remarks>
  /// <returns></returns>
  private async Task<List<FieldResponseSubmissionModel>> GenerateFileFieldResponses(string filesFieldResponses, List<IFormFile> files)
  {
    var metadata = DeserialiseSafely<List<FieldResponseHelperModel>>(filesFieldResponses) ?? [];

    var list = new Dictionary<int, List<FileInputTypeModel>>();
    
    for (var i = 0; i < metadata.Count; i++)
    {
      var item = metadata[i];
      var file = DeserialiseSafely<FileInputTypeModel>(item.Value.GetRawText());
      if (file is null) continue;
      
      if (!list.ContainsKey(item.Id)) list[item.Id] = new List<FileInputTypeModel>();
      
      if (file.IsMarkedForDeletion is not null && file.IsMarkedForDeletion == true)
      {
        await _azStorage.Delete(file.Location); continue; // delete if marked for deletion
      }

      if (file.IsNew is not null && file.IsNew == true)
      {
        file.Location = await _azStorage.Upload(Guid.NewGuid() + Path.GetExtension(file.Name), files[i].OpenReadStream());
        var newFile = new FileInputTypeModel { Name = file.Name, Location = file.Location, Caption = file.Caption};
        list[item.Id].Add(newFile); continue; // upload and capture blob name for new file
      }
      list[item.Id].Add(file); // add existing file as it is
    }
    
    return list.Select(x=> new FieldResponseSubmissionModel
    {
      Id = x.Key,
      Value = JsonSerializer.Serialize(x.Value, new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, 
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
      })
    }).ToList();
  }
  
  /// <summary>
  /// Deserialise a json string. Ensures only valid json strings are deserialised.
  /// </summary>
  /// <param name="jsonString"> json string to deserialise </param>
  /// <returns> deserialised json element or null if invalid or empty </returns>
  private T? DeserialiseSafely<T>(string jsonString)
  {
    if (string.IsNullOrWhiteSpace(jsonString)) return default;

    var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true , DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull};
    try
    {
      return JsonSerializer.Deserialize<T>(jsonString, jsonSerializerOptions);
    }
    catch (JsonException)
    {
      try
      {
        using var doc = JsonDocument.Parse($"\"{jsonString}\"");
        var jsonElement = doc.RootElement.Clone();
        return JsonSerializer.Deserialize<T>(jsonElement.GetRawText(), jsonSerializerOptions);
      }
      catch (JsonException)
      {
      }
    }
    return default;
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
               (DeserialiseSafely<List<SelectFieldOptionModel>>(parentFieldResponse)?
                 .Any(x => x.Name == parentField.TriggerCause) ?? false);

      default:
        // shouldn't reach here as we expect only select (Radio and Multiple) fields to have trigger cause.
        return parentFieldResponse is not null &&
               DeserialiseSafely<string>(parentFieldResponse) == parentField.TriggerCause;
    }
  }
}
