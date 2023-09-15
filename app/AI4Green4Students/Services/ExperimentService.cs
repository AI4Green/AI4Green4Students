using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data.Entities.Identity;
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
    var projectGroup = await _db.ProjectGroups.FindAsync(model.ProjectGroupId)
                       ?? throw new KeyNotFoundException();
    var experimentType = await _db.ExperimentTypes.FindAsync(model.ExperimentTypeId)
                          ?? throw new KeyNotFoundException();
    var experiment = new Experiment
    {
      Title = model.Title,
      ProjectGroup = projectGroup,
      ExperimentType = experimentType,
      Owner = new ApplicationUser{Id = ownerId}
    };

    _db.Experiments.Add(experiment);
    await _db.SaveChangesAsync();

    return new ExperimentModel
    {
      Id = experiment.Id,
      Title = experiment.Title
    };
  }
  
  public async Task<List<ExperimentModel>> ListByUser(string userId)
  {
    var user = await _db.Users.FindAsync(userId) 
               ?? throw new KeyNotFoundException();
    return await _db.Experiments
      .AsNoTracking()
      .Where(x => x.Owner.Id == user.Id)
      .Select(x => new ExperimentModel
      {
        Id = x.Id,
        Title = x.Title
      })
      .ToListAsync();
  }
  
  public async Task<ExperimentModel> GetByUser(int experimentId, string userId)
  {
    var experiment = await _db.Experiments
      .AsNoTracking()
      .Where(x => x.Owner.Id == userId && x.Id == experimentId)
      .FirstOrDefaultAsync()
      ?? throw new KeyNotFoundException();
    
    return new ExperimentModel
    {
      Id = experiment.Id,
      Title = experiment.Title
    };
  }

  public async Task Delete(int id)
  {
    var entity = await _db.Experiments
                   .AsNoTracking()
                   .FirstOrDefaultAsync(x=>x.Id == id)
                 ?? throw new KeyNotFoundException();
    
    _db.Experiments.Remove(entity);
    await _db.SaveChangesAsync();
  }
}
