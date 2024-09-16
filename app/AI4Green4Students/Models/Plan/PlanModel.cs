namespace AI4Green4Students.Models.Plan;

public class PlanModel
{
  public PlanModel(Data.Entities.SectionTypeData.Plan entity)
  {
    Id = entity.Id;
    Title = entity.Title;
    Deadline = entity.Deadline;
    OwnerId = entity.Owner.Id;
    OwnerName = entity.Owner.FullName;
    Stage = entity.Stage.DisplayName;
    ProjectId = entity.Project.Id;
    ProjectName = entity.Project.Name;
  }

  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string OwnerId { get; set; } = string.Empty;
  public string OwnerName { get; set; } = string.Empty;
  public DateTimeOffset Deadline { get; set; }
  public string Stage { get; set; }
  public int ProjectId { get; set; }
  public string ProjectName { get; set; } = string.Empty;
  public List<string> Permissions { get; set; } = new();
  public PlanNoteModel Note { get; set; } 
}

public class PlanNoteModel
{
  public PlanNoteModel(Data.Entities.SectionTypeData.Note entity)
  {
    Id = entity.Id;
    Stage = entity.Stage.DisplayName;
  }

  public int Id { get; set; }
  public string Stage { get; set; } = string.Empty;
  public List<string> Permissions { get; set; } = new();
}
