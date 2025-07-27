using System.Text.Json.Serialization;

namespace AI4Green4Students.Models.ReactionTable;

public record ReactionTableDataModel(List<CompoundDataModel> Compounds, MetadataModel Metadata);

public record CompoundDataModel(
  int Id,
  string Name,
  [property: JsonPropertyName("molecular_weight")]
  double? MolecularWeight,
  double? Density,
  string? Hazards,
  string? Smiles,
  [property: JsonPropertyName("substance_type")]
  string SubstanceType
);

public record MetadataModel(
  [property: JsonPropertyName("number_of_reactants")]
  int NumberOfReactants,

  [property: JsonPropertyName("number_of_products")]
  int NumberOfProducts
);

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
