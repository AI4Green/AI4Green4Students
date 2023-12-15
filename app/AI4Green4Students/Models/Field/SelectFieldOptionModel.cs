namespace AI4Green4Students.Models.Field;

public class SelectFieldOptionModel
{
  public SelectFieldOptionModel(Data.Entities.SelectFieldOption entity)
  {
    Id = entity.Id;
    Name = entity.Name;
  }
  
  public SelectFieldOptionModel()
  {
    
  }
  
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
}
