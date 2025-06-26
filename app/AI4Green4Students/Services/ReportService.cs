namespace AI4Green4Students.Services;

using Constants;
using Data;
using Data.Entities.SectionTypeData;
using Microsoft.EntityFrameworkCore;
using Models.Report;
using SectionTypeData;

public class ReportService : BaseSectionTypeService<Report>
{
  private readonly ApplicationDbContext _db;
  private readonly ExportService _export;
  private readonly FieldResponseService _fieldResponses;
  private readonly StageService _stages;

  public ReportService(
    ApplicationDbContext db,
    StageService stageService,
    SectionFormService sectionForm,
    FieldResponseService fieldResponses,
    ExportService export
  ) : base(db, sectionForm)
  {
    _db = db;
    _stages = stageService;
    _fieldResponses = fieldResponses;
    _export = export;
  }

  /// <summary>
  /// List user's project reports.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="userId">User id.</param>
  /// <returns>List user's reports.</returns>
  public async Task<List<ReportModel>> ListByUser(int id, string userId)
  {
    var reports = await Query().AsNoTracking().Where(x => x.Owner.Id == userId && x.Project.Id == id).ToListAsync();
    if (reports.Count == 0)
    {
      return new List<ReportModel>();
    }

    var stageOrders = reports.Select(x => x.Stage.SortOrder).Distinct().ToList();
    var reportPermissions = await _stages.ListPermissionsByStages(stageOrders, SectionTypes.Report);

    var list = reports.Select(x => new ReportModel(
      x,
      reportPermissions.GetValueOrDefault(x.Stage.SortOrder, new List<string>())
    )).ToList();

    return list;
  }

  /// <summary>
  /// Get a report.
  /// </summary>
  /// <param name="id">Report id.</param>
  /// <returns>Report.</returns>
  public async Task<ReportModel> Get(int id)
  {
    var report = await Query().AsNoTracking().Where(x => x.Id == id).SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    return new ReportModel(
      report,
      await _stages.ListPermissions(report.Stage.SortOrder, SectionTypes.Report)
    );
  }

  /// <summary>
  /// Check if all students have submitted their reports for a given project.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <returns>True or false.</returns>
  public async Task<bool> HasEveryStudentSubmitted(int id)
  {
    var projectStudents = await _db.ProjectGroups
      .AsNoTracking()
      .Where(x => x.Project.Id == id)
      .SelectMany(x => x.Students)
      .CountAsync();

    var reportsSubmitted = await _db.Reports
      .AsNoTracking()
      .Where(x => x.Project.Id == id && x.Stage.DisplayName == Stages.Submitted)
      .Select(x => x.Owner)
      .Distinct()
      .CountAsync();

    return projectStudents == reportsSubmitted;
  }

  /// <summary>
  /// Check if a student has submitted their report for a given project.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="userId">User id.</param>
  /// <returns>True if submitted, else false.</returns>
  public async Task<bool> HasStudentSubmitted(int id, string userId)
    => await _db.Reports
      .AsNoTracking()
      .Where(x => x.Project.Id == id && x.Owner.Id == userId && x.Stage.DisplayName == Stages.Submitted)
      .AnyAsync();


  /// <summary>
  /// Create a new report.
  /// </summary>
  /// <param name="userId">User id.</param>
  /// <param name="model">Create model.</param>
  /// <returns>Newly created report.</returns>
  public async Task<ReportModel> Create(string userId, CreateReportModel model)
  {
    var user = await _db.Users.FindAsync(userId) ?? throw new KeyNotFoundException();
    var pg = await GetProjectGroup(model.ProjectGroupId, userId);

    var existing = await _db.Reports
      .Where(x => x.Owner.Id == userId && x.Project.Id == pg.Project.Id)
      .FirstOrDefaultAsync();

    if (existing is not null)
    {
      return await Get(existing.Id);
    }

    var draftStage = await GetStage(SectionTypes.Report, Stages.Draft);

    var entity = new Report
    {
      Title = model.Title, Owner = user, Project = pg.Project, Stage = draftStage
    };

    entity.FieldResponses = await _fieldResponses.CreateResponses<Report>(entity.Id, pg.Project.Id);

    _db.Reports.Add(entity);
    await _db.SaveChangesAsync();
    return await Get(entity.Id);
  }

  /// <summary>
  /// Advance the stage of a report.
  /// </summary>
  /// <param name="id">Report id.</param>
  /// <param name="setStage">Stage to set.</param>
  /// <param name="userId">User id.</param>
  /// <returns>Report.</returns>
  public async Task AdvanceStage(int id, string userId, string? setStage = null)
  {
    var stage = await _stages.Advance<Report>(id, setStage);

    if (stage is null)
    {
      throw new InvalidOperationException();
    }
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
  private IQueryable<Report> Query()
    => _db.Reports
      .Include(x => x.Project)
      .Include(x => x.Owner)
      .Include(x => x.Stage);
}
