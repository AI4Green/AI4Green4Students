using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data.Entities.SectionTypeData;
using AI4Green4Students.Models.Plan;
using AI4Green4Students.Models.Section;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class PlanService
{
  private readonly ApplicationDbContext _db;
  private readonly StageService _stages;
  private readonly SectionFormService _sectionForm;
  private readonly FieldResponseService _fieldResponses;

  public PlanService(ApplicationDbContext db, StageService stages, SectionFormService sectionForm, FieldResponseService fieldResponses)
  {
    _db = db;
    _stages = stages;
    _sectionForm = sectionForm;
    _fieldResponses = fieldResponses;
  }

  /// <summary>
  /// Get a list of project plans for a given user.
  /// </summary>
  /// <param name="projectId">Id of the project to get plans for.</param>
  /// <param name="userId">Id of the user to get plans for.</param>
  /// <returns>List of project plans of the user.</returns>
  public async Task<List<PlanModel>> ListByUser(int projectId, string userId)
  {
    var plans = await PlansQuery().AsNoTracking().Where(x => x.Owner.Id == userId && x.Project.Id == projectId).ToListAsync();

    var list = new List<PlanModel>();

    foreach (var plan in plans)
    {
      if (plan.Note is null) { plan.Note = await CreateNoteForPlan(plan.Id); } 
      var permissions = await _stages.GetStagePermissions(plan.Stage, StageTypes.Plan);
      var model = new PlanModel(plan)
      {
        Permissions = permissions
      };
      list.Add(model);
    }
    return list;
  }


  /// <summary>
  /// Get student plans for a project group.
  /// </summary>
  /// <param name="projectGroupId">Project group id.</param>
  /// <returns>List of student plans.</returns>
  public async Task<List<PlanModel>> ListByProjectGroup(int projectGroupId)
  {
    var pgStudents = await _db.ProjectGroups
      .AsNoTracking()
      .Include(x => x.Students)
      .Where(x => x.Id == projectGroupId)
      .SelectMany(x => x.Students)
      .ToListAsync();
    
    var plans = await PlansQuery().AsNoTracking().Where(x => pgStudents.Contains(x.Owner)).ToListAsync();
    
    var list = new List<PlanModel>();

    foreach (var plan in plans)
    {
      if (plan.Note is null) { plan.Note = await CreateNoteForPlan(plan.Id); } 
      var permissions = await _stages.GetStagePermissions(plan.Stage, StageTypes.Plan);
      var model = new PlanModel(plan)
      {
        Permissions = permissions
      };
      list.Add(model);
    }
    return list;
  }

  /// <summary>
  /// Get a plan by its id.
  /// </summary>
  /// <param name="id">Id of the plan</param>
  /// <returns>Plan matching the id.</returns>
  public async Task<PlanModel> Get(int id)
  {
    var plan = await PlansQuery().AsNoTracking().Where(x => x.Id == id).SingleOrDefaultAsync() ?? throw new KeyNotFoundException();
    
    if (plan.Note is null) { plan.Note = await CreateNoteForPlan(plan.Id); } 
    var permissions = await _stages.GetStagePermissions(plan.Stage, StageTypes.Plan);
    return new PlanModel(plan)
    {
      Permissions = permissions
    };
  }

  /// <summary>
  /// Create a new plan.
  /// Before creating a plan, check if the user is a member of the project group.
  /// </summary>
  /// <param name="ownerId">Id of the user creating the plan.</param>
  /// <param name="model">Plan dto model. Currently only contains project group id.</param>
  /// <returns>Newly created plan.</returns>
  public async Task<PlanModel> Create(string ownerId, CreatePlanModel model)
  {
    var user = await _db.Users.FindAsync(ownerId)
               ?? throw new KeyNotFoundException();

    var projectGroup = await _db.ProjectGroups
                         .Where(x => x.Id == model.ProjectGroupId && x.Students.Any(y => y.Id == ownerId))
                         .Include(x=>x.Project)
                         .SingleOrDefaultAsync()
                       ?? throw new KeyNotFoundException();

    var draftStage = await _db.Stages.SingleAsync(x => x.DisplayName == PlanStages.Draft && x.Type.Value == StageTypes.Plan);
    var noteDraftStage = await _db.Stages.SingleAsync(x => x.DisplayName == NoteStages.Draft && x.Type.Value == StageTypes.Note);

    var entity = new Plan
    {
      Title = model.Title, Owner = user, Project = projectGroup.Project, Stage = draftStage,
      Note = new Note { Owner = user, Project = projectGroup.Project, Stage = noteDraftStage }
    };
    await _db.Plans.AddAsync(entity);

    //Need to set up the field values for this plan now - partly to cover the default values
    entity.FieldResponses = await _fieldResponses.CreateResponses<Plan>(entity.Id, projectGroup.Project.Id, SectionTypes.Plan, null); // create field responses for the plan.;
    entity.Note.FieldResponses = await _fieldResponses.CreateResponses<Note>(entity.Note.Id, projectGroup.Project.Id, SectionTypes.Note, null); // create field responses for the note.;

    await _db.SaveChangesAsync();
    return await Get(entity.Id);
  }

  /// <summary>
  /// Delete plan by its id.
  /// </summary>
  /// <param name="userId">Id of the user to delete the plan for.</param>
  /// <param name="id">The id of a plan to delete.</param>
  /// <returns></returns>
  public async Task Delete(int id, string userId)
  {
    var entity = await _db.Plans
                   .Where(x => x.Id == id && x.Owner.Id == userId)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    _db.Plans.Remove(entity);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Check if a given user is the owner of a given plan.
  /// </summary>
  /// <param name="userId">Id of the user to check.</param>
  /// <param name="planId">Id of the plan to check the user against.</param>
  /// <returns>True if the user is the owner of the plan, false otherwise.</returns>
  public async Task<bool> IsPlanOwner(string userId, int planId)
    => await _db.Plans
      .AsNoTracking()
      .AnyAsync(x => x.Id == planId && x.Owner.Id == userId);

  /// <summary>
  /// Check if a given user is the member of a given project group.
  /// </summary>
  /// <param name="userId">Id of the user viewing.</param>
  /// <param name="planId">Plan id.</param>
  /// <returns>True if the user viewing is the member of the project group, false otherwise.</returns>
  public async Task<bool> IsInSameProjectGroup(string userId, int planId)
  {
    var plan = await Get(planId);
    
    // Check if both the owner and the viewer are in the same project group
    return await _db.ProjectGroups.AsNoTracking()
      .Where(x => x.Project.Id == plan.ProjectId && x.Students.Any(y => y.Id == plan.OwnerId))
      .AnyAsync(x => x.Students.Any(y => y.Id == userId));
  }

  /// <summary>
  /// Check if a given user is the project instructor.
  /// </summary>
  /// <param name="userId">Instructor id to check.</param>
  /// <param name="planId">Plan id.</param>
  /// <returns>True if the user is the instructor and is not draft plan, false otherwise.</returns>
  public async Task<bool> IsProjectInstructor(string userId, int planId)
  {
    var plan = await Get(planId);
    return await _db.Projects.AsNoTracking()
      .AnyAsync(x => x.Id == plan.ProjectId && plan.Stage != Stages.Draft  && x.Instructors.Any(y => y.Id == userId));
  }
  
  /// <summary>
  /// Advance the stage of a plan.
  /// </summary>
  /// <param name="id">Id of the plan to advance the stage for.</param>
  /// <param name="userId">Id of the user advancing the stage.</param>
  /// <param name="setStage">Stage to set the plan to. (Optional)</param>
  /// <returns>Plan with the updated stage.</returns>
  public async Task<PlanModel?> AdvanceStage(int id, string userId, string? setStage = null)
  {
    var plan = await Get(id); // contains the current stage.
    
    var entity = await _stages.AdvanceStage<Plan>(id, StageTypes.Plan, setStage);
    
    if (entity?.Stage is null) return null;

    if (entity.Stage.DisplayName == Stages.Approved) await CopyReactionSchemeToNote(entity.Id);
    
    var stagePermission = await _stages.GetStagePermissions(entity.Stage, StageTypes.Plan);

    var isNewSubmission = plan.Stage == PlanStages.Draft;
    var comments = entity.Stage.DisplayName == PlanStages.AwaitingChanges ? await CommentCount(id) : 0;

    await _stages.SendStageAdvancementEmail<Plan>(id, userId, isNewSubmission, comments, entity.Title);
    
    return new PlanModel(entity) { Permissions = stagePermission };
  }
  
  /// <summary>
  /// Get section summaries for a given plan.
  /// Includes each section's status, such as approval status and number of comments.
  /// </summary>
  /// <param name="planId">Id of the plan to be used when processing the summaries</param>
  /// <returns>Section summaries</returns>
  public async Task<List<SectionSummaryModel>> ListSummary(int planId)
  {
    var plan = await Get(planId);
    var fieldsResponses = await _fieldResponses.ListBySectionType<Plan>(planId);
    return await _sectionForm.GetSummaryModel(plan.ProjectId, SectionTypes.Plan, fieldsResponses, plan.Permissions, plan.Stage);
  }
  
  /// <summary>
  /// Get a plan section form including its fields, last field response and comments.
  /// </summary>
  /// <param name="planId">Id of the plan to get the field responses for</param>
  /// <param name="sectionId">Id of the section to get</param>
  /// <returns>Plan section form with its fields, fields response and more.</returns>
  public async Task<SectionFormModel> GetSectionForm(int planId, int sectionId)
  {
    var fieldsResponses = await _fieldResponses.ListBySection<Plan>(planId, sectionId);
    return await _sectionForm.GetFormModel(sectionId, fieldsResponses);
  }
  
  /// <summary>
  /// Save plan section form. Also creates new field responses if they don't exist.
  /// </summary>
  /// <param name="model"></param>
  /// <returns></returns>
  public async Task<SectionFormModel> SaveForm(SectionFormPayloadModel model)
  {
    // Transform the payload model to a submission model.
    // Basically, we are preparing the data to be saved in the database.
    var submission = new SectionFormSubmissionModel
    {
      SectionId = model.SectionId,
      RecordId = model.RecordId,
      FieldResponses = await _fieldResponses.GenerateFieldResponseSubmissionModel(model.FieldResponses, model.Files, model.FileFieldResponses),
      NewFieldResponses = await _fieldResponses.GenerateFieldResponseSubmissionModel(model.NewFieldResponses, model.NewFiles, model.NewFileFieldResponses, true)
    };
    
    var plan = await Get(model.RecordId);
    var fieldResponses = await _fieldResponses.ListBySection<Plan>(submission.RecordId, submission.SectionId);

    var updatedValues= plan.Stage == PlanStages.Draft
      ? _fieldResponses.UpdateDraft(submission.FieldResponses, fieldResponses)
      : _fieldResponses.UpdateAwaitingChanges(submission.FieldResponses, fieldResponses);
    
    foreach (var updatedValue in updatedValues) _db.Update(updatedValue);
    await _db.SaveChangesAsync();

    if (submission.NewFieldResponses.Count == 0) return await GetSectionForm(submission.RecordId, submission.SectionId);
    
    var entity = await _db.Plans.FindAsync(submission.RecordId) ?? throw new KeyNotFoundException();
    var newFieldResponses = await _fieldResponses.CreateResponses<Plan>(plan.Id, plan.ProjectId, SectionTypes.Plan, submission.NewFieldResponses);
    entity.FieldResponses.AddRange(newFieldResponses);
    await _db.SaveChangesAsync();

    return await GetSectionForm(model.RecordId, model.SectionId);
  }
  
  /// <summary>
  /// Create a new note for a plan.
  /// </summary>
  /// <remarks>
  /// Not necessary, but since Note entity has been just added to Plan, there might be some plans without a note.
  /// </remarks>
  private async Task<Note> CreateNoteForPlan(int planId)
  {
    var newNote = new Note { PlanId = planId };
    await _db.Notes.AddAsync(newNote);
    await _db.SaveChangesAsync();
    return newNote;
  }

  /// <summary>
  /// Get the number of comments for a plan.
  /// </summary>
  /// <param name="id">Id of the plan.</param>
  /// <returns>Comment count.</returns>
  private async Task<int> CommentCount(int id)
  {
    var sectionSummaries = await ListSummary(id);
    return sectionSummaries.Sum(x => x.Comments);
  }

  /// <summary>
  /// Copy plan's reaction scheme over to note's
  /// </summary>
  /// <param name="planId">Plan id.</param>
  private async Task CopyReactionSchemeToNote(int planId)
  {
    var plan = await Get(planId);

    // Get reaction scheme field response from plan
    var frPlanRScheme = await GetReactionSchemeFieldResponse<Plan>(plan.Id);
    var frvPlanRScheme = frPlanRScheme?.FieldResponseValues.MaxBy(x => x.ResponseDate)?.Value;
    if (frPlanRScheme is null || frvPlanRScheme is null) return;

    // Get reaction scheme field response from note
    var frNoteRScheme = await GetReactionSchemeFieldResponse<Note>(plan.NoteId);
    if (frNoteRScheme is null) // create new one if field response doesn't exist
    {
      var field = await _db.Fields.AsNoTracking()
        .Where(x => x.InputType.Name == InputTypes.ReactionScheme && x.Section.Project.Id == plan.ProjectId)
        .SingleAsync();

      var note = await _db.Notes.Where(x => x.Id == plan.NoteId).SingleAsync();
      var frv = await _fieldResponses.Create(field, frvPlanRScheme);
      note.FieldResponses = new List<FieldResponse> { frv };
      await _db.SaveChangesAsync();
      return;
    }
    
    // Update the reaction scheme field response for note with the plan version
    var frvNoteRScheme = frNoteRScheme.FieldResponseValues.MaxBy(x => x.ResponseDate);
    if (frvNoteRScheme is null)
    {
      var frv = new FieldResponseValue
      {
        FieldResponse = frNoteRScheme,
        Value = frvPlanRScheme
      };
      await _db.AddAsync(frv);
    }
    else
    {
      frvNoteRScheme.Value = frvPlanRScheme;
      _db.Update(frNoteRScheme);
    }
    await _db.SaveChangesAsync();
  }

  private async Task<FieldResponse?> GetReactionSchemeFieldResponse<T>(int id) where T : BaseSectionTypeData
    => await _db.Set<T>()
      .Where(x => x.Id == id)
      .SelectMany(x => x.FieldResponses)
      .Where(x => x.Field.InputType.Name == InputTypes.ReactionScheme)
      .Include(x => x.FieldResponseValues)
      .SingleOrDefaultAsync();
  
  /// <summary>
  /// Construct a query to fetch Plan along with its related entities.
  /// </summary>
  /// <returns>An IQueryable of Plan entities.</returns>
  private IQueryable<Plan> PlansQuery()
  {
    return _db.Plans
      .Include(x => x.Project)
      .Include(x => x.Owner)
      .Include(x => x.Stage)
      .Include(x => x.Note);
  }
}

