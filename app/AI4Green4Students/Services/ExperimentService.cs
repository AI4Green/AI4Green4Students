using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
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
    await _db.SaveChangesAsync();

    //TODO If any fields have any default values we need to now create field responses for them with those default values.

    return await GetByUser(experimentPlan.Id, ownerId);
  }

  public async Task<ExperimentModel> Set(int id, CreateExperimentModel model, (string name, Stream data) file,
    string ownerId)
  {
    var existingExperimentPlan = await _db.Experiments
                                   .Where(x => x.Id == id && x.Owner.Id == ownerId)
                                   .Include(x => x.ProjectGroup)
                                   .Include(x => x.ExperimentType)
                                   .Include(x => x.Owner)
                                   .SingleOrDefaultAsync()
                                 ?? throw new KeyNotFoundException();

    var updatedExperimentPlan = model.ToUpdateEntity(existingExperimentPlan);

    if (!string.IsNullOrEmpty(existingExperimentPlan.LiteratureFileName)) // based on existing file status
    {
      switch (model.IsLiteratureReviewFilePresent)
      {
        case true when !string.IsNullOrEmpty(file.name): // replace existing file with new one
        {
          updatedExperimentPlan.LiteratureFileName = file.name;
          var newLocation = Guid.NewGuid() + Path.GetExtension(file.name);
          updatedExperimentPlan.LiteratureFileLocation = await _azExperimentStorageService
            .Replace(existingExperimentPlan.LiteratureFileLocation, newLocation, file.data);
          break;
        }
        case false: // delete existing file
          await _azExperimentStorageService.Delete(existingExperimentPlan.LiteratureFileLocation);
          updatedExperimentPlan.LiteratureFileName = string.Empty;
          updatedExperimentPlan.LiteratureFileLocation = string.Empty;
          break;
      }

      if (model.IsLiteratureReviewFilePresent && string.IsNullOrEmpty(file.name)) // keep existing file
      {
        updatedExperimentPlan.LiteratureFileName = existingExperimentPlan.LiteratureFileName;
        updatedExperimentPlan.LiteratureFileLocation = existingExperimentPlan.LiteratureFileLocation;
      }
    }

    if (string.IsNullOrEmpty(existingExperimentPlan.LiteratureFileName) && model.IsLiteratureReviewFilePresent &&
        !string.IsNullOrEmpty(file.name))
    {
      updatedExperimentPlan.LiteratureFileName = file.name;
      var newLocation = Guid.NewGuid() + Path.GetExtension(file.name);
      updatedExperimentPlan.LiteratureFileLocation = await _azExperimentStorageService
        .Upload(newLocation, file.data);
    }

    _db.Entry(existingExperimentPlan).CurrentValues.SetValues(updatedExperimentPlan);

    await _db.SaveChangesAsync();
    return await GetByUser(id, ownerId);
  }

  public async Task<List<ExperimentModel>> List()
  {
    var list = await _db.Experiments
      .AsNoTracking()
      .Include(x => x.Owner)
      .Include(x => x.ProjectGroup)
      .ThenInclude(x => x.Project)
      .ToListAsync();
    return list.ConvertAll<ExperimentModel>(x => new ExperimentModel(x));
  }

  public async Task<List<ExperimentModel>> ListByUser(string userId)
  {
    var user = await _db.Users.FindAsync(userId)
               ?? throw new KeyNotFoundException();
    var list = await _db.Experiments
      .AsNoTracking()
      .Where(x => x.Owner.Id == user.Id)
      .Include(x => x.Owner)
      .Include(x => x.ProjectGroup)
      .ThenInclude(x => x.Project)
      .ToListAsync();
    return list.ConvertAll<ExperimentModel>(x => new ExperimentModel(x));
  }

  public async Task<ExperimentModel> Get(int experimentId)
  {
    var experiment = await _db.Experiments
                       .AsNoTracking()
                       .Where(x => x.Id == experimentId)
                       .Include(x=>x.Owner)
                       .Include(x => x.ProjectGroup)
                       .ThenInclude(x => x.Project)
                       .FirstOrDefaultAsync()
                     ?? throw new KeyNotFoundException();

    return new ExperimentModel(experiment);
  }
  
  public async Task<ExperimentModel> GetByUser(int experimentId, string userId)
  {
    var experiment = await _db.Experiments
                       .AsNoTracking()
                       .Where(x => x.Owner.Id == userId && x.Id == experimentId)
                       .Include(x=>x.Owner)
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
                   .FirstOrDefaultAsync(x => x.Id == id && x.Owner.Id == userId)
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
