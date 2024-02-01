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

  public async Task<List<ReagentPartialModel>> ListPartialReagents(string queryName)
    => await _db.Compounds
      .AsNoTracking()
      .Where(x => x.Name.ToLower().StartsWith(queryName.ToLower()))
      .Select(x => new ReagentPartialModel(x.Name))
      .ToListAsync();

  public async Task<ReagentModel> GetReagent(string reagentName)
    => await _db.Compounds
         .AsNoTracking()
         .Where(x => x.Name.ToLower().Equals(reagentName.ToLower()))
         .Select(x => new ReagentModel
         {
           Name = x.Name,
           MolecularWeight = x.MolecWeight,
           Density = x.Density,
           Hazards = x.Hphrase,
           Smiles = x.Smiles
         })
         .FirstOrDefaultAsync()
       ?? throw new KeyNotFoundException($"Reagent {reagentName} not found");
}
