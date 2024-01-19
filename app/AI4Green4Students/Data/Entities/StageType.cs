using System.ComponentModel.DataAnnotations;

namespace AI4Green4Students.Data.Entities;

public class StageType
{
  [Key]
  public int Id { get; set; }
  
  public string Value { get; set; } = string.Empty;
}
