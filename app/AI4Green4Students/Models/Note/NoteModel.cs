namespace AI4Green4Students.Models.Note;

public class BaseNoteModel
{
  public int Id { get; set; }
  public string Stage { get; set; } = string.Empty;
  public int ProjectId { get; set; }
  public string ProjectName { get; set; } = string.Empty;
}

public class NoteModel : BaseNoteModel
{
  public NoteModel(Data.Entities.SectionTypeData.Note entity)
  {
    Id = entity.Id;
    Stage = entity.Stage.DisplayName;
    ProjectId = entity.Project.Id;
    ProjectName = entity.Project.Name;
    Plan = new NotePlanModel(entity.Plan);
    FeedbackRequested = entity.FeedbackRequested;
  }

  public NoteModel()
  { }
  
  public string? ReactionName { get; set; }
  public List<string> Permissions { get; set; } = new();
  public NotePlanModel Plan { get; set; } = new();

  public bool FeedbackRequested { get; set; }
}

public class NotePlanModel : BaseNoteModel
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
  
  public string Title { get; set; } = string.Empty;
  public string OwnerId { get; set; } = string.Empty;
  public string OwnerName { get; set; } = string.Empty;
}
