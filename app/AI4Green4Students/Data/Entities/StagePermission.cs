namespace AI4Green4Students.Data.Entities;

public class StagePermission
{
  public int Id { get; set; } 

  public int MinStageSortOrder { get; set; }
  
  public int MaxStageSortOrder { get; set; }

  public string Key { get; set; } = string.Empty;

  public StageType Type { get; set; } = null!;
}
