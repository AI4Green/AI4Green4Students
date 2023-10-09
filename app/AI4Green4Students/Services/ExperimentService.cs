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
    
    var updatedExperimentPlan = model.ToUpdateEntity(existingExperimentPlan);
    await UpdateReferences(id, model.References); // update references
    
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
    
    if (string.IsNullOrEmpty(existingExperimentPlan.LiteratureFileName) && model.IsLiteratureReviewFilePresent && !string.IsNullOrEmpty(file.name))
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
      .Include(x => x.References)
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

  private async Task UpdateReferences(int id, List<ReferenceModel> model)
  {
    var existingReferences = await _db.References.Where(x => x.Experiment.Id == id).ToListAsync();

    if (model.Count == 0)
    {
      _db.References.RemoveRange(existingReferences);
      await _db.SaveChangesAsync();
      return;
    }

    var referencesToRemove = new List<Reference>();
    existingReferences.ForEach(reference =>
    {
      var modelReference = model.FirstOrDefault(r => r.Id == reference.Id);
      if (modelReference != null)
      {
        reference.Order = modelReference.Order;
        reference.Content = modelReference.Content;
      }
      else referencesToRemove.Add(reference);
    });

    existingReferences.RemoveAll(r => referencesToRemove.Contains(r));
    _db.References.RemoveRange(referencesToRemove);

    var newReferences = model.Where(reference => existingReferences.All(r => r.Id != reference.Id))
      .Select(reference => new Reference
      {
        Order = reference.Order,
        Content = reference.Content,
        Experiment = _db.Experiments.Find(id) ?? throw new InvalidOperationException()
      }).ToList();

    _db.References.UpdateRange(existingReferences);
    await _db.References.AddRangeAsync(newReferences);
    await _db.SaveChangesAsync();
  }


}
