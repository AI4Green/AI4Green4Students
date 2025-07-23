namespace AI4Green4Students.Services;

using Constants;
using Data;
using Data.Entities.SectionTypeData;
using Extensions;
using Microsoft.EntityFrameworkCore;
using Models.LiteratureReview;
using SectionTypeData;

public class LiteratureReviewService : BaseSectionTypeService<LiteratureReview>
{
  private readonly ApplicationDbContext _db;
  private readonly FieldResponseService _fieldResponses;
  private readonly StageService _stages;

  public LiteratureReviewService(
    ApplicationDbContext db,
    StageService stages,
    FieldResponseService fieldResponses,
    SectionFormService sectionForms
  ) : base(db, sectionForms)
  {
    _db = db;
    _stages = stages;
    _fieldResponses = fieldResponses;
  }

  /// <summary>
  /// List user's project literature reviews.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="userId">User id.</param>
  /// <returns>List user's literature reviews.</returns>
  public async Task<List<LiteratureReviewModel>> ListByUser(int id, string userId)
  {
    var lrs = await Query().AsNoTracking().Where(x => x.Owner.Id == userId && x.Project.Id == id).ToListAsync();
    if (lrs.Count == 0)
    {
      return new List<LiteratureReviewModel>();
    }

    var stageOrders = lrs.Select(x => x.Stage.SortOrder).Distinct().ToList();
    var permissions = await _stages.ListPermissionsByStages(stageOrders, SectionTypes.LiteratureReview);

    return lrs.Select(x => new LiteratureReviewModel(
      x,
      permissions.GetValueOrDefault(x.Stage.SortOrder, new List<string>())
      )).ToList();
  }

  /// <summary>
  /// Get a literature review.
  /// </summary>
  /// <param name="id">Literature review id.</param>
  /// <returns>Literature review.</returns>
  public async Task<LiteratureReviewModel> Get(int id)
  {
    var lr = await Query().AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync()
             ?? throw new KeyNotFoundException();

    return new LiteratureReviewModel(
      lr,
      await _stages.ListPermissions(lr.Stage.SortOrder, SectionTypes.LiteratureReview)
    );
  }

  /// <summary>
  /// Create a new literature review.
  /// </summary>
  /// <param name="userId">User id.</param>
  /// <param name="model">Create model.</param>
  /// <returns>Newly created literature review.</returns>
  public async Task<LiteratureReviewModel> Create(string userId, CreateLiteratureReviewModel model)
  {
    var user = await _db.Users.FindAsync(userId) ?? throw new KeyNotFoundException();
    var pg = await GetProjectGroup(model.ProjectGroupId, userId);

    var existing = await _db.LiteratureReviews
      .Where(x => x.Owner.Id == userId && x.Project.Id == pg.Project.Id)
      .FirstOrDefaultAsync();

    if (existing is not null)
    {
      return await Get(existing.Id);
    }

    var draftStage = await GetStage(SectionTypes.LiteratureReview, Stages.Draft);

    var entity = new LiteratureReview
    {
      Owner = user,
      Project = pg.Project,
      Stage = draftStage
    };

    entity.FieldResponses = await _fieldResponses.CreateResponses<LiteratureReview>(entity.Id, pg.Project.Id);

    _db.LiteratureReviews.Add(entity);
    await _db.SaveChangesAsync();
    return await Get(entity.Id);
  }

  /// <summary>
  /// Advance the stage of a literature review.
  /// </summary>
  /// <param name="id">Literature review id.</param>
  /// <param name="userId">User id.</param>
  /// <param name ="request">Request context model.</param>
  /// <param name="setStage">Stage to set.</param>
  public async Task AdvanceStage(int id, string userId, RequestContextModel? request = null, string? setStage = null)
  {
    var lr = await _db.LiteratureReviews.AsNoTracking()
                 .Include(x => x.Stage)
                 .FirstOrDefaultAsync(x => x.Id == id)
               ?? throw new KeyNotFoundException();

    var stage = await _stages.Advance<LiteratureReview>(id, setStage);

    if (stage is null)
    {
      throw new InvalidOperationException();
    }

    await _stages.SendAdvancementEmail<LiteratureReview>(id, userId, lr.Stage.DisplayName, request);
  }

  /// <summary>
  /// Base literature review query.
  /// </summary>
  private IQueryable<LiteratureReview> Query()
    => _db.LiteratureReviews
      .Include(x => x.Project)
      .Include(x => x.Owner)
      .Include(x => x.Stage);
}
