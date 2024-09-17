using System.Text.Json;
using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data.Entities.SectionTypeData;
using AI4Green4Students.Models.InputType;
using AI4Green4Students.Models.Section;
using AI4Green4Students.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class FieldResponseService
{
  private readonly ApplicationDbContext _db;
  private readonly FieldService _fields;
  private readonly AzureStorageService _azureStorageService;
  private static readonly List<string> _filteredFields = [InputTypes.Content, InputTypes.Header];

  public FieldResponseService(ApplicationDbContext db, FieldService fields, AzureStorageService azureStorageService)
  {
    _db = db;
    _fields = fields;
    _azureStorageService = azureStorageService;
  }


  /// <summary>
  /// Get a list of field responses for a given section type entity.
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
  public async Task<FieldResponseModel> GetByField<T>(int id, int fieldId) where T : BaseSectionTypeData
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
      Value = SerializerHelper.DeserializeOrDefault<JsonElement>(
        // direct deserialization should work as we expect Value to be always a valid json string,
        // but just to ensure we correctly handle invalid json strings
        fieldResponse.FieldResponseValues.MaxBy(x => x.ResponseDate)?.Value
        ?? JsonSerializer.Serialize(fieldResponse.Field.DefaultResponse))
    };
  }

  /// <summary>
  /// Create field responses for a given entity.
  /// </summary>
  /// <param name="id">Id of the entity to create field responses for. E.g Plan id</param>
  /// <param name="projectId">Project id. Ensures only fields associated with the project and section type are returned </param>
  /// <param name="sectionType">Section type name (e.g.. Plan, Note_</param>
  /// <param name="fieldResponses">List of field responses to add to the field. (Optional)</param>
  public async Task<List<FieldResponse>> CreateResponses<T>(int id, int projectId, string sectionType,
    List<FieldResponseSubmissionModel>? fieldResponses) where T : BaseSectionTypeData
  {
    var fields = await _fields.ListBySectionType(sectionType, projectId);
    var filteredFields = fields.Where(x => !_filteredFields.Contains(x.InputType.Name)).ToList();

    var existingFieldResponseIds = await _db.Set<T>()
      .Where(x => x.Id == id).SelectMany(x => x.FieldResponses).Select(fr => fr.Field.Id)
      .ToListAsync();

    var newFieldResponses = new List<FieldResponse>();
    foreach (var f in filteredFields)
    {
      if (existingFieldResponseIds.Contains(f.Id)) continue;

      var value = fieldResponses?.FirstOrDefault(x => x.Id == f.Id)?.Value
                  ?? JsonSerializer.Serialize(f.DefaultResponse);
      var fr = await Create(f, value);

      newFieldResponses.Add(fr);
    }

    await _db.SaveChangesAsync();
    return newFieldResponses;
  }

  /// <summary>
  /// Create field response for a given field
  /// </summary>
  /// <param name="field">Field to create field response</param>
  /// <param name="value">Response value for the field.</param>
  public async Task<FieldResponse> Create(Field field, string value)
  {
    var fr = new FieldResponse { Field = field, Approved = false };
    await _db.AddAsync(fr);

    var frv = new FieldResponseValue
    {
      FieldResponse = fr,
      Value = value
    };
    await _db.AddAsync(frv);
    return fr;
  }

  /// <summary>
  /// Generate updated draft field responses.
  /// </summary>
  /// <param name="fieldResponses">New field responses</param>
  /// <param name="selectedFieldResponses">Existing field responses</param>
  /// <returns>Updated draft field responses</returns>
  public List<FieldResponseValue> UpdateDraft(List<FieldResponseSubmissionModel> fieldResponses,
    List<FieldResponse> selectedFieldResponses)
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
  public List<FieldResponseValue> UpdateAwaitingChanges(List<FieldResponseSubmissionModel> fieldResponses,
    List<FieldResponse> selectedFieldResponses)
  {
    var updatedFieldResponseValues = new List<FieldResponseValue>();
    foreach (var fieldResponseValue in fieldResponses)
    {
      var entityToUpdate = selectedFieldResponses
        .SingleOrDefault(x => x.Id == fieldResponseValue.Id && x.Approved == false)
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
  /// <param name="isNew">Bool to indicate if the field responses are new or existing.</param>
  /// <returns></returns>
  public async Task<List<FieldResponseSubmissionModel>> GenerateFieldResponseSubmissionModel(string fieldResponses,
    List<IFormFile> files, string filesFieldResponses, bool isNew = false)
  {
    var initialFieldResponses =
      SerializerHelper.DeserializeOrDefault<List<FieldResponseHelperModel>>(fieldResponses) ?? [];
    var filesMetadata = await GenerateFieldResponseWithFileSubmissionModel(filesFieldResponses, files, isNew);

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
  /// <param name="isNew">Bool to indicate if the field responses are new or existing.</param>
  /// <remarks>Each file corresponds to a field response.</remarks>
  /// <returns></returns>
  private async Task<List<FieldResponseSubmissionModel>> GenerateFieldResponseWithFileSubmissionModel(
    string filesFieldResponses, List<IFormFile> files, bool isNew)
  {
    var metadata = SerializerHelper.DeserializeOrDefault<List<FieldResponseHelperModel>>(filesFieldResponses) ?? [];

    var fileInputTypeList = new Dictionary<int, List<FileInputTypeModel>>();
    var reactionSchemeInputTypeList = new Dictionary<int, ReactionSchemeInputTypeModel>();

    foreach (var (item, index) in metadata.Select((item, index) => (item, index)))
    {
      var field = isNew ? await _fields.Get(item.Id) : await _fields.GetByFieldResponse(item.Id);
      switch (field.FieldType)
      {
        case InputTypes.File:
        case InputTypes.ImageFile:
        {
          var frv = SerializerHelper.DeserializeOrDefault<FileInputTypeModel>(item.Value.GetRawText());
          if (frv is null) break;

          if (!fileInputTypeList.ContainsKey(item.Id)) fileInputTypeList[item.Id] = new List<FileInputTypeModel>();

          if (frv.IsMarkedForDeletion is not null && frv.IsMarkedForDeletion == true)
          {
            await _azureStorageService.Delete(frv.Location);
            break; // delete if marked for deletion
          }

          if (frv.IsNew is not null && frv.IsNew == true)
          {
            frv.Location = await _azureStorageService.Upload(Guid.NewGuid() + Path.GetExtension(frv.Name),
              files[index].OpenReadStream());
            fileInputTypeList[item.Id].Add(new FileInputTypeModel
            {
              Name = frv.Name, Location = frv.Location, Caption = frv.Caption
            });
            break;
          }

          fileInputTypeList[item.Id].Add(frv);
          break;
        }

        case InputTypes.ReactionScheme:
        {
          var frv = SerializerHelper.DeserializeOrDefault<ReactionSchemeInputTypeModel>(item.Value.GetRawText());
          if (frv is null) break;

          var reactionImage = frv.ReactionSketch.ReactionImage;

          if (!reactionSchemeInputTypeList.ContainsKey(item.Id))
            reactionSchemeInputTypeList[item.Id] = new ReactionSchemeInputTypeModel();

          if (reactionImage is null)
          {
            reactionSchemeInputTypeList[item.Id] = frv;
            break;
          }

          if (reactionImage.IsMarkedForDeletion == true)
          {
            await _azureStorageService.Delete(reactionImage.Location);
            break;
          }

          var guid = Guid.NewGuid();
          var fileStream = files[index].OpenReadStream();
          
          reactionImage.Location = reactionImage.IsNew ?? false
            ? await _azureStorageService.Upload(guid + ".png", fileStream)
            : fileStream.Length > 0
              ? await _azureStorageService.Replace(reactionImage.Location, reactionImage.Location,fileStream)
              : reactionImage.Location;

          reactionSchemeInputTypeList[item.Id] = new ReactionSchemeInputTypeModel
          {
            ReactionTable = frv.ReactionTable,
            ReactionSketch = new ReactionSketchModel
            {
              ReactionImage = new FileInputTypeModel
              {
                Name = "Reaction_" + reactionImage.Location,
                Location = reactionImage.Location
              },
              SketcherSmiles = frv.ReactionSketch.SketcherSmiles,
              Reactants = frv.ReactionSketch.Reactants,
              Products = frv.ReactionSketch.Products,
              Smiles = frv.ReactionSketch.Smiles,
              Data = frv.ReactionSketch.Data
            }
          };
          break;
        }
      }
    }

    var result = fileInputTypeList.Select(x => new FieldResponseSubmissionModel
    {
      Id = x.Key,
      Value = JsonSerializer.Serialize(x.Value, DefaultJsonOptions.Serializer)
    }).ToList();

    result.AddRange(reactionSchemeInputTypeList.Select(x => new FieldResponseSubmissionModel
    {
      Id = x.Key,
      Value = JsonSerializer.Serialize(x.Value, DefaultJsonOptions.Serializer)
    }));

    return result;
  }
}
