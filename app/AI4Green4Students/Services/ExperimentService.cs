using System.Security.Authentication;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.Experiment;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class ExperimentService
{
  private readonly ApplicationDbContext _db;
  private readonly AZExperimentStorageService _azExperimentStorageService;
  
  public ExperimentService(ApplicationDbContext db, AZExperimentStorageService azExperimentStorageService)
  {
    _db = db;
    _azExperimentStorageService = azExperimentStorageService;
  }

  public async Task<ExperimentModel> Create(CreateExperimentModel model, string ownerId)
  {
    // TODO: check if user belongs to the project group,
    // as we don't want to allow users to create experiments for other projects/groups
    
    var projectGroup = await _db.ProjectGroups.FindAsync(model.ProjectGroupId)
                       ?? throw new KeyNotFoundException();
    var experimentType = await _db.ExperimentTypes.FindAsync(model.ExperimentTypeId)
                          ?? throw new KeyNotFoundException();
    var owner = await _db.Users.FindAsync(ownerId)
                 ?? throw new KeyNotFoundException();
    
    var experimentPlan = model.ToEntity();
    experimentPlan.ProjectGroup = projectGroup;
    experimentPlan.ExperimentType = experimentType;
    experimentPlan.Owner = owner;
    
    await _db.Experiments.AddAsync(experimentPlan);
    _db.Experiments.Add(experimentPlan);
    await _db.SaveChangesAsync();

    return await GetByUser(experimentPlan.Id, ownerId);
  }
  
  public async Task<ExperimentModel> Set(int id, CreateExperimentModel model, (string name, Stream data) file, string ownerId)
  {
    var existingExperimentPlan = await _db.Experiments
                                   .Where(x=> x.Id == id && x.Owner.Id == ownerId)
                                   .Include(x => x.ProjectGroup)
                                   .Include(x=>x.ExperimentType)
                                   .Include(x=>x.Owner)
                                   .SingleOrDefaultAsync() 
                                 ?? throw new KeyNotFoundException();
    
    var update = model.ToEntity();
    update.Id = existingExperimentPlan.Id;
    update.ProjectGroup = existingExperimentPlan.ProjectGroup;
    update.ExperimentType = existingExperimentPlan.ExperimentType;
    update.Owner = existingExperimentPlan.Owner;
    
    if (!string.IsNullOrEmpty(existingExperimentPlan.LiteratureFileName))
    {
      if (file.data != null && !string.IsNullOrEmpty(file.name)) // replace existing file with new one
      {
        update.LiteratureFileName = file.name;
        var newLocation = Guid.NewGuid() + Path.GetExtension(file.name);
        update.LiteratureFileLocation = await _azExperimentStorageService 
          .Replace(existingExperimentPlan.LiteratureFileLocation, newLocation, file.data);
      }

      if (file.data is null)
      {
        update.LiteratureFileName = model.IsLiteratureReviewFilePresent ? existingExperimentPlan.LiteratureFileName : string.Empty;
        update.LiteratureFileLocation = model.IsLiteratureReviewFilePresent ? existingExperimentPlan.LiteratureFileLocation : string.Empty;
        if (!model.IsLiteratureReviewFilePresent) // delete existing file
          await _azExperimentStorageService.Delete(existingExperimentPlan.LiteratureFileLocation);
      }
    }
    
    if (file.data != null && !string.IsNullOrEmpty(file.name) && string.IsNullOrEmpty(existingExperimentPlan.LiteratureFileName))
    {
      update.LiteratureFileName = file.name;
      var newLocation = Guid.NewGuid() + Path.GetExtension(file.name);
      update.LiteratureFileLocation = await _azExperimentStorageService 
        .Upload(newLocation, file.data);
    }

    _db.Entry(existingExperimentPlan).CurrentValues.SetValues(update);

    await _db.SaveChangesAsync();
    return await GetByUser(id, ownerId);
  }
  
  public async Task<List<ExperimentModel>> ListByUser(string userId)
  {
    var user = await _db.Users.FindAsync(userId) 
               ?? throw new KeyNotFoundException();
    var list = await _db.Experiments
      .AsNoTracking()
      .Where(x => x.Owner.Id == user.Id)
      .Include(x => x.ProjectGroup)
      .ThenInclude(x => x.Project)
      .ToListAsync();
    return list.ConvertAll<ExperimentModel>(x=> new ExperimentModel(x));
  }
  
  public async Task<ExperimentModel> GetByUser(int experimentId, string userId)
  {
    var experiment = await _db.Experiments
      .AsNoTracking()
      .Where(x => x.Owner.Id == userId && x.Id == experimentId)
      .Include(x => x.ProjectGroup)
      .ThenInclude(x => x.Project)
      .FirstOrDefaultAsync()
      ?? throw new KeyNotFoundException();
    
    return new ExperimentModel(experiment);
  }

  public async Task Delete(int id, string userId)
  {
    var entity = await _db.Experiments
                   .AsNoTracking()
                   .FirstOrDefaultAsync(x=>x.Id == id && x.Owner.Id == userId)
                 ?? throw new KeyNotFoundException();
    
    _db.Experiments.Remove(entity);
    await _db.SaveChangesAsync();
  }

  public async Task<byte[]> GetFileToDownload(int id, string userId, string fileName)
  {
    var experiment = await _db.Experiments
      .AsNoTracking()
      .Where(x => x.Owner.Id == userId && x.Id == id && x.LiteratureFileName == fileName)
      .FirstOrDefaultAsync() ?? throw new KeyNotFoundException();

    return await _azExperimentStorageService.Get(experiment.LiteratureFileLocation);
  }
}
