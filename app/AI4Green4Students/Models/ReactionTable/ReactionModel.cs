namespace AI4Green4Students.Models.ReactionTable;

public record CompoundModel
{
  public string Name { get; init; } = string.Empty;
  public double? MolecularWeight { get; init; }
  public double? Density { get; init; }
  public string? Hazards { get; init; } = string.Empty;
  public string? Smiles { get; init; } = string.Empty;
}

public record SolventModel
{
  public string Name { get; init; } = string.Empty;
  public double? MolecularWeight { get; init; }
  public double? Density { get; init; }
  public string? Hazards { get; init; } = string.Empty;
  public int? Flag { get; init; }
  public string? Smiles { get; init; } = string.Empty;
}

public record PartialReagentModel(string Name);
public record PartialSolventModel(string Name, int? Flag);
