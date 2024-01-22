using AI4Green4Students.Models.SectionType;

namespace AI4Green4Students.Models.Section;

public class SectionModel
{
  public SectionModel()
  { }

  public SectionModel(Data.Entities.Section entity)
  {
    Id = entity.Id;
    Name = entity.Name;
    SortOrder = entity.SortOrder;
    SectionType = new SectionTypeModel(entity.SectionType);
  }

  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public int SortOrder { get; set; }
  public SectionTypeModel SectionType { get; set; } = null!;
}
