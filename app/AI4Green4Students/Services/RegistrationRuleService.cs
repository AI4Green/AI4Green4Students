using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class RegistrationRuleService
{
  private readonly ApplicationDbContext _db;
  
  public RegistrationRuleService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<List<RegistrationRuleModel>> List()
  {
    var list = await _db.RegistrationRules
      .AsNoTracking()
      .ToListAsync();

    return list.ConvertAll<RegistrationRuleModel>(x =>
      new RegistrationRuleModel
      {
        Id = x.Id,
        Value = x.Value,
        IsBlocked = x.IsBlocked,
        Modified = x.Modified
      });
  }
  
  public async Task<RegistrationRuleModel> Get(int id)
  {
    var result = await _db.RegistrationRules
      .AsNoTracking()
      .Where(x => x.Id == id)
      .SingleOrDefaultAsync()
      ?? throw new KeyNotFoundException();
    
    return new RegistrationRuleModel
    {
      Value = result.Value,
      IsBlocked = result.IsBlocked,
      Modified = result.Modified
    };
  }

  public async Task Delete(int id)
  {
    var entity = await _db.RegistrationRules
      .AsNoTracking()
      .FirstOrDefaultAsync(x=>x.Id == id)
      ?? throw new KeyNotFoundException();
    
    _db.RegistrationRules.Remove(entity);
    await _db.SaveChangesAsync();
  }
  
  public async Task<RegistrationRuleModel> Create(CreateRegistrationRuleModel model)
  {
    var isExistingValue = await _db.RegistrationRules
      .AsNoTracking()
      .Where(x => x.Value == model.Value)
      .FirstOrDefaultAsync();
    
    if (isExistingValue is not null)
      return await Set(isExistingValue.Id, model); // Update existing rule if it exists
    
    // Else, create new rule for a new value
    var entity = new RegistrationRule
    {
      Value = model.Value,
      IsBlocked = model.IsBlocked,
      Modified = DateTimeOffset.UtcNow
    };
    
    await _db.RegistrationRules.AddAsync(entity);
    await _db.SaveChangesAsync();
    
    return await Get(entity.Id);
  }
  
  public async Task<RegistrationRuleModel> Set (int id, CreateRegistrationRuleModel model)
  {
    var entity = await _db.RegistrationRules
      .AsNoTracking()
      .Where(x => x.Id == id)
      .FirstOrDefaultAsync()
      ?? throw new KeyNotFoundException(); // if rule does not exist
    
    entity.IsBlocked = model.IsBlocked;
    entity.Modified = DateTimeOffset.UtcNow;
    
    _db.RegistrationRules.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }

  public async Task<bool> RuleContainsValue(string email)
  {
    var isEmailAllowed = await _db.RegistrationRules.AnyAsync(rule =>
      email.ToLowerInvariant().Equals(rule.Value) && // true if email matched with rule value and 
      !rule.IsBlocked); // rule value is not blocked

    var isEmailExist = await _db.RegistrationRules.AnyAsync(rule =>
      email.ToLowerInvariant().Equals(rule.Value));// true if email exist in the rule

    var isEmailEndsWithRegValue = await _db.RegistrationRules.AnyAsync(rule =>
      email.ToLowerInvariant().EndsWith(rule.Value) && // true if email ends with rule value and 
      !rule.IsBlocked); // rule value is not blocked

    
    return !(isEmailAllowed // true if exact email exist and it is not blocked 
           || (!isEmailExist // Or true if exact email doesn't exist and
               && isEmailEndsWithRegValue)); // ends with one of the rule value, which is not blocked
  }
}
