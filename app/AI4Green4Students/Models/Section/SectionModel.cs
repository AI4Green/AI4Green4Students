namespace AI4Green4Students.Models.Section;

public class SectionModel
{
  public SectionModel()
  { }

  public SectionModel(Data.Entities.Section entity)
  {
    Id = entity.Id;
    Name = entity.Name;
  }

  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
}
