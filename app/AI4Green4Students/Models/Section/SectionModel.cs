using AI4Green4Students.Models.SectionType;

namespace AI4Green4Students.Models.Section;

public record SectionModel(
  int Id,
  string Name,
  int SortOrder,
  SectionTypeModel SectionType,
  SectionProjectTypeModel? ProjectType
)
{
  public SectionModel(Data.Entities.Section entity)
    : this(
      entity.Id,
      entity.Name,
      entity.SortOrder,
      new SectionTypeModel(entity.SectionType),
      new SectionProjectTypeModel(entity.ProjectType.Id, entity.ProjectType.Name, entity.ProjectType.Description)
    )
  {
  }
}

public record ProjectSectionModel(int Id, SectionModel Section);

public record SectionProjectTypeModel(int Id, string Name, string Description);
