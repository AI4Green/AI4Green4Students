namespace AI4Green4Students.Models.Experiment;

public class ReferenceModel
{
  public ReferenceModel(Data.Entities.Reference entity)
  {
    Id= entity.Id;
    Order = entity.Order;
    Content = entity.Content;
  }
  
  public ReferenceModel()
  {
  }
  
  public int Id { get; set; }
  public int Order { get; set; }
  public string Content { get; set; }= string.Empty;
}
