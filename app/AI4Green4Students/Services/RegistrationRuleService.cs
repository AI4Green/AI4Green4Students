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

  /// <summary>
  /// Checks to see if provided email is valid, by looking up blocked and allowed emails.
  /// Allowed emails override blocked.
  /// </summary>
  /// <param name="email"></param>
  /// <returns></returns>
  public async Task<bool> ValidEmail(string email)
  {
    //check for a specific block - return false if found
    //e.g. domain allowed but that email has been blocked
    var isSpecificEmailBlocked = await _db.RegistrationRules.AnyAsync
      (rule => email.ToLowerInvariant().Equals(rule.Value) && rule.IsBlocked);

    if (isSpecificEmailBlocked)
      return false;

      //default to valid, unless we find a reason to block.
      var validEmail = true;

    //check for global block - set false if it exists
    var globalExists = await _db.RegistrationRules.AnyAsync(x => x.Value == "*");

    //check for specific block - set false if found
    var isEmailBlocked = await _db.RegistrationRules.AnyAsync(rule =>
    email.ToLowerInvariant().EndsWith(rule.Value) && rule.IsBlocked);

    if (isEmailBlocked || globalExists)
      validEmail = false;

    //check for allow - override to true if found
    var isEmailAllowed = await _db.RegistrationRules.AnyAsync(rule =>
    email.ToLowerInvariant().EndsWith(rule.Value) && !rule.IsBlocked);

    if (isEmailAllowed)
      validEmail = true;

    return validEmail;
  }
}
