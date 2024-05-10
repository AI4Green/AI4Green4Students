using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities.SectionTypeData;
using AI4Green4Students.Models.Report;
using AI4Green4Students.Models.Section;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class ReportService
{
  private readonly ApplicationDbContext _db;
  private readonly StageService _stages;
  private readonly SectionFormService _sectionForm;

  public ReportService(ApplicationDbContext db, StageService stageService, SectionFormService sectionForm)
  {
    _db = db;
    _stages = stageService;
    _sectionForm = sectionForm;
  }

  /// <summary>
  /// Get a list of project reports for a given user.
  /// </summary>
  /// <param name="projectId">Id of the project to get reports for.</param>
  /// <param name="userId">Id of the user to get reports for.</param>
  /// <returns>List of project report of the user.</returns>
  public async Task<List<ReportModel>> ListByUser(int projectId, string userId)
  {
    var reports = await _db.Reports
      .AsNoTracking()
      .Where(x => x.Owner.Id == userId && x.Project.Id == projectId)
      .Include(x => x.Owner)
      .Include(x => x.Stage)
      .ToListAsync();

    var list = new List<ReportModel>();

    foreach (var report in reports)
    {
      var permissions = await _stages.GetStagePermissions(report.Stage, StageTypes.Report);
      var model = new ReportModel(report)
      {
        Permissions = permissions
      };
      list.Add(model);
    }
    return list; 
  }

  /// <summary>
  /// Get student reports for a project group.
  /// </summary>
  /// <param name="projectGroupId">Id of the project group to check reports for.</param>
  /// <returns>List of project reports for given project group.</returns>
  public async Task<List<ReportModel>> ListByProjectGroup(int projectGroupId)
  {
    var pgStudents = await _db.ProjectGroups
      .AsNoTracking()
      .Include(x => x.Students)
      .Where(x => x.Id == projectGroupId)
      .SelectMany(x => x.Students)
      .ToListAsync();
    
    var reports = await _db.Reports.AsNoTracking()
      .Where(x => pgStudents.Contains(x.Owner))
      .Include(x => x.Owner)
      .Include(x=>x.Stage)
      .ToListAsync();
    
    var list = new List<ReportModel>();

    foreach (var report in reports)
    {
      var permissions = await _stages.GetStagePermissions(report.Stage, StageTypes.Report);
      var model = new ReportModel(report)
      {
        Permissions = permissions
      };
      list.Add(model);
    }
    return list;
  }

  /// <summary>
  /// Get a report by its id.
  /// </summary>
  /// <param name="id">Id of the report</param>
  /// <returns>Report matching the id.</returns>
  public async Task<ReportModel> Get(int id)
  {
    var report = await _db.Reports.AsNoTracking()
               .AsNoTracking()
               .Where(x => x.Id == id)
               .Include(x => x.Owner)
               .Include(x=>x.Stage)
               .SingleOrDefaultAsync()
             ?? throw new KeyNotFoundException();
    
    var permissions = await _stages.GetStagePermissions(report.Stage, StageTypes.Report); 
    return new ReportModel(report)
    {
      Permissions = permissions
    };
  }

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

    var projectGroup = await _db.ProjectGroups
                         .Where(x => x.Id == model.ProjectGroupId && x.Students.Any(y => y.Id == ownerId))
                         .Include(x=>x.Project)
                         .SingleOrDefaultAsync()
                       ?? throw new KeyNotFoundException();
    
    var existing = await _db.Reports
      .Where(x => x.Owner.Id == ownerId && x.Project.Id == projectGroup.Project.Id)
      .FirstOrDefaultAsync();

    if (existing is not null) return await Get(existing.Id); // Only one report allowed to a user for a project.

    var draftStage = await _db.Stages.SingleAsync(x => x.DisplayName == ReportStages.Draft && x.Type.Value == StageTypes.Report);
    
    var entity = new Report { Title = model.Title, Owner = user, Project = projectGroup.Project, Stage = draftStage };
    await _db.Reports.AddAsync(entity);
    
    entity.FieldResponses = await _sectionForm.CreateFieldResponse(projectGroup.Project.Id, SectionTypes.Report, null);
    
    await _db.SaveChangesAsync();
    return await Get(entity.Id);
  }

  /// <summary>
  /// Delete report by its id.
  /// </summary>
  /// <param name="userId">Id of the user to delete the report for.</param>
  /// <param name="id">The id of a report to delete.</param>
  /// <returns></returns>
  public async Task Delete(int id, string userId)
  {
    var entity = await _db.Reports
                   .SingleOrDefaultAsync(x => x.Id == id && x.Owner.Id == userId)
                 ?? throw new KeyNotFoundException();

    _db.Reports.Remove(entity);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Check if a given user is the owner of a given report.
  /// </summary>
  /// <param name="userId">Id of the user to check.</param>
  /// <param name="reportId">Id of the report to check the user against.</param>
  /// <returns>True if the user is the owner of the report, false otherwise.</returns>
  public async Task<bool> IsReportOwner(string userId, int reportId)
    => await _db.Reports
      .AsNoTracking()
      .AnyAsync(x => x.Id == reportId && x.Owner.Id == userId);

  public async Task<ReportModel?> AdvanceStage(int id, string? setStage = null)
  {
    var entity = await _stages.AdvanceStage<Report>(id, StageTypes.Report, setStage);

    if (entity?.Stage is null) return null;

    var stagePermission = await _stages.GetStagePermissions(entity.Stage, StageTypes.Report);
    return new ReportModel(entity) { Permissions = stagePermission };
  }
  
  /// <summary>
  /// Get section summaries for a given report.
  /// Includes each section's status, such as approval status and number of comments.
  /// </summary>
  /// <param name="reportId">Id of the report to be used when processing the summaries</param>
  /// <param name="sectionTypeId">Id of the section type.</param>
  /// <returns>Section summaries</returns>
  public async Task<List<SectionSummaryModel>> ListSummary(int reportId, int sectionTypeId)
  {
    var report = await Get(reportId);
    var fieldsResponses = await _sectionForm.ListBySectionType<Report>(reportId);
    return await _sectionForm.GetSummaryModel(sectionTypeId, fieldsResponses, report.Permissions, report.Stage);
  }
  
  /// <summary>
  /// Get a report section including its fields, last field response and comments.
  /// </summary>
  /// <param name="sectionId">Id of the section to get</param>
  /// <param name="reportId">Id of the report to get the field responses for</param>
  /// <returns>Report section with its fields, fields response and more.</returns>
  public async Task<SectionFormModel> GetSectionForm(int reportId, int sectionId)
  {
    var fieldsResponses = await _sectionForm.ListBySection<Report>(reportId, sectionId);
    return await _sectionForm.GetFormModel(sectionId, fieldsResponses);
  }
  
  /// <summary>
  /// Save report section form. Also creates new field responses if they don't exist.
  /// </summary>
  /// <param name="model"></param>
  /// <returns></returns>
  public async Task<SectionFormModel> SaveForm(SectionFormPayloadModel model)
  {
    var submission = new SectionFormSubmissionModel
    {
      SectionId = model.SectionId,
      RecordId = model.RecordId,
      FieldResponses = await _sectionForm.GenerateFieldResponses(model.FieldResponses, model.Files, model.FileFieldResponses),
      NewFieldResponses = await _sectionForm.GenerateFieldResponses(model.NewFieldResponses, model.NewFiles, model.NewFileFieldResponses)
    };
    
    var report = await Get(submission.RecordId);
    var fieldResponses = await _sectionForm.ListBySection<Report>(submission.RecordId, submission.SectionId);

    var updatedValues= report.Stage == ReportStages.Draft
      ? _sectionForm.UpdateDraftFieldResponses(submission.FieldResponses, fieldResponses)
      : _sectionForm.UpdateAwaitingChangesFieldResponses(submission.FieldResponses, fieldResponses);
    
    foreach (var updatedValue in updatedValues) _db.Update(updatedValue);
    await _db.SaveChangesAsync();

    if (submission.NewFieldResponses.Count == 0) return await GetSectionForm(submission.RecordId, submission.SectionId);
    
    var entity = await _db.Reports.FindAsync(submission.RecordId) ?? throw new KeyNotFoundException();
    entity.FieldResponses = await _sectionForm.CreateFieldResponse(submission.RecordId, SectionTypes.Report, submission.NewFieldResponses);

    return await GetSectionForm(model.RecordId, model.SectionId);
  }
}
