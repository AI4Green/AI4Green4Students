using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Field;
using AI4Green4Students.Models.Section;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class SectionService
{
  private readonly ApplicationDbContext _db;

  public SectionService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<List<SectionModel>> List()
    => await _db.Sections.AsNoTracking().Select(x => new SectionModel
    {
      Id = x.Id,
      Name = x.Name
    }).ToListAsync();


  /// <summary>
  /// Get all sections for a specific project and a specific user.
  /// </summary>
  /// <param name="experimentId"></param>
  /// <returns></returns>
  public async Task<List<SectionSummaryModel>> ListSummaries(int experimentId)
  {
    var experiment = _db.Experiments.AsNoTracking().Where(x => x.Id == experimentId)
      .Include(e => e.ProjectGroup)
      .ThenInclude(pg => pg.Project).SingleOrDefault() ?? throw new KeyNotFoundException();

    return await _db.Sections.Where(x => x.Project.Id == experiment.ProjectGroup.Project.Id)
      .Include(section => section.Fields)
      .ThenInclude(x => x.FieldResponses)
      .ThenInclude(x => x.Conversation)
      .ThenInclude(x => x.Comments)
      .Select(x => new SectionSummaryModel
      {
        Id = x.Id,
        Name = x.Name,
        Approved = x.Fields.SelectMany(field => field.FieldResponses).All(fr => fr.Approved == true),
        Comments = x.Fields.SelectMany(field => field.FieldResponses)
          .Where(x => x.Experiment.Id == experimentId)
          .Select(fieldResponse => fieldResponse.Conversation)
          .Sum(conversation => conversation.Comments.Count),
        SortOrder = x.SortOrder
      }).OrderBy(o => o.SortOrder).ToListAsync();
  }

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
                ?? throw new KeyNotFoundException()
    };

    await _db.Sections.AddAsync(entity);
    await _db.SaveChangesAsync();

    return await GetModel(entity.Id);
  }

  public async Task<SectionModel> Set(int id, CreateSectionModel model)
  {
    var entity = await _db.Sections
                   .Where(x => x.Id == id)
                   .FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException(); // if section does not exist

    entity.Name = model.Name;

    _db.Sections.Update(entity);
    await _db.SaveChangesAsync();
    return await GetModel(id);
  }

  public async Task<SectionModel> GetModel(int id)
    =>
      await _db.Sections
        .AsNoTracking()
        .Where(x => x.Id == id)
        .Select(x => new SectionModel
        {
          Id = x.Id,
          Name = x.Name
        })
        .SingleOrDefaultAsync()
      ?? throw new KeyNotFoundException();


  public async Task<SectionFormModel> GetFormModel(int sectionId, int experimentId)
    => await _db.Sections.Where(x => x.Id == sectionId)
         .Include(section => section.Fields)
         .ThenInclude(fields => fields.FieldResponses)
         .ThenInclude(fieldResponses => fieldResponses.Experiment)
         .Include(section => section.Fields)
         .ThenInclude(fields => fields.InputType)
         .Include(section => section.Fields)
         .ThenInclude(fields => fields.SelectFieldOptions)
         .Select(x =>
           new SectionFormModel
           {
             Id = x.Id,
             Name = x.Name,
             FieldResponses = x.Fields.Select(y => new FieldResponseFormModel
             {
               Id = y.Id,
               Name = y.Name,
               Mandatory = y.Mandatory,
               Hidden = y.Hidden,
               SortOrder = y.SortOrder,
               FieldType = y.InputType.Name,
               DefaultResponse = y.DefaultResponse,
               SelectFieldOptions = y.SelectFieldOptions.Count >= 1
                 ? y.SelectFieldOptions
                   .Select(option => new SelectFieldOptionModel(option))
                   .ToList()
                 : null,
               Trigger = (y.TriggerCause != null && y.TriggerTarget != null)
                 ? new TriggerFormModel
                 {
                   Value = y.TriggerCause,
                   Target = y.TriggerTarget.Id
                 }
                 : null,
               FieldResponse = y.FieldResponses.Single(z => z.Experiment.Id == experimentId).FieldResponseValues
                 .OrderByDescending(fr => fr.ResponseDate).FirstOrDefault().Value
             }).ToList()
           }).SingleAsync()
       ?? throw new KeyNotFoundException();
}
