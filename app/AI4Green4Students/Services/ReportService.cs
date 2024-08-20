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
  private readonly FieldResponseService _fieldResponses;
  private readonly ExportService _export;

  public ReportService(ApplicationDbContext db, StageService stageService, SectionFormService sectionForm, FieldResponseService fieldResponses, ExportService export)
  {
    _db = db;
    _stages = stageService;
    _sectionForm = sectionForm;
    _fieldResponses = fieldResponses;
    _export = export;
  }

  /// <summary>
  /// Get a list of project reports for a given user.
  /// </summary>
  /// <param name="projectId">Id of the project to get reports for.</param>
  /// <param name="userId">Id of the user to get reports for.</param>
  /// <returns>List of project report of the user.</returns>
  public async Task<List<ReportModel>> ListByUser(int projectId, string userId)
  {
    var reports = await ReportsQuery().AsNoTracking().Where(x => x.Owner.Id == userId && x.Project.Id == projectId).ToListAsync();

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
    
    var reports = await ReportsQuery().AsNoTracking().Where(x => pgStudents.Contains(x.Owner)).ToListAsync();
    
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
    var report = await ReportsQuery().AsNoTracking().Where(x => x.Id == id).SingleOrDefaultAsync() ?? throw new KeyNotFoundException();
    
    var permissions = await _stages.GetStagePermissions(report.Stage, StageTypes.Report); 
    return new ReportModel(report)
    {
      Permissions = permissions
    };
  }
  
  /// <summary>
  /// Check if all students have submitted their reports for a given project.
  /// </summary>
  /// <param name="projectId">Project Id.</param>
  /// <returns>True or false.</returns>
  public async Task<bool> HasEveryStudentSubmitted(int projectId)
  {
    var projectStudents = await _db.ProjectGroups
      .AsNoTracking()
      .Where(x => x.Project.Id == projectId)
      .SelectMany(x => x.Students)
      .CountAsync();
    
    var reportsSubmitted = await _db.Reports
      .AsNoTracking()
      .Where(x => x.Project.Id == projectId && x.Stage.Value == ReportStages.Submitted)
      .Select(x => x.Owner)
      .Distinct()
      .CountAsync();
    
    return projectStudents == reportsSubmitted;
  }

  /// <summary>
  /// Check if a student has submitted their report for a given project.
  /// </summary>
  /// <param name="projectId">Project id.</param>
  /// <param name="userId">User id.</param>
  /// <returns>True if submitted, else false.</returns>
  public async Task<bool> HasStudentSubmitted(int projectId, string userId)
    => await _db.Reports
      .AsNoTracking()
      .Where(x => x.Project.Id == projectId && x.Owner.Id == userId && x.Stage.Value == ReportStages.Submitted)
      .AnyAsync();
  

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
    
    entity.FieldResponses = await _fieldResponses.CreateResponses<Report>(entity.Id, projectGroup.Project.Id, SectionTypes.Report, null);
    
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

  /// <summary>
  /// Check if a given user is the member of a given project group.
  /// </summary>
  /// <param name="userId">Id of the user viewing.</param>
  /// <param name="reportId">Report id.</param>
  /// <returns>True if the user viewing is the member of the project group, false otherwise.</returns>
  public async Task<bool> IsInSameProjectGroup(string userId, int reportId)
  {
    var report = await Get(reportId);
    
    // Check if both the owner and the viewer are in the same project group
    return await _db.ProjectGroups.AsNoTracking()
      .Where(x => x.Project.Id == report.ProjectId && x.Students.Any(y => y.Id == report.OwnerId))
      .AnyAsync(x => x.Students.Any(y => y.Id == userId));
  }

  /// <summary>
  /// Check if a given user is the project instructor.
  /// </summary>
  /// <param name="userId">Instructor id to check.</param>
  /// <param name="reportId">Report id.</param>
  /// <returns>True if the user is the instructor, false otherwise.</returns>
  public async Task<bool> IsProjectInstructor(string userId, int reportId)
  {
    var report = await Get(reportId);
    return await _db.Projects.AsNoTracking()
      .AnyAsync(x => x.Id == report.ProjectId && x.Instructors.Any(y => y.Id == userId));
  }
  
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
  /// <returns>Section summaries</returns>
  public async Task<List<SectionSummaryModel>> ListSummary(int reportId)
  {
    var report = await Get(reportId);
    var fieldsResponses = await _fieldResponses.ListBySectionType<Report>(reportId);
    return await _sectionForm.GetSummaryModel(report.ProjectId, SectionTypes.Report, fieldsResponses, report.Permissions, report.Stage);
  }
  
  /// <summary>
  /// Get a report section including its fields, last field response and comments.
  /// </summary>
  /// <param name="sectionId">Id of the section to get</param>
  /// <param name="reportId">Id of the report to get the field responses for</param>
  /// <returns>Report section with its fields, fields response and more.</returns>
  public async Task<SectionFormModel> GetSectionForm(int reportId, int sectionId)
  {
    var fieldsResponses = await _fieldResponses.ListBySection<Report>(reportId, sectionId);
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
      FieldResponses = await _fieldResponses.GenerateFieldResponseSubmissionModel(model.FieldResponses, model.Files, model.FileFieldResponses),
      NewFieldResponses = await _fieldResponses.GenerateFieldResponseSubmissionModel(model.NewFieldResponses, model.NewFiles, model.NewFileFieldResponses, true)
    };
    
    var report = await Get(model.RecordId);
    var fieldResponses = await _fieldResponses.ListBySection<Report>(submission.RecordId, submission.SectionId);

    var updatedValues= report.Stage == ReportStages.Draft
      ? _fieldResponses.UpdateDraft(submission.FieldResponses, fieldResponses)
      : _fieldResponses.UpdateAwaitingChanges(submission.FieldResponses, fieldResponses);
    
    foreach (var updatedValue in updatedValues) _db.Update(updatedValue);
    await _db.SaveChangesAsync();

    if (submission.NewFieldResponses.Count == 0) return await GetSectionForm(submission.RecordId, submission.SectionId);
    
    var entity = await _db.Reports.FindAsync(submission.RecordId) ?? throw new KeyNotFoundException();
    var newFieldResponses = await _fieldResponses.CreateResponses<Report>(report.Id, report.ProjectId, SectionTypes.Report, submission.NewFieldResponses);
    entity.FieldResponses.AddRange(newFieldResponses);
    await _db.SaveChangesAsync();

    return await GetSectionForm(model.RecordId, model.SectionId);
  }

  /// <summary>
  /// Generate a Word document for a given report.
  /// </summary>
  /// <param name="id">Report id.</param>
  /// <param name="projectId">Project id.</param>
  /// <param name="title">Title of the document.</param>
  /// <param name="author">Author of the document.</param>
  /// <returns>Word document as a byte array.</returns>
  public async Task<Stream> GenerateExport(int id, int projectId, string title, string author)
  => await _export.GeExportStream<Report>(id, projectId, title, author);
  
  /// <summary>
  /// Construct a query to fetch Report along with its related entities.
  /// </summary>
  /// <returns>An IQueryable of Report entities.</returns>
  private IQueryable<Report> ReportsQuery()
  {
    return _db.Reports
      .Include(x => x.Project)
      .Include(x => x.Owner)
      .Include(x => x.Stage);
  }
}
