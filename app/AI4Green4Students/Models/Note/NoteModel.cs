namespace AI4Green4Students.Models.Note;

public class NoteModel
{
  public NoteModel(Data.Entities.SectionTypeData.Note entity)
  {
    Id = entity.Id;
    Plan = new NotePlanModel(entity.Plan);
  }

  public NoteModel()
  { }

  public int Id { get; set; }
  public string? ReactionName { get; set; }
  public NotePlanModel Plan { get; set; } = new();
}

public class NotePlanModel
{
  public NotePlanModel(Data.Entities.SectionTypeData.Plan entity)
  {
    Id = entity.Id;
    Title = entity.Title;
    OwnerId = entity.Owner.Id;
    OwnerName = entity.Owner.FullName;
    Stage = entity.Stage.DisplayName;
    ProjectId = entity.Project.Id;
    ProjectName = entity.Project.Name;
  }

  public NotePlanModel()
  { }

  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string OwnerId { get; set; } = string.Empty;
  public string OwnerName { get; set; } = string.Empty;
  public string Stage { get; set; }
  public int ProjectId { get; set; }
  public string ProjectName { get; set; } = string.Empty;
}
