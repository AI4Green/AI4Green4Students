namespace AI4Green4Students.Services;

using System.Text.Json;
using Constants;
using Data;
using Data.Entities;
using Data.Entities.SectionTypeData;
using Microsoft.EntityFrameworkCore;
using Models.InputType;
using Models.Section;
using Utilities;

public class FieldResponseService
{
  private static readonly List<string> _filteredFields = [InputTypes.Content, InputTypes.Header];
  private readonly AzureStorageService _azureStorageService;
  private readonly ApplicationDbContext _db;
  private readonly FieldService _fields;

  public FieldResponseService(ApplicationDbContext db, FieldService fields, AzureStorageService azureStorageService)
  {
    _db = db;
    _fields = fields;
    _azureStorageService = azureStorageService;
  }

  /// <summary>
  /// List field responses.
  /// </summary>
  /// <param name="id">Entity id. E.g Plan id.</param>
  /// <returns>Field responses.</returns>
  public async Task<List<FieldResponse>> ListBySectionType<T>(int id) where T : BaseSectionTypeData
    => await _db.Set<T>()
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
      .AsSplitQuery()
      .ToListAsync() ?? throw new KeyNotFoundException();

  /// <summary>
  /// List field responses of a section.
  /// </summary>
  /// <param name="id">Entity id. E.g Plan id.</param>
  /// <param name="sectionId">Section id.</param>
  /// <returns>Field responses.</returns>
  public async Task<List<FieldResponse>> ListBySection<T>(int id, int sectionId) where T : BaseSectionTypeData
    => await _db.Set<T>()
      .Where(x => x.Id == id && x.FieldResponses.Any(y => y.Field.Section.Id == sectionId))
      .SelectMany(x => x.FieldResponses)
      .Where(fr => !_filteredFields.Contains(fr.Field.InputType.Name))
      .Include(x => x.Field)
      .Include(x => x.FieldResponseValues)
      .Include(x => x.Conversation)
      .AsSplitQuery()
      .ToListAsync() ?? throw new KeyNotFoundException();

  /// <summary>
  /// List field responses of a field.
  /// </summary>
  /// <param name="ids">List of entity ids. E.g Plan ids.</param>
  /// <param name="fieldId">Field id.</param>
  /// <returns>Field responses.</returns>
  public async Task<List<FieldResponseModel>> ListByField<T>(List<int> ids, int fieldId) where T : BaseSectionTypeData
  {
    var fieldResponses = await _db.Set<T>()
      .Where(x => ids.Contains(x.Id))
      .Include(x => x.FieldResponses).ThenInclude(y => y.Field.InputType)
      .Include(x => x.FieldResponses).ThenInclude(y => y.Field)
      .Include(x => x.FieldResponses).ThenInclude(y => y.FieldResponseValues)
      .AsSplitQuery()
      .ToListAsync();

    return fieldResponses.SelectMany(x => x.FieldResponses
        .Where(y => y.Field.Id == fieldId)
        .Select(z => new FieldResponseModel(z, x.Id)))
      .ToList();
  }

  /// <summary>
  /// Create field responses.
  /// </summary>
  /// <param name="id">Entity id. E.g Plan id.</param>
  /// <param name="projectId">Project id.</param>
  /// <param name="fieldResponses">Create field response models.</param>
  /// <returns>Field responses.</returns>
  public async Task<List<FieldResponse>> CreateResponses<T>(
    int id,
    int projectId,
    List<CreateFieldResponseModel>? fieldResponses = null
  ) where T : BaseSectionTypeData
  {
    var fields = await _fields.ListBySectionType(SectionTypeHelper.GetSectionTypeName<T>(), projectId);
    var filteredFields = fields.Where(x => !_filteredFields.Contains(x.InputType.Name)).ToList();

    var existingFieldResponseIds = await _db.Set<T>()
      .Where(x => x.Id == id)
      .SelectMany(x => x.FieldResponses)
      .Select(fr => fr.Field.Id)
      .ToListAsync();

    var newFieldResponses = new List<FieldResponse>();
    foreach (var f in filteredFields)
    {
      if (existingFieldResponseIds.Contains(f.Id))
      {
        continue;
      }

      var value = fieldResponses?.FirstOrDefault(x => x.Id == f.Id)?.Value
                ?? JsonSerializer.Serialize(f.DefaultResponse);

      newFieldResponses.Add(await Create(f, value));
    }

    await _db.SaveChangesAsync();
    return newFieldResponses;
  }

  /// <summary>
  /// Create field response.
  /// </summary>
  /// <param name="field">Field entity.</param>
  /// <param name="value">Response value for the field.</param>
  public async Task<FieldResponse> Create(Field field, string value)
  {
    var fr = new FieldResponse
    {
      Field = field,
      Approved = false
    };
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
  /// <param name="updates">Updates for existing field responses.</param>
  /// <param name="existing">Existing field responses</param>
  /// <returns>Updated draft field responses</returns>
  public List<FieldResponseValue> UpdateDraft(List<CreateFieldResponseModel> updates, List<FieldResponse> existing)
  {
    var updatedValues = new List<FieldResponseValue>();
    foreach (var update in updates)
    {
      var existingResponse = existing.SingleOrDefault(x => x.Id == update.Id)?.FieldResponseValues.SingleOrDefault();
      if (existingResponse is null)
      {
        continue;
      }

      existingResponse.Value = update.Value;
      updatedValues.Add(existingResponse);
    }

    return updatedValues;
  }

  /// <summary>
  /// Generate updated awaiting changes field responses.
  /// </summary>
  /// <param name="updates">Updates for existing field responses.</param>
  /// <param name="existing">Existing field responses.</param>
  /// <returns>Updated awaiting changes field responses.</returns>
  public List<FieldResponseValue> UpdateAwaitingChanges(
    List<CreateFieldResponseModel> updates,
    List<FieldResponse> existing
  )
  {
    var updatedValues = new List<FieldResponseValue>();
    foreach (var update in updates)
    {
      var existingResponse = existing.SingleOrDefault(x => x.Id == update.Id && !x.Approved)
        ?.FieldResponseValues.OrderByDescending(x => x.ResponseDate)
        .FirstOrDefault();

      if (existingResponse is null)
      {
        continue;
      }

      existingResponse.Value = update.Value;
      updatedValues.Add(existingResponse);
    }

    return updatedValues;
  }

  /// <summary>
  /// Create field response models from a json string.
  /// </summary>
  /// <param name="regularFieldResponsesJson">JSON string containing section field responses.</param>
  /// <param name="uploadFiles">List of files to upload.</param>
  /// <param name="filesFieldResponsesJson">JSON string containing metadata for the existing files.</param>
  /// <param name="isNew">Bool to indicate if the field responses are new or existing.</param>
  /// <returns>Models for field responses.</returns>
  public async Task<List<CreateFieldResponseModel>> CreateFieldResponseModels(
    string regularFieldResponsesJson,
    List<IFormFile> uploadFiles,
    string filesFieldResponsesJson,
    bool isNew = false
  )
  {
    var regularResponses =
      SerializerHelper.DeserializeOrDefault<List<FieldResponseJsonModel>>(regularFieldResponsesJson) ?? [];
    var regularFieldResponses = regularResponses.Select(x =>
        new CreateFieldResponseModel(
          x.Id,
          x.Value.GetRawText()
        ))
      .ToList();

    var fileResponses = await ProcessFileFieldResponses(filesFieldResponsesJson, uploadFiles, isNew);

    return regularFieldResponses.Concat(fileResponses).ToList();
  }

  /// <summary>
  /// Process file field responses and handle file uploads.
  /// </summary>
  private async Task<List<CreateFieldResponseModel>> ProcessFileFieldResponses(
    string filesFieldResponsesJson,
    List<IFormFile> uploadFiles,
    bool isNew
  )
  {
    var fileInputTypeList = new Dictionary<int, List<FileInputTypeModel>>();
    var reactionSchemeInputTypeList = new Dictionary<int, ReactionSchemeInputTypeModel>();

    var metadata = SerializerHelper.DeserializeOrDefault<List<FieldResponseJsonModel>>(filesFieldResponsesJson) ?? [];

    for (var i = 0; i < metadata.Count; i++)
    {
      var item = metadata[i];
      var field = isNew ? await _fields.Get(item.Id) : await _fields.GetByFieldResponse(item.Id);

      var file = i < uploadFiles.Count ? uploadFiles[i] : null;

      switch (field.FieldType)
      {
        case InputTypes.File:
        case InputTypes.ImageFile:
          if (file is null)
          {
            continue;
          }

          var fileResponse = await ProcessFileInputType(item, file);
          if (fileResponse is not null)
          {
            if (!fileInputTypeList.TryGetValue(item.Id, out var fileList))
            {
              fileList = new List<FileInputTypeModel>();
              fileInputTypeList[item.Id] = fileList;
            }

            fileList.Add(fileResponse);
          }
          break;

        case InputTypes.ReactionScheme:
          if (file is null)
          {
            continue;
          }

          var reactionSchemeResponse = await ProcessReactionSchemeInputType(item, file);
          if (reactionSchemeResponse is not null)
          {
            reactionSchemeInputTypeList[item.Id] = reactionSchemeResponse;
          }
          break;
      }
    }

    var result = fileInputTypeList.Select(x => new CreateFieldResponseModel(
      x.Key,
      JsonSerializer.Serialize(x.Value, DefaultJsonOptions.Serializer)
    )).ToList();

    result.AddRange(reactionSchemeInputTypeList.Select(x =>
      new CreateFieldResponseModel(
        x.Key,
        JsonSerializer.Serialize(x.Value, DefaultJsonOptions.Serializer)
      ))
    );

    return result;
  }

  /// <summary>
  /// Process file input type field response.
  /// </summary>
  private async Task<FileInputTypeModel?> ProcessFileInputType(FieldResponseJsonModel item, IFormFile file)
  {
    var fileModel = SerializerHelper.DeserializeOrDefault<FileInputTypeModel>(item.Value.GetRawText());
    if (fileModel is null)
    {
      return null;
    }

    if (fileModel.IsMarkedForDeletion == true)
    {
      await _azureStorageService.Delete(fileModel.Location);
      return null;
    }

    if (fileModel.IsNew != true)
    {
      return fileModel;
    }

    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(fileModel.Name)}";
    var location = await _azureStorageService.Upload(fileName, file.OpenReadStream());

    var newFileModel = new FileInputTypeModel
    {
      Name = fileModel.Name,
      Location = location,
      Caption = fileModel.Caption
    };

    return newFileModel;
  }

  /// <summary>
  /// Process reaction scheme input type field response.
  /// </summary>
  private async Task<ReactionSchemeInputTypeModel?> ProcessReactionSchemeInputType(
    FieldResponseJsonModel json,
    IFormFile file
  )
  {
    var model = SerializerHelper.DeserializeOrDefault<ReactionSchemeInputTypeModel>(json.Value.GetRawText());
    if (model is null)
    {
      return null;
    }

    var reactionImage = model.ReactionSketch.ReactionImage;
    if (reactionImage is null)
    {
      return model;
    }

    if (reactionImage.IsMarkedForDeletion == true)
    {
      await _azureStorageService.Delete(reactionImage.Location);
      return null;
    }

    var updatedLocation = await HandleReactionImageUpload(reactionImage, file);
    var updatedReactionModel = new ReactionSchemeInputTypeModel
    {
      ReactionTable = model.ReactionTable,
      ReactionSketch = new ReactionSketchModel
      {
        ReactionImage = new FileInputTypeModel
        {
          Name = $"Reaction_{updatedLocation}",
          Location = updatedLocation
        },
        SketcherSmiles = model.ReactionSketch.SketcherSmiles,
        Reactants = model.ReactionSketch.Reactants,
        Products = model.ReactionSketch.Products,
        Smiles = model.ReactionSketch.Smiles,
        Data = model.ReactionSketch.Data
      }
    };

    return updatedReactionModel;
  }

  /// <summary>
  /// Handle reaction image upload or replacement.
  /// </summary>
  private async Task<string> HandleReactionImageUpload(FileInputTypeModel image, IFormFile file)
  {
    if (image.IsNew == true)
    {
      var fileName = $"{Guid.NewGuid()}.png";
      return await _azureStorageService.Upload(fileName, file.OpenReadStream());
    }

    if (file.Length > 0)
    {
      return await _azureStorageService.Replace(image.Location, image.Location, file.OpenReadStream());
    }

    return image.Location;
  }
}
