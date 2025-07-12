namespace AI4Green4Students.Services;

using Auth;
using Constants;
using Data;
using Data.Entities;
using Data.Entities.Identity;
using Data.Entities.SectionTypeData;
using EmailServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Emails;
using Utilities;

public class StageService
{
  private readonly CommentService _comments;
  private readonly ApplicationDbContext _db;
  private readonly StageEmailService _stageEmail;
  private readonly UserManager<ApplicationUser> _users;

  public StageService(ApplicationDbContext db, StageEmailService stageEmail, CommentService comments,
    UserManager<ApplicationUser> users)
  {
    _db = db;
    _stageEmail = stageEmail;
    _comments = comments;
    _users = users;
  }

  /// <summary>
  /// List stage permissions for a given section type.
  /// </summary>
  /// <param name="sortOrder">Stage sort order.</param>
  /// <param name="type">Section type. E.g. Note's section type.</param>
  /// <returns>Stage permissions.</returns>
  public async Task<List<string>> ListPermissions(int sortOrder, string type)
  {
    var stagePermissions = await _db.StagePermissions
      .Where(x => x.Type.Value == type)
      .Where(x => x.MinStageSortOrder <= sortOrder)
      .Where(x => x.MaxStageSortOrder >= sortOrder)
      .Select(x => x.Key)
      .ToListAsync();

    return stagePermissions;
  }

  /// <summary>
  /// List stage permissions grouped by stages for a given section type.
  /// </summary>
  /// <param name="sortOrders">Stage sort orders.</param>
  /// <param name="type">Stage type (e.g. 'Plan').</param>
  /// <returns>Stage permissions.</returns>
  public async Task<Dictionary<int, List<string>>> ListPermissionsByStages(IEnumerable<int> sortOrders, string type)
  {
    var permissions = await _db.StagePermissions
      .Where(x => x.Type.Value == type)
      .Where(x => sortOrders.Contains(x.MinStageSortOrder))
      .Select(x => new
      {
        x.MinStageSortOrder, x.Key
      })
      .ToListAsync();

    return permissions
      .GroupBy(x => x.MinStageSortOrder)
      .ToDictionary(
        g => g.Key,
        g => g.Select(x => x.Key).ToList()
      );
  }

  /// <summary>
  /// Advance the stage.
  /// </summary>
  /// <param name="id">Entity id.</param>
  /// <param name="set">Stage to advance to. If null, the next stage will be used.</param>
  /// <returns>Advanced stage.</returns>
  public async Task<Stage?> Advance<T>(int id, string? set = null) where T : CoreSectionTypeData
  {
    var entity = await _db.Set<T>()
                   .Include(x => x.Stage).ThenInclude(y => y.NextStage)
                   .SingleOrDefaultAsync(x => x.Id == id)
                 ?? throw new KeyNotFoundException();

    var type = SectionTypeHelper.GetSectionTypeName<T>();

    Stage? nextStage;

    if (set is null)
    {
      if (entity.Stage.NextStage is not null)
      {
        nextStage = entity.Stage.NextStage;
      }
      else
      {
        nextStage = await _db.Stages
          .Where(x => x.SortOrder == entity.Stage.SortOrder + 1 && x.Type.Value == type)
          .SingleOrDefaultAsync();
      }
    }
    else
    {
      nextStage = await _db.Stages.Where(x => x.DisplayName == set && x.Type.Value == type).SingleOrDefaultAsync()
                  ?? throw new InvalidOperationException($"Invalid stage identifier: {set}");
    }

    if (nextStage is null)
    {
      return null;
    }

    entity.Stage = nextStage;
    await _db.SaveChangesAsync();

    return nextStage;
  }

  /// <summary>
  /// Send stage advancement email.
  /// </summary>
  /// <param name="id">Entity id.</param>
  /// <param name="userId">User id initiating advancement.</param>
  /// <param name="prevStage">Previous stage of the entity.</param>
  public async Task SendAdvancementEmail<T>(int id, string userId, string prevStage) where T : CoreSectionTypeData
  {
    var user = await _db.Users.FindAsync(userId);
    var entity = await _db.Set<T>().AsNoTracking()
                   .Include(x => x.Owner)
                   .Include(x => x.Project).ThenInclude(x => x.ProjectGroups)
                   .Include(x => x.Stage).ThenInclude(y => y.Type)
                   .SingleOrDefaultAsync(x => x.Id == id)
                 ?? throw new KeyNotFoundException();

    var title = entity switch
    {
      Plan plan => plan.Title,
      Report report => report.Title,
      _ => string.Empty
    };

    var isNewSubmission = prevStage == Stages.Draft;
    var emailModel = new AdvanceStageEmailModel(
      entity.Owner.FullName,
      entity.Project.Name,
      entity.Project.ProjectGroups.First().Name,
      entity.Stage.Type.Value,
      title,
      isNewSubmission
    );

    var stageName = entity.Stage.DisplayName;
    var userIsInstructor = user is not null && await _users.IsInRoleAsync(user, Roles.Instructor);

    if (userIsInstructor)
    {
      var studentEmail = entity.Owner.Email ?? throw new InvalidOperationException();
      var studentName = entity.Owner.FullName;

      await NotifyStudent(
        stageName,
        emailModel,
        new EmailAddress(studentEmail)
        {
          Name = studentName
        },
        user!.FullName,
        await _comments.Count<T>(entity.Id)
      );
    }

    if (!userIsInstructor && stageName == Stages.InReview)
    {
      var addresses = await _db.Projects
        .Where(x => x.Id == entity.Project.Id)
        .SelectMany(x => x.Instructors).Where(x => x.Email != null)
        .Select(x => new EmailAddress(x.Email!)
        {
          Name = x.FullName
        })
        .ToListAsync();

      await NotifyInstructor(isNewSubmission, emailModel, addresses);
    }
  }

  /// <summary>
  /// Notify the student.
  /// </summary>
  /// <param name="stage">Current stage.</param>
  /// <param name="model">Email model.</param>
  /// <param name="email">Student email.</param>
  /// <param name="notifierName">Name of the notifier (instructor).</param>
  /// <param name="comments">Number of comments.</param>
  /// <remarks>Only sends email for certain stages.</remarks>
  private async Task NotifyStudent(
    string stage,
    AdvanceStageEmailModel model,
    EmailAddress email,
    string notifierName,
    int comments
  )
  {
    model.Instructor = notifierName;
    switch (stage)
    {
      case Stages.AwaitingChanges:
        model.CommentCount = comments;
        await _stageEmail.SendRequestChangeNotification(email, model);
        break;

      case Stages.Approved:
        await _stageEmail.SendApproveNotification(email, model);
        break;
    }
  }

  /// <summary>
  /// Notify the instructor.
  /// </summary>
  /// <param name="isNewSubmission">
  /// Is this a new submission?
  /// Useful to determine whether the submission is new or no.
  /// </param>
  /// <param name="model">Email model to use when sending the email.</param>
  /// <param name="addresses">List of addresses to notify.</param>
  private async Task NotifyInstructor(
    bool isNewSubmission,
    AdvanceStageEmailModel model,
    List<EmailAddress> addresses
  )
  {
    foreach (var address in addresses)
    {
      model.Instructor = address.Name;
      if (isNewSubmission)
      {
        await _stageEmail.SendNewSubmissionNotification(address, model);
      }
      else
      {
        await _stageEmail.SendReSubmissionNotification(address, model);
      }
    }
  }
}
