using AI4Green4Students.Data;
using AI4Green4Students.Models.ReactionTable;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class ReactionTableService
{
  private readonly AI4GreenDbContext _db;

  public ReactionTableService(AI4GreenDbContext db)
  {
    _db = db;
  }

  public async Task<List<PartialReagentModel>> ListCompounds(string queryName)
    => await _db.Compounds
      .AsNoTracking()
      .Where(x => x.Name.ToLower().StartsWith(queryName.ToLower()))
      .Select(x => new PartialReagentModel(x.Name))
      .ToListAsync();
  
  public async Task<List<PartialSolventModel>> ListSolvents()
    => await _db.Solvents
      .AsNoTracking()
      .Select(x => new PartialSolventModel(x.Name, x.Flag))
      .ToListAsync();

  public async Task<CompoundModel> GetReagent(string reagentName)
    => await GetCompound(reagentName);

  public async Task<SolventModel> GetSolvent(string solventName)
  {
    var solvent = await _db.Solvents
      .AsNoTracking()
      .Where(x => x.Name.ToLower().Equals(solventName.ToLower()))
      .Select(x => new SolventModel
      {
        Name = x.Name,
        Hazards = x.Hazard,
        Flag = x.Flag
      })
      .FirstOrDefaultAsync();

    if (solvent is not null) return solvent;

    var compound = await GetCompound(solventName);
    return new SolventModel
    {
      Name = compound.Name,
      MolecularWeight = compound.MolecularWeight,
      Density = compound.Density,
      Hazards = compound.Hazards,
      Smiles = compound.Smiles
    };
  }
  
  private async Task<CompoundModel> GetCompound(string compoundName)
    => await _db.Compounds
         .AsNoTracking()
         .Where(x => x.Name.ToLower().Equals(compoundName.ToLower()))
         .Select(x => new CompoundModel
         {
           Name = x.Name,
           MolecularWeight = x.MolecWeight,
           Density = x.Density,
           Hazards = x.Hphrase,
           Smiles = x.Smiles
         })
         .FirstOrDefaultAsync()
       ?? throw new KeyNotFoundException($"Reagent {compoundName} not found");
}
