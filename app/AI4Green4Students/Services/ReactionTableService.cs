using AI4Green4Students.Constants;
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

  public List<CompoundModel> GetInitialTableData(ReactionDataModel data)
  {
    var reactantsData = data.Reactants.Select((reactant, index) => new CompoundModel
    {
      Name = reactant,
      MolecularWeight = data.ReactantMolWeights.Count > index ? data.ReactantMolWeights[index] : null,
      Density = data.ReactantDensities.Count > index ? data.ReactantDensities[index] : null,
      Hazards = data.ReactantHazards.Count > index ? data.ReactantHazards[index] : null,
      SubstanceType = ReactionSubstanceType.Reactant
    }).ToList();

    var productsData = data.Products.Select((product, index) => new CompoundModel
    {
      Name = product,
      MolecularWeight = data.ProductMolWeights.Count > index ? data.ProductMolWeights[index] : null,
      Density = null,
      Hazards = data.ProductHazards.Count > index ? data.ProductHazards[index] : null,
      SubstanceType = ReactionSubstanceType.Product
    }).ToList();

    return reactantsData.Concat(productsData).ToList();
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
    => await GetCompound(reagentName, ReactionSubstanceType.Reagent);

  public async Task<SolventModel> GetSolvent(string solventName)
  {
    var solvent = await _db.Solvents
      .AsNoTracking()
      .Where(x => x.Name.ToLower().Equals(solventName.ToLower()))
      .Select(x => new SolventModel
      {
        Name = x.Name,
        Hazards = x.Hazard,
        Flag = x.Flag,
        SubstanceType = ReactionSubstanceType.Solvent
      })
      .FirstOrDefaultAsync();

    if (solvent is not null) return solvent;

    var compound = await GetCompound(solventName, ReactionSubstanceType.Solvent);
    return new SolventModel
    {
      Name = compound.Name,
      MolecularWeight = compound.MolecularWeight,
      Density = compound.Density,
      Hazards = compound.Hazards,
      Smiles = compound.Smiles,
      SubstanceType = compound.SubstanceType,
    };
  }

  private async Task<CompoundModel> GetCompound(string compoundName, string substanceType)
    => await _db.Compounds
         .AsNoTracking()
         .Where(x => x.Name.ToLower().Equals(compoundName.ToLower()))
         .Select(x => new CompoundModel
         {
           Name = x.Name,
           MolecularWeight = x.MolecWeight,
           Density = x.Density,
           Hazards = x.Hphrase,
           Smiles = x.Smiles,
           SubstanceType = substanceType
         })
         .FirstOrDefaultAsync()
       ?? throw new KeyNotFoundException($"Reagent {compoundName} not found");
}
