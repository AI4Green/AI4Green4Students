namespace AI4Green4Students.Services;

using Constants;
using Data;
using Microsoft.EntityFrameworkCore;
using Models.ReactionTable;

public class ReactionTableService
{
  private readonly AI4GreenDbContext _db;

  public ReactionTableService(AI4GreenDbContext db) => _db = db;

  /// <summary>
  /// List compounds starting with the given string.
  /// </summary>
  /// <param name="query">String to search for.</param>
  /// <returns>Compounds.</returns>
  public async Task<List<PartialReagentModel>> ListCompounds(string query)
    => await _db.Compounds
      .AsNoTracking()
      .Where(x => EF.Functions.ILike(x.Name, $"{query}%"))
      .Select(x => new PartialReagentModel(x.Name))
      .ToListAsync();

  /// <summary>
  /// List solvents.
  /// </summary>
  /// <returns>Solvents.</returns>
  public async Task<List<PartialSolventModel>> ListSolvents()
    => await _db.Solvents
      .AsNoTracking()
      .Select(x => new PartialSolventModel(x.Name, x.Flag))
      .ToListAsync();

  /// <summary>
  /// Get a reagent by name.
  /// </summary>
  /// <param name="name">Reagent name.</param>
  /// <returns>Reagent.</returns>
  public async Task<CompoundModel> GetReagent(string name)
    => await GetCompound(name, ReactionSubstanceType.Reagent);

  /// <summary>
  /// Get a solvent by name.
  /// </summary>
  /// <param name="name">Solvent name.</param>
  /// <returns>Solvent.</returns>
  public async Task<SolventModel> GetSolvent(string name)
  {
    var solvent = await _db.Solvents
      .AsNoTracking()
      .Where(x => EF.Functions.ILike(x.Name, name))
      .Select(x => new SolventModel
      {
        Name = x.Name, Hazards = x.Hazard, Flag = x.Flag, SubstanceType = ReactionSubstanceType.Solvent
      })
      .FirstOrDefaultAsync();

    if (solvent is not null)
    {
      return solvent;
    }

    var compound = await GetCompound(name, ReactionSubstanceType.Solvent);
    return new SolventModel
    {
      Name = compound.Name,
      MolecularWeight = compound.MolecularWeight,
      Density = compound.Density,
      Hazards = compound.Hazards,
      Smiles = compound.Smiles,
      SubstanceType = compound.SubstanceType
    };
  }

  /// <summary>
  /// Get a compound by name and substance type.
  /// </summary>
  /// <param name="name">Compound name.</param>
  /// <param name="type">Substance type. E.g. Solvent</param>
  /// <returns>Compound.</returns>
  private async Task<CompoundModel> GetCompound(string name, string? type = null)
    => await _db.Compounds
         .AsNoTracking()
         .Where(x => EF.Functions.ILike(x.Name, name))
         .Select(x => new CompoundModel
         {
           Name = x.Name,
           MolecularWeight = x.MolecWeight,
           Density = x.Density,
           Hazards = x.Hphrase,
           Smiles = x.Smiles,
           SubstanceType = type ?? string.Empty
         })
         .FirstOrDefaultAsync()
       ?? throw new KeyNotFoundException($"Compound with name '{name}' not found.");
}
