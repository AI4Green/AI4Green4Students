using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Report;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class ReportService
{
  private readonly ApplicationDbContext _db;
  private readonly StageService _stages;

  public ReportService(ApplicationDbContext db, StageService stageService)
  {
    _db = db;
    _stages = stageService;
  }

  /// <summary>
  /// Get a list of project reports for a given user.
  /// </summary>
  /// <param name="projectId">Id of the project to get reports for.</param>
  /// <param name="userId">Id of the user to get reports for.</param>
  /// <returns>List of project plans of the user.</returns>
  public async Task<List<ReportModel>> ListByUser(int projectId, string userId)
    => await _db.Reports.AsNoTracking()
      .AsNoTracking()
      .Where(x => x.Plan.Owner.Id == userId && x.Plan.Project.Id == projectId)
      .Include(x => x.Plan)
      .ThenInclude(x => x.Project)
      .ThenInclude(x => x.ProjectGroups)
      .Select(x => new ReportModel(x)).ToListAsync();

  /// <summary>
  /// Get a list of reports matching a given project group.
  /// </summary>
  /// <param name="projectGroupId">Id of the project group to check reports for.</param>
  /// <returns>List of project reports for given project group.</returns>
  public async Task<List<ReportModel>> ListByProjectGroup(int projectGroupId)
  {
    return await _db.Reports.AsNoTracking()
      .Where(x => x.Plan.Id == projectGroupId)
      .Include(x => x.Plan)
      .ThenInclude(x => x.Owner)
      .Include(x => x.Plan)
      .ThenInclude(x => x.Project)
      .ThenInclude(x => x.ProjectGroups)
      .Select(x => new ReportModel(x))
      .ToListAsync();
  }

  /// <summary>
  /// Get a report by its id.
  /// </summary>
  /// <param name="id">Id of the report</param>
  /// <returns>Report matching the id.</returns>
  public async Task<ReportModel> Get(int id)
    => await _db.Reports.AsNoTracking()
         .AsNoTracking()
         .Where(x => x.Id == id)
         .Include(x => x.Plan)
         .ThenInclude(x => x.Owner)
         .Include(x => x.Plan)
         .ThenInclude(x => x.Project)
         .ThenInclude(x => x.ProjectGroups)
         .Select(x => new ReportModel(x)).SingleOrDefaultAsync()
       ?? throw new KeyNotFoundException();

  /// <summary>
  /// Create a new report.
  /// Before creating a report, check if the user is a member of the project group.
  /// </summary>
  /// <param name="ownerId">Id of the user creating the report.</param>
  /// <param name="model">Report dto model. Currently only contains project group id.</param>
  /// <returns>Newly created report.</returns>
  public async Task<ReportModel> Create(string ownerId, CreateReportModel model)
  {
    var user = await _db.Users.FindAsync(ownerId)
               ?? throw new KeyNotFoundException();

    var plan = await _db.Plans
      .Include(x => x.Stage)
      .SingleOrDefaultAsync(x => x.Id == model.PlanId)
      ?? throw new KeyNotFoundException("Plan not found for report");

    //should this be a custom exception for the controller to then handle - if so, what response do we want to return to the front end?
    if(plan.Stage.Value != PlanStages.Approved) 
        throw new Exception("Plan has not been approved yet. Report cannot be created.");

    var projectGroup = await _db.ProjectGroups
                         .SingleOrDefaultAsync(x => x.Id == model.PlanId && x.Students.Any(y => y.Id == ownerId))
                       ?? throw new KeyNotFoundException();

    var draftStage = _db.Stages.SingleOrDefault(x => x.DisplayName == ReportStages.Draft);

    var entity = new Report {Plan = plan , Stage = draftStage };
    await _db.Reports.AddAsync(entity);
    await _db.SaveChangesAsync();
    return await Get(entity.Id);
  }

  /// <summary>
  /// Delete report by its id.
  /// </summary>
  /// <param name="userId">Id of the user to delete the report for.</param>
  /// <param name="id">The id of an experiment reaction to delete.</param>
  /// <returns></returns>
  public async Task Delete(int id, string userId)
  {
    var entity = await _db.Reports
                   .SingleOrDefaultAsync(x => x.Id == id && x.Plan.Owner.Id == userId)
                 ?? throw new KeyNotFoundException();

    _db.Reports.Remove(entity);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Check if a given user is the owner of a given report.
  /// </summary>
  /// <param name="userId">Id of the user to check.</param>
  /// <param name="reportId">Id of the plan to check the user against.</param>
  /// <returns>True if the user is the owner of the report, false otherwise.</returns>
  public async Task<bool> IsReportOwner(string userId, int reportId)
    => await _db.Reports
      .AsNoTracking()
      .Include(x => x.Plan)
      .AnyAsync(x => x.Id == reportId && x.Plan.Owner.Id == userId);

  /// <summary>
  /// Get report field responses for a given report. 
  /// </summary>
  /// <param name="reportId">Id of the report to get field responses for.</param>
  /// <returns> A list of report field responses. </returns>
  public async Task<List<FieldResponse>> GetPlanFieldResponses(int reportId)
    => await _db.Reports
         .AsNoTracking()
         .Where(x => x.Id == reportId)
         .SelectMany(x => x.ReportFieldResponses
           .Select(y => y.FieldResponse))
         .Include(x => x.FieldResponseValues)
         .Include(x => x.Field)
         .ThenInclude(x => x.Section)
         .Include(x => x.Conversation)
         .ToListAsync()
       ?? throw new KeyNotFoundException();

  public async Task<ReportModel?> AdvanceStage(int id, string? setStage = null)
  {
    var entity = await _db.Reports
        .Include(x => x.Plan.Owner)
        .Include(x => x.Stage)
        .ThenInclude(y => y.NextStage)
        .SingleOrDefaultAsync(x => x.Id == id)
        ?? throw new KeyNotFoundException();
    var nextStage = new Stage();


    if (setStage == null)
    {
      nextStage = await _stages.GetNextStage(entity.Stage, StageTypes.Report);

      if (nextStage == null)
        return null;
    }
    else
    {
      nextStage = await _db.Stages
      .Where(x => x.DisplayName == setStage)
      .Where(x => x.Type.Value == StageTypes.Plan)
      .SingleOrDefaultAsync()
      ?? throw new Exception("Stage identifier not recognised. Cannot advance to the specified stage");
    }
    entity.Stage = nextStage;


    var stagePermission = await _stages.GetPlanStagePermissions(nextStage, StageTypes.Report);

    await _db.SaveChangesAsync();
    return new(entity)
    {
      Permissions = stagePermission
    };
  }
}
