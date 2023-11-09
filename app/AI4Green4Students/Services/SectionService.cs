using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
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

  /// <summary>
  /// Get all sections for a specific project and a specific user.
  /// </summary>
  /// <param name="projectId"></param>
  /// <param name="experimentId"></param>
  /// <returns></returns>
  public async Task<List<SectionSummaryModel>> List(int projectId, int experimentId)
    => _db.Sections.Where(x => x.Project.Id == projectId)
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
      }).OrderBy(o => o.SortOrder).ToList();

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

    return await Get(entity.Id);
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
    return await Get(id);
  }

  public async Task<SectionModel> Get(int id)
  =>
    await _db.Sections
                   .AsNoTracking()
                   .Where(x => x.Id == id)
                   .Include(section => section.Fields)
                  .ThenInclude(x => x.FieldResponses)
                  .ThenInclude(x => x.Conversation)
                  .ThenInclude(x => x.Comments)
                  .Select(x => new SectionModel
                  {
                    Id = x.Id,
                    Name = x.Name
                  })
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();
 

}
