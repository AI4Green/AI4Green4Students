using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Experiment;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class ExperimentReactionService
{
  private readonly ApplicationDbContext _db;

  public ExperimentReactionService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<List<ExperimentReactionModel>> ListByExperiment(int experimentId)
    => await _db.ExperimentReactions
      .AsNoTracking()
      .Include(x => x.Experiment)
      .ThenInclude(x => x.ProjectGroup)
      .ThenInclude(x => x.Project)
      .Include(x => x.Experiment)
      .ThenInclude(x => x.Owner)
      .Where(x => x.Experiment.Id == experimentId)
      .Select(x => new ExperimentReactionModel(x))
      .ToListAsync();

  public async Task<List<ExperimentReactionModel>> ListByUser(string userId, int experimentId)
    => await _db.ExperimentReactions
      .AsNoTracking()
      .Include(x => x.Experiment)
      .ThenInclude(x => x.ProjectGroup)
      .ThenInclude(x => x.Project)
      .Include(x => x.Experiment)
      .ThenInclude(x => x.Owner)
      .Where(x => x.Experiment.Id == experimentId && x.Experiment.Owner.Id == userId)
      .Select(x => new ExperimentReactionModel(x))
      .ToListAsync();

  public async Task<ExperimentReactionModel> Get(int id)
  {
    var result = await _db.ExperimentReactions
                   .AsNoTracking()
                   .Where(x => x.Id == id)
                   .Include(x => x.Experiment)
                   .ThenInclude(x => x.ProjectGroup)
                   .ThenInclude(x => x.Project)
                   .Include(x => x.Experiment)
                   .ThenInclude(x => x.Owner)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    return new ExperimentReactionModel(result);
  }

  public async Task<ExperimentReactionModel> GetByUser(string userId, int id)
  {
    var result = await _db.ExperimentReactions
                   .AsNoTracking()
                   .Where(x => x.Id == id && x.Experiment.Owner.Id == userId)
                   .Include(x => x.Experiment)
                   .ThenInclude(x=>x.ProjectGroup)
                   .ThenInclude(x=>x.Project)
                   .Include(x => x.Experiment)
                   .ThenInclude(x=>x.Owner)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    return new ExperimentReactionModel(result);
  }

  public async Task<ExperimentReactionModel> Create(string userId, CreateExperimentReactionModel model)
  {
    var existingExperiment = await _db.Experiments
                               .Where(x => x.Id == model.ExperimentId && x.Owner.Id == userId)
                               .Include(x => x.ExperimentReactions)
                               .FirstOrDefaultAsync()
                             ?? throw new KeyNotFoundException();

    var isExistingReaction = existingExperiment.ExperimentReactions
      .FirstOrDefault(x => x.Title.Contains(model.Title, StringComparison.OrdinalIgnoreCase));

    if (isExistingReaction is not null)
      return await Set(userId, isExistingReaction.Id, model); // Update existing experiment reaction if it exists

    // Else, create new experiment reaction
    var entity = new ExperimentReaction { Title = model.Title, Experiment = existingExperiment };

    await _db.ExperimentReactions.AddAsync(entity);
    await _db.SaveChangesAsync();

    return await Get(entity.Id);
  }

  public async Task<ExperimentReactionModel> Set(string userId, int id, CreateExperimentReactionModel model)
  {
    var entity = await _db.ExperimentReactions
                   .Where(x => x.Id == id && x.Experiment.Id == model.ExperimentId && x.Experiment.Owner.Id == userId)
                   .FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException(); // if reaction does not exist

    entity.Title = model.Title;

    _db.ExperimentReactions.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }

  public async Task Delete(int id, string userId)
  {
    var entity = await _db.ExperimentReactions
                   .AsNoTracking()
                   .FirstOrDefaultAsync(x => x.Id == id && x.Experiment.Owner.Id == userId)
                 ?? throw new KeyNotFoundException();

    _db.ExperimentReactions.Remove(entity);
    await _db.SaveChangesAsync();
  }
}
