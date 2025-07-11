namespace AI4Green4Students.Models.Note;

using Data.Entities.SectionTypeData;
using SectionTypeData;

public record NoteModel(
  int Id,
  string? ReactionName,
  string Stage,
  int ProjectId,
  string ProjectName,
  bool FeedbackRequested,
  List<string> Permissions,
  NotePlanModel Plan
)
{
  public NoteModel(Note entity, List<string> permissions, string? reactionName = null)
    : this(
      entity.Id,
      reactionName,
      entity.Stage.DisplayName,
      entity.Project.Id,
      entity.Project.Name,
      entity.FeedbackRequested,
      permissions,
      new NotePlanModel(entity.Plan)
    )
  {
  }
}

public record NotePlanModel : BaseSectionTypeModel
{
  public string OwnerId { get; } = string.Empty;
  public string OwnerName { get; } = string.Empty;

  public NotePlanModel(Plan entity)
    : base(
      entity.Id,
      entity.Title,
      entity.Stage.DisplayName,
      entity.Project.Id,
      entity.Project.Name,
      entity.Deadline
    )
  {
    Title = entity.Title;
    OwnerId = entity.Owner.Id;
    OwnerName = entity.Owner.FullName;
  }
}

public record NoteFeedbackEmailModel(
  (int Id, string Name) Project,
  (int Id, string Name) ProjectGroup,
  string Plan,
  (string Id, string Name, string? Email) Owner,
  List<(string Id, string Name, string? Email)> Instructors
)
{
  public NoteFeedbackEmailModel(Note note)
    : this(
      (note.Project.Id, note.Project.Name),
      GetProjectGroup(note),
      note.Plan.Title,
      (note.Owner.Id, note.Owner.FullName, note.Owner.Email),
      note.Project.Instructors.Select(x => (x.Id, x.FullName, x.Email)).ToList()
    )
  {
  }

  private static (int Id, string Name) GetProjectGroup(Note note)
  {
    var projectGroup = note.Owner.ProjectGroups.FirstOrDefault(x => x.Project.Id == note.Project.Id);

    return projectGroup != null
      ? (projectGroup.Id, projectGroup.Name)
      : throw new InvalidOperationException($"Note owner is not in a project group for project {note.Project.Id}");
  }
}
