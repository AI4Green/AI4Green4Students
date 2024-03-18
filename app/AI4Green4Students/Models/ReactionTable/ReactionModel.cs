using System.Text.Json.Serialization;

namespace AI4Green4Students.Models.ReactionTable;

public record ReactionDataModel
{
  [JsonPropertyName("reactants")]
  public List<string> Reactants { get; init; } = new();
  
  [JsonPropertyName("reactant_mol_weights")]
  public List<double> ReactantMolWeights { get; init; } = new();
  
  [JsonPropertyName("reactant_densities")]
  public List<double?> ReactantDensities { get; init; } = new();
  
  [JsonPropertyName("reactant_hazards")]
  public List<string> ReactantHazards { get; init; } = new();
  
  [JsonPropertyName("products")]
  public List<string> Products { get; init; } = new();
  
  [JsonPropertyName("product_mol_weights")]
  public List<double> ProductMolWeights { get; init; } = new();
  
  [JsonPropertyName("product_densities")]
  public List<double?> ProductDensities { get; init; } = new();
  
  [JsonPropertyName("product_hazards")]
  public List<string> ProductHazards { get; init; } = new();
}

public record CompoundModel
{
  public string Name { get; init; } = string.Empty;
  public double? MolecularWeight { get; init; }
  public double? Density { get; init; }
  public string? Hazards { get; init; }
  public string? Smiles { get; init; }
  public string SubstanceType { get; init; } = string.Empty;
}

public record SolventModel : CompoundModel
{
  public int? Flag { get; init; }
}

public record PartialReagentModel(string Name);
public record PartialSolventModel(string Name, int? Flag);
