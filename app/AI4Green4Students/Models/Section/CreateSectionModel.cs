namespace AI4Green4Students.Models.Section;

public record CreateSectionModel(string Name, int ProjectTypeId, int SectionTypeId, int SortOrder);
public record SaveSectionsModel(int ProjectTypeId, int SectionTypeId, List<SaveSectionModel> Sections);
public record SaveSectionModel(int? Id, string Name, int SortOrder);
