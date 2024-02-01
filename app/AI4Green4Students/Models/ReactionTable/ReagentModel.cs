namespace AI4Green4Students.Models.ReactionTable;

public record ReagentModel
{
  public string Name { get; init; } = string.Empty;
  public double? MolecularWeight { get; init; }
  public double? Density { get; init; }
  public string? Hazards { get; init; } = string.Empty;
  public string? Smiles { get; init; } = string.Empty;
}

public record ReagentPartialModel (string Name);
