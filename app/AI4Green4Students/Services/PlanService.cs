using System.Text.Json;
using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Plan;
using AI4Green4Students.Models.Section;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class PlanService
{
  private readonly ApplicationDbContext _db;
  private readonly StageService _stages;
  private readonly SectionService _sections;

  public PlanService(ApplicationDbContext db, StageService stages, SectionService sections)
  {
    _db = db;
    _stages = stages;
    _sections = sections;
  }

  /// <summary>
  /// Get a list of project plans for a given user.
  /// </summary>
  /// <param name="projectId">Id of the project to get plans for.</param>
  /// <param name="userId">Id of the user to get plans for.</param>
  /// <returns>List of project plans of the user.</returns>
  public async Task<List<PlanModel>> ListByUser(int projectId, string userId)
  {
    var plans = await _db.Plans
      .AsNoTracking()
      .Where(x => x.Owner.Id == userId && x.Project.Id == projectId)
      .Include(x => x.Owner)
      .Include(x => x.Stage)
      .Include(x=>x.Note)
      .ToListAsync();

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
    
    var plans = await _db.Plans.AsNoTracking()
      .Where(x => pgStudents.Contains(x.Owner))
      .Include(x => x.Owner)
      .Include(x=>x.Stage)
      .Include(x=>x.Note)
      .ToListAsync();
    
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
    var plan = await _db.Plans.AsNoTracking()
                 .Where(x => x.Id == id)
                 .Include(x => x.Owner)
                 .Include(x => x.Stage)
                 .Include(x=>x.Note)
                 .SingleOrDefaultAsync()
               ?? throw new KeyNotFoundException();
    
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

    var draftStage = _db.Stages
      .FirstOrDefault(x => x.DisplayName == PlanStages.Draft && x.Type.Value == StageTypes.Plan);

    var entity = new Plan { Title = model.Title, Owner = user, Project = projectGroup.Project, Stage = draftStage, Note = new Note()};
    await _db.Plans.AddAsync(entity);

    //Need to setup the field values for this plan now - partly to cover the default values
    //Get all fields of plan type - this way we know which fields are relevant
    var planSectionsFields = await _sections.ListSectionFieldsByType(SectionTypes.Plan, projectGroup.Project.Id);
    var filteredPlanFields = planSectionsFields
      .Where(x => x.InputType.Name != InputTypes.Content && x.InputType.Name != InputTypes.Header).ToList(); // filter out fields, which doesn't need field responses
    
    var noteSectionsFields = await _sections.ListSectionFieldsByType(SectionTypes.Note, projectGroup.Project.Id);
    var filteredNoteFields = noteSectionsFields
      .Where(x => x.InputType.Name != InputTypes.Content && x.InputType.Name != InputTypes.Header).ToList(); // filter out fields, which doesn't need field responses
    
    await _sections.CreateFieldResponses(entity, filteredPlanFields, null); // create field responses for the plan.
    await _sections.CreateFieldResponses(entity.Note, filteredNoteFields, null); // create field responses for the note.

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
  /// Get plan field responses for a given plan. 
  /// </summary>
  /// <param name="planId">Id of the plan to get field responses for.</param>
  /// <returns> A list of plan field responses. </returns>
  public async Task<List<FieldResponse>> GetPlanFieldResponses(int planId)
  {
    var excludedInputTypes = new List<string> { InputTypes.Content, InputTypes.Header };
    return await _db.Plans
        .AsNoTracking()
        .Where(x => x.Id == planId)
        .SelectMany(x => x.PlanFieldResponses
          .Select(y => y.FieldResponse))
        .Where(fr => !excludedInputTypes.Contains(fr.Field.InputType.Name))
        .Include(x => x.Field.InputType)
        .Include(x => x.FieldResponseValues)
        .Include(x => x.Field)
        .ThenInclude(x => x.Section)
        .Include(x => x.Field)
        .ThenInclude(x => x.TriggerTarget)
        .Include(x => x.Conversation)
        .ToListAsync()
      ?? throw new KeyNotFoundException();
  }

  public async Task<PlanModel?> AdvanceStage(int id, string? setStage = null)
  {
    var entity = await _db.Plans
        .Include(x => x.Owner)
        .Include(x => x.Stage)
        .ThenInclude(y => y.NextStage)
        .Include(x=>x.Note)
        .SingleOrDefaultAsync(x => x.Id == id)
        ?? throw new KeyNotFoundException();
    var nextStage = new Stage();
  
  
    if (setStage == null)
    {
      nextStage = await _stages.GetNextStage(entity.Stage, StageTypes.Plan);
  
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
  
  
    var stagePermission = await _stages.GetStagePermissions(nextStage, StageTypes.Plan);
  
    await _db.SaveChangesAsync();
    return new(entity)
    {
      Permissions = stagePermission
    };
  }
  
  /// <summary>
  /// Get a list of plan sections summaries.
  /// Includes each section's status, such as approval status and number of comments.
  /// </summary>
  /// <param name="planId">Id of the plan to be used when processing the summaries</param>
  /// <param name="sectionTypeId">
  /// Id if the section type
  /// Ensures that only sections matching the section type are returned
  /// </param>
  /// <returns>Section summaries list of a plan</returns>
  public async Task<List<SectionSummaryModel>> ListSummariesByPlan(int planId, int sectionTypeId)
  {
    var sections = await _sections.ListBySectionType(sectionTypeId);
    var planFieldResponses = await GetPlanFieldResponses(planId);
    var plan = await Get(planId);
    return _sections.GetSummaryModel(sections, planFieldResponses, plan.Permissions, plan.Stage);
  }
  
  /// <summary>
  /// Get a plan section including its fields, last field response and comments.
  /// </summary>
  /// <param name="sectionId">Id of the section to get</param>
  /// <param name="planId">Id of the plan to get the field responses for</param>
  /// <returns>Plan section with its fields, fields response and more.</returns>
  public async Task<SectionFormModel> GetPlanFormModel(int sectionId, int planId)
  {
    var section = await _sections.Get(sectionId);
    var sectionFields = await _sections.GetSectionFields(sectionId);
    var planFieldResponses = await GetPlanFieldResponses(planId);
    return _sections.GetFormModel(section, sectionFields, planFieldResponses);
  }
  
  /// <summary>
  /// Save plan section form. Also creates new field responses if they don't exist.
  /// </summary>
  /// <param name="model"></param>
  /// <returns></returns>
  public async Task<SectionFormModel> SavePlan(SectionFormSubmissionModel model)
  {
    var planStage = _db.Plans.AsNoTracking().Where(x => x.Id == model.RecordId)
      .Include(pl => pl.Stage).Single()
      .Stage;
    
    var selectedFieldResponses = await _sections.GetSectionFieldResponses(model.SectionId, model.RecordId);

    //check for the stage of the plan - this will define how we handle field values.
    //if its a draft, we can just save the existing (first and only) set of values
    //we can also save every single value from the form, as they're all eligible for submission
    if (planStage.DisplayName == PlanStages.Draft)
    {
      _sections.UpdateDraftFieldResponses(model, selectedFieldResponses);
    }
    //if its awaiting changes, we need to update the latest response value - a new response value will have been created when a comment was left
    // we're only interested in the fields which have been commented on - the others can't be changed so we can ignore them
    else if(planStage.DisplayName == PlanStages.AwaitingChanges)
    {
      _sections.UpdateAwaitingChangesFieldResponses(model, selectedFieldResponses);
    }

    await _db.SaveChangesAsync();
    
    if (model.NewFieldResponses.Count != 0)
    {
      var fields = await _sections.GetSectionFields(model.SectionId);
      var selectedFields = fields.Where(x => model.NewFieldResponses.Any(y=>y.Id == x.Id)).ToList();
      var plan = await _db.Plans.FindAsync(model.RecordId) ?? throw new KeyNotFoundException();
      await _sections.CreateFieldResponses(plan, selectedFields, model.NewFieldResponses);
    }
    
    return await GetPlanFormModel(model.SectionId, model.RecordId);
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
}

