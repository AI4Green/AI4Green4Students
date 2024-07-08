namespace AI4Green4Students.Constants;

/// <summary>
/// Export constants, which can be used to define formatting when exporting to a file.
/// </summary>
public static class ExportDefinitions
{
  public const string FontFace = "Arial";
  public const string PrimaryHeadingFontSize = "38"; // Document title
  public const string SecondaryHeadingFontSize = "26"; // Document secondary text
  public const string SectionHeadingFontSize = "28";
  public const string FieldNameFontSize = "24";
  public const string FieldNameSecondaryFontSize = "20";
  public const string FieldResponseFontSize = "20";
  public const string CaptionFontSize = "16";
  public const int ImageWidthPixels = 450;
  
}

/// <summary>
/// Constants for the columns in the yield table.
/// </summary>
public static class YieldTable
{
  public static readonly string[] ColumnHeaders = ["Product", "Expected Mass", "Actual Mass", "Moles", "Yield"];
}

/// <summary>
/// Constants for the columns in the reaction table.
/// </summary>
public static class ReactionTable
{
  public static readonly string[] ColumnHeaders =
  [
    "Type", "Substances Used", "Limiting", "Mass (Vol)", "g/l/s (Physical form)", "Mol.Wt", "Amount (mmol)", "Density", "Hazards"
  ];
  
  public const string Title = "Reaction Table";
}


/// <summary>
/// Constants for the columns for Green Metrics calculations.
/// </summary>
public static class WasteIntensity
{
  public static readonly string[] ColumnHeaders = ["Waste Produced", "Productivity Output", "Waste Intensity"];
  public const string Title = "Waste Intensity Calculation";
}
public static class Efactor
{
  public static readonly string[] ColumnHeaders = ["Total Mass of Waste Generated", "Mass of Product Obtained", "E-factor"];
  public const string Title = "E-factor Calculation";
}
public static class Rme
{
  public static readonly string[] ColumnHeaders = ["Mass of Product", "Total Mass of Reactants used", "Reaction Mass Efficiency"];
  public const string Title = "Reaction Mass Efficiency Calculation";
}
public static class Pmi
{
  public static readonly string[] ColumnHeaders = ["Total Mass in Process (including water)", "Mass of Product Obtained", "Process Mass Intensity"];
  public const string Title = "Process Mass Intensity Calculation";
}
