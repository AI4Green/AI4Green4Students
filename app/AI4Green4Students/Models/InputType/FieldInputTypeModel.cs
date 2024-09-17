using System.Text.Json;

namespace AI4Green4Students.Models.InputType;

/// <summary>
/// Model for File input type
/// </summary>
public class FileInputTypeModel
{
  public string Name { get; set; } = string.Empty;
  public string Location { get; set; } = string.Empty; // Blob name
  public string? Caption { get; set; }
  public bool? IsMarkedForDeletion { get; set; }
  public bool? IsNew { get; set; }
}

/// <summary>
/// Model for multi-text or sortable list item input type
/// </summary>
public class SortableListItemInputTypeModel
{
  public int Order { get; set; }
  public string Content { get; set; } = string.Empty;
}

/// <summary>
/// Model for yield table input type
/// </summary>
public class YieldTableInputTypeModel
{
  public int? SerialNumber { get; set; }
  public string? Product { get; set; } = string.Empty;
  public MeasurementModel? ExpectedMass { get; set; } = new();
  public MeasurementModel? ActualMass { get; set; } = new();
  public double? Moles { get; set; }
  public double? Yield { get; set; }
}

/// <summary>
/// Model for green metrics input type
/// </summary>
public class GreenMetricsInputTypeModel
{
  public WasteIntensityCalculationModel? WasteIntensityCalculation { get; set; } = new();
  public EfactorCalculationModel? EfactorCalculation { get; set; } = new();
  public RmeCalculationModel? RmeCalculation { get; set; } = new();
  public PmiCalculationModel? PmiCalculation { get; set; } = new();
}

/// <summary>
/// Source model for multi input type source
/// </summary>
public class SourceForMultiInputTypeModel
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
}

/// <summary>
/// Model for multi yield table input type
/// </summary>
public class MultiYieldTableInputTypeModel
{
  public SourceForMultiInputTypeModel Source { get; set; } = new();
  public List<YieldTableInputTypeModel> Data { get; set; } = new();
}

/// <summary>
/// Model for multi green metrics input type
/// </summary>
public class MultiGreenMetricsInputTypeModel
{
  public SourceForMultiInputTypeModel Source { get; set; } = new();
  public GreenMetricsInputTypeModel Data { get; set; } = new();
}

public class MultiReactionSchemeInputTypeModel
{
  public SourceForMultiInputTypeModel Source { get; set; } = new();
  public ReactionSchemeInputTypeModel Data { get; set; } = new();
}

/// <summary>
/// Model for measurement
/// </summary>
public class MeasurementModel
{
  public string Unit { get; set; } = string.Empty;
  public double Value { get; set; }
}

/// <summary>
/// Green metrics models
/// </summary>
public class WasteIntensityCalculationModel
{
  public double? Waste { get; set; }
  public double? Output { get; set; }
  public double? WasteIntensity { get; set; }
}
public class EfactorCalculationModel
{
  public double? WasteMass { get; set; }
  public double? ProductMass { get; set; }
  public double? Efactor { get; set; }
}
public class RmeCalculationModel
{
  public double? ProductMass { get; set; }
  public double? ReactantMass { get; set; }
  public double? Rme { get; set; }
}
public class PmiCalculationModel
{
  public double? TotalMassInProcess { get; set; }
  public double? ProductMass { get; set; }
  public double? Pmi { get; set; }
}

/// <summary>
/// Model for reaction scheme input type reaction table
/// </summary>
public class ReactionSchemeInputTypeModel
{
  public List<ReactionTableDataModel> ReactionTable { get; set; } = new();
  public ReactionSketchModel ReactionSketch { get; set; } = new();
}

public class ReactionSketchModel
{
  public string SketcherSmiles { get; set; } = string.Empty;
  public List<string>? Reactants { get; set; }
  public List<string>? Products { get; set; }
  public string? Smiles { get; set; }
  public JsonElement? Data { get; set; }
  public FileInputTypeModel? ReactionImage { get; set; }
}

public class ReactionTableDataModel
{
  public bool? ManualEntry { get; set; }
  public string? SubstanceType { get; set; } = string.Empty;
  public string? SubstancesUsed { get; set; } = string.Empty;
  public bool? Limiting { get; set; }
  public MeasurementModel? Mass { get; set; } = new();
  public string? Gls { get; set; } = string.Empty;
  public double? MolWeight { get; set; }
  public double? Amount { get; set; }
  public double? Density { get; set; }
  public string Hazards { get; set; } = string.Empty;
  public HazardCodesModel? HazardsInput { get; set; } = new();
}

public class HazardCodesModel
{
  public string HazardCodes { get; set; } = string.Empty;
  public string HazardDescription { get; set; } = string.Empty;
}

