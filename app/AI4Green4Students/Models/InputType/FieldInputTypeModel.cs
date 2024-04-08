namespace AI4Green4Students.Models.InputType;

public class FileInputTypeModel
{
  public string Name { get; set; } = String.Empty;
  public string Location { get; set; } = String.Empty; // Blob name
  public bool? IsMarkedForDeletion { get; set; }
  public bool? IsNew { get; set; }
}
