using System.Text.Json;
using System.Text.Json.Serialization;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Field;
using AI4Green4Students.Models.InputType;
using AI4Green4Students.Models.Section;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class SectionService
{
  private readonly ApplicationDbContext _db;
  private readonly AZExperimentStorageService _azStorage;
  private readonly ReportService _reports;

  public SectionService(
    ApplicationDbContext db,
    AZExperimentStorageService azStorage,
    ReportService reports)
  {
    _db = db;
    _azStorage = azStorage;
    _reports = reports;
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
      Project = await _db.Projects.SingleOrDefaultAsync(x => x.Id == model.ProjectId)
                ?? throw new KeyNotFoundException(),
      SectionType = await _db.SectionTypes.SingleOrDefaultAsync(x => x.Id == model.SectionTypeId)
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

    entity.Project = await _db.Projects.SingleOrDefaultAsync(x => x.Id == model.ProjectId)
                     ?? throw new KeyNotFoundException();
    entity.SectionType = await _db.SectionTypes.SingleOrDefaultAsync(x => x.Id == model.SectionTypeId)
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
    var report = await _reports.Get(reportId);
    return GetSummaryModel(sections, reportFieldResponses, report.Permissions, report.StageName);
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

  /// <summary>
  /// Create field responses for a given entity.
  /// </summary>
  /// <param name="entity"> Entity to create responses for.</param>
  /// <param name="fields"> Fields to create responses for.</param>
  /// <param name="fieldResponses">List of field responses to add to the field. (Optional)</param>
  public async Task CreateFieldResponses<T>(T entity, List<Field> fields, List<FieldResponseSubmissionModel>? fieldResponses)
  {
    foreach (var f in fields)
    {
      var fr = new FieldResponse { Field = f, Approved = false };
      await _db.AddAsync(fr);

      var frv = new FieldResponseValue
      {
        FieldResponse = fr,
        Value = fieldResponses?.FirstOrDefault(x => x.Id == f.Id)?.Value
                ?? JsonSerializer.Serialize(f.DefaultResponse)
      };

      await _db.AddAsync(frv);

      switch (entity)
      {
        case ProjectGroup projectGroup:
          await _db.AddAsync(new ProjectGroupFieldResponse { ProjectGroup = projectGroup, FieldResponse = fr });
          break;
        case LiteratureReview literatureReview:
          await _db.AddAsync(new LiteratureReviewFieldResponse { LiteratureReview = literatureReview, FieldResponse = fr });
          break;
        case Plan plan:
          await _db.AddAsync(new PlanFieldResponse { Plan = plan, FieldResponse = fr });
          break;
      }
    }
    await _db.SaveChangesAsync();
  }

  public List<SectionSummaryModel> GetSummaryModel(List<SectionModel> sections, List<FieldResponse> fieldsResponses, List<string> permissions, string stage)
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
        SectionType = section.SectionType,
        Stage = stage,
        Permissions = permissions
      }).OrderBy(o => o.SortOrder)
      .ToList();

  public SectionFormModel GetFormModel(SectionModel section, List<Field> sectionFields,
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
        FieldResponseId = fieldsResponses.FirstOrDefault(y => y.Field.Id == x.Id)?.Id,
        FieldResponse = DeserialiseSafely<JsonElement>(
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

  public async Task<List<Field>> GetSectionFields(int sectionId)
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

  public async Task<Section> GetSection(int sectionId)
  => await _db.Sections
       .Include(x => x.Fields)
       .ThenInclude(y => y.FieldResponses)
       .ThenInclude(z => z.ProjectGroupFieldResponses)
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

  public void UpdateDraftFieldResponses(SectionFormSubmissionModel model, IEnumerable<FieldResponse> selectedFieldResponses)
  {
    foreach(var fieldResponseValue in model.FieldResponses)
    {
      var entityToUpdate = selectedFieldResponses.SingleOrDefault(x => x.Id == fieldResponseValue.Id).FieldResponseValues.SingleOrDefault();
      entityToUpdate.Value = fieldResponseValue.Value; // expecting value to be a json string
      _db.Update(entityToUpdate);
    }
  }

  public void UpdateAwaitingChangesFieldResponses(SectionFormSubmissionModel model, IEnumerable<FieldResponse> selectedFieldResponses)
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
  /// <param name="files"> List of files to upload.</param>
  /// <param name="filesFieldResponses"> json string containing metadata for the existing files.</param>
  /// <returns></returns>
  public async Task<List<FieldResponseSubmissionModel>> GetFieldResponses(string fieldResponses, List<IFormFile> files, string filesFieldResponses)
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

  private async Task<List<FieldResponse>> GetReportFieldResponses(int reportId)
    => await _db.Reports
         .AsNoTracking()
         .Where(x => x.Id == reportId)
         .SelectMany(x => x.ReportFieldResponses
           .Select(y => y.FieldResponse))
         .Include(x => x.Conversation)
         .ToListAsync()
       ?? throw new KeyNotFoundException();
  
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
        var newFile = new FileInputTypeModel { Name = file.Name, Location = file.Location };
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
}
