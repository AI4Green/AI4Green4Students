namespace AI4Green4Students.Models.SectionType;

public class SectionTypeModel
{
  public SectionTypeModel(Data.Entities.SectionType entity)
  {
    Id = entity.Id;
    Name = entity.Name;
  }

  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
}
