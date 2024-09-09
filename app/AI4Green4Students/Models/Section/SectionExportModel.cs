using AI4Green4Students.Models.Field;

namespace AI4Green4Students.Models.Section;

public class SectionExportModel
{
  public int Id { get; set; }
  public int SortOrder { get; set; }
  public string Name { get; set; } = string.Empty;
  public List<ExportFieldModel> Fields { get; set; } = new();
}

public class ExportFieldModel
{
  public int Id { get; set; }
  public int SortOrder { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
  public List<SelectFieldOptionModel>? SelectFieldOptions { get; set; }
  public string Response { get; set; } = string.Empty;
}
