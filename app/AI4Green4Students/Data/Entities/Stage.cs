using System.ComponentModel.DataAnnotations;

namespace AI4Green4Students.Data.Entities;

public class Stage
{
  public int Id { get; set; }

  public string Value { get; set; } = string.Empty;
  public string DisplayName { get; set; } = string.Empty;
  
  [Required]
  public int SortOrder { get; set; }

  [Required]
  public StageType Type { get; set; }

  public int TypeId { get; set; }

  public Stage? NextStage { get; set; }
}
