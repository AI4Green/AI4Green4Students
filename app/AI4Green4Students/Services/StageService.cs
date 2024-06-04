using AI4Green4Students.Auth;
using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Data.Entities.SectionTypeData;
using AI4Green4Students.Models.Emails;
using AI4Green4Students.Services.EmailServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class StageService
{
  private readonly ApplicationDbContext _db;
  private readonly StageEmailService _stageEmail;
  private readonly UserManager<ApplicationUser> _users;

  public StageService(ApplicationDbContext db, StageEmailService stageEmail, UserManager<ApplicationUser> users)
  {
    _db = db;
    _stageEmail = stageEmail;
    _users = users;
  }

  public async Task<Stage?> GetNextStage(Stage currentStage, string stageTypes)
  {
    if (currentStage.NextStage is not null) return currentStage.NextStage;
    
    var nextStage = await _db.Stages
      .Where(x => x.SortOrder == (currentStage.SortOrder + 1))
      .Where(x => x.Type.Value == stageTypes)
      .Include(x => x.NextStage)
      .SingleOrDefaultAsync();

    return nextStage;
  }

  public async Task<List<string>> GetStagePermissions(Stage stage, string stageTypes)
  {
    var proposalStagePermission = await _db.StagePermissions
        .Include(x => x.Type)
        .Where(x => x.Type.Value == stageTypes)
        .Where(x => x.MinStageSortOrder <= stage.SortOrder)
        .Where(x => x.MaxStageSortOrder >= stage.SortOrder)
        .ToListAsync();

    var stagePermission = proposalStagePermission.Select(x => x.Key).ToList();
    return stagePermission;
  }

  /// <summary>
  /// Advance the stage of a section type entity
  /// </summary>
  /// <param name="id">Id of the entity to advance </param>
  /// <param name="stageType">Type of stage to advance. E.g. 'Plan' </param>
  /// <param name="setStage">Stage to advance to. If null, the next stage will be used </param>
  /// <typeparam name="T">Type of entity to advance </typeparam>
  /// <returns>Entity with the updated stage </returns>
  public async Task<T?> AdvanceStage<T>(int id, string stageType, string? setStage = null) where T : CoreSectionTypeData
  {
    var entity = await GetEntity<T>(id);

    var nextStage = await GetStageToAdvance(entity.Stage, stageType, setStage);
    if (nextStage is null) return null;

    entity.Stage = nextStage;
    await _db.SaveChangesAsync();

    return entity;
  }
  
  /// <summary>
  /// Get the next stage to advance to
  /// </summary>
  /// <param name="currentStage">Current stage </param>
  /// <param name="stageType">Type of stage to advance. E.g. 'Plan' </param>
  /// <param name="setStage">Stage to advance to. If null, the next stage will be used </param>
  /// <returns>Next stage to advance to </returns>
  public async Task<Stage?> GetStageToAdvance(Stage currentStage, string stageType, string? setStage = null)
  {
    if (setStage is null) return await GetNextStage(currentStage, stageType);
    
    var nextStage = await _db.Stages
                      .Where(x => x.DisplayName == setStage && x.Type.Value == stageType)
                      .SingleOrDefaultAsync()
                    ?? throw new Exception("Stage identifier not recognised. Cannot advance to the specified stage");

    return nextStage;
  }

  /// <summary>
  /// Send stage advancement email
  /// </summary>
  /// <param name="id">Entity id. e.g. Plan id </param>
  /// <param name="userId">User id of the entity owner </param>
  /// <param name="isNewSubmission">Is this a new submission i.e. previously a draft </param>
  /// <param name="comments">Number of comments </param>
  /// <param name="recordName">Name of the record. Some entities have title/name property </param>
  public async Task SendStageAdvancementEmail<T>(int id, string userId, bool isNewSubmission, int comments, string? recordName = null) where T : CoreSectionTypeData
  {
    var user = await _db.Users.FindAsync(userId);
    var entity = await GetEntity<T>(id);
    var pg = entity.Project.ProjectGroups.First(); // entity can only belong to one project group

    var emailModel = new StageAdvancementEmailModel(
      entity.Owner.FullName,
      entity.Project.Name,
      pg.Name,
      entity.Stage.Type.Value,
      recordName,
      isNewSubmission
    );

    switch (entity.Stage.DisplayName)
    {
      case Stages.InReview:
        //TODO: This may need to be refactored to only send to the instructor of the project group
        var instructors = await _users.GetUsersInRoleAsync(Roles.Instructor);
        foreach (var instructor in instructors)
        {
          if (instructor.Email is null) continue;

          var emailAddress = new EmailAddress(instructor.Email) { Name = instructor.FullName };
          emailModel.InstructorName = instructor.FullName;

          if (isNewSubmission)
            await _stageEmail.SendNewSubmissionNotification(emailAddress, emailModel);
          else
            await _stageEmail.SendReSubmissionNotification(emailAddress, emailModel);
        }

        break;
      case Stages.AwaitingChanges:
        emailModel.InstructorName = user?.FullName;
        emailModel.CommentCount = comments;

        if (entity.Owner.Email is not null)
          await _stageEmail.SendRequestChangeNotification(
            new EmailAddress(entity.Owner.Email) { Name = entity.Owner.FullName },
            emailModel);
        break;

      case Stages.Approved:
        emailModel.InstructorName = user?.FullName;

        if (entity.Owner.Email is not null)
          await _stageEmail.SendApproveNotification(
            new EmailAddress(entity.Owner.Email) { Name = entity.Owner.FullName },
            emailModel);
        break;
    }
  }
  
  private async Task<T> GetEntity<T>(int id) where T : CoreSectionTypeData
  {
    IQueryable<T> query = _db.Set<T>()
      .Include(x => x.Owner)
      .Include(x => x.Stage)
      .ThenInclude(y => y.NextStage)
      .Include(x => x.Project)
      .ThenInclude(y => y.ProjectGroups);
  
    if (typeof(T) == typeof(Plan)) query = query.Include("Note");
  
    return await query.SingleOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException();
  }
}
