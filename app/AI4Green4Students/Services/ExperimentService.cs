using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Experiment;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class ExperimentService
{
  private readonly ApplicationDbContext _db;

  public ExperimentService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<ExperimentModel> Create(CreateExperimentModel model, string ownerId)
  {
    // TODO: check if user belongs to the project group,
    // as we don't want to allow users to create experiments for other projects/groups

    var projectGroup = await _db.ProjectGroups.FindAsync(model.ProjectGroupId)
                       ?? throw new KeyNotFoundException();
    var owner = await _db.Users.FindAsync(ownerId)
                ?? throw new KeyNotFoundException();

    var entity = new Experiment
    {
      Title = model.Title,
      ProjectGroup = projectGroup,
      Owner = owner
    };

    await _db.Experiments.AddAsync(entity);
    await _db.SaveChangesAsync();
    return await GetByUser(entity.Id, ownerId);
  }

  public async Task<ExperimentModel> Set(int id, CreateExperimentModel model, string ownerId)
  {
    var entity = await _db.Experiments
                   .Where(x => x.Id == id && x.Owner.Id == ownerId)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    entity.Title = model.Title;
    await _db.SaveChangesAsync();
    return await GetByUser(id, ownerId);
  }

  public async Task<List<ExperimentModel>> ListByProject(int projectId)
  {
    var list = await _db.Experiments
      .AsNoTracking()
      .Where(x => x.ProjectGroup.Project.Id == projectId)
      .Include(x => x.Owner)
      .Include(x => x.ProjectGroup)
      .ThenInclude(x => x.Project)
      .Include(x => x.ExperimentReactions)
      .ToListAsync();
    return list.ConvertAll<ExperimentModel>(x => new ExperimentModel(x));
  }

  public async Task<List<ExperimentModel>> ListByUser(string userId, int projectId)
  {
    var user = await _db.Users.FindAsync(userId)
               ?? throw new KeyNotFoundException();
    var list = await _db.Experiments
      .AsNoTracking()
      .Where(x => x.Owner.Id == user.Id && x.ProjectGroup.Project.Id == projectId)
      .Include(x => x.Owner)
      .Include(x => x.ProjectGroup)
      .ThenInclude(x => x.Project)
      .Include(x => x.ExperimentReactions)
      .ToListAsync();
    return list.ConvertAll<ExperimentModel>(x => new ExperimentModel(x));
  }

  public async Task<ExperimentModel> Get(int experimentId)
  {
    var experiment = await _db.Experiments
                       .AsNoTracking()
                       .Where(x => x.Id == experimentId)
                       .Include(x => x.Owner)
                       .Include(x => x.ProjectGroup)
                       .ThenInclude(x => x.Project)
                       .Include(x => x.ExperimentReactions)
                       .FirstOrDefaultAsync()
                     ?? throw new KeyNotFoundException();

    return new ExperimentModel(experiment);
  }

  public async Task<ExperimentModel> GetByUser(int experimentId, string userId)
  {
    var experiment = await _db.Experiments
                       .AsNoTracking()
                       .Where(x => x.Owner.Id == userId && x.Id == experimentId)
                       .Include(x => x.Owner)
                       .Include(x => x.ProjectGroup)
                       .ThenInclude(x => x.Project)
                       .Include(x => x.ExperimentReactions)
                       .FirstOrDefaultAsync()
                     ?? throw new KeyNotFoundException();

    return new ExperimentModel(experiment);
  }

  public async Task Delete(int id, string userId)
  {
    var entity = await _db.Experiments
                   .AsNoTracking()
                   .FirstOrDefaultAsync(x => x.Id == id && x.Owner.Id == userId)
                 ?? throw new KeyNotFoundException();

    _db.Experiments.Remove(entity);
    await _db.SaveChangesAsync();
  }
}
