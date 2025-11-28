namespace AI4Green4Students.Services;

using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Models;

public class RegistrationRuleService
{
  private readonly ApplicationDbContext _db;

  public RegistrationRuleService(ApplicationDbContext db) => _db = db;

  /// <summary>
  /// List all registration rules.
  /// </summary>
  /// <returns>Registration rules.</returns>
  public async Task<List<RegistrationRuleModel>> List()
    => await _db.RegistrationRules
      .AsNoTracking()
      .Select(x => new RegistrationRuleModel
      {
        Id = x.Id, Value = x.Value, IsBlocked = x.IsBlocked, Modified = x.Modified
      })
      .ToListAsync();

  /// <summary>
  /// Get registration rule by id.
  /// </summary>
  /// <param name="id">Id.</param>
  /// <returns>Registration rule.</returns>
  public async Task<RegistrationRuleModel> Get(int id)
    => await _db.RegistrationRules
         .AsNoTracking()
         .Where(x => x.Id == id)
         .Select(x => new RegistrationRuleModel
         {
           Id = x.Id, Value = x.Value, IsBlocked = x.IsBlocked, Modified = x.Modified
         })
         .SingleOrDefaultAsync()
       ?? throw new KeyNotFoundException();

  /// <summary>
  /// Delete registration rule by id.
  /// </summary>
  /// <param name="id">Rule Id..</param>
  /// <exception cref="KeyNotFoundException"></exception>
  public async Task Delete(int id)
  {
    var entity = await _db.RegistrationRules
                   .AsNoTracking()
                   .FirstOrDefaultAsync(x => x.Id == id)
                 ?? throw new KeyNotFoundException();

    _db.RegistrationRules.Remove(entity);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Create registration rule.
  /// </summary>
  /// <param name="model">Create model.</param>
 public async Task Create(CreateRegistrationRuleModel model)
  {
    var isExistingValue = await _db.RegistrationRules
      .AsNoTracking()
      .FirstOrDefaultAsync(x => EF.Functions.ILike(x.Value, model.Value));

    if (isExistingValue is not null)
    {
      throw new InvalidOperationException("A rule with this value already exists.");
    }

    var entity = new RegistrationRule
    {
      Value = model.Value, IsBlocked = model.IsBlocked, Modified = DateTimeOffset.UtcNow
    };

    await _db.RegistrationRules.AddAsync(entity);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Update registration rule.
  /// </summary>
  /// <param name="id">Rule Id.</param>
  /// <param name="model">Update model.</param>
  public async Task Set(int id, CreateRegistrationRuleModel model)
  {
    var entity = await _db.RegistrationRules
                   .AsNoTracking()
                   .Where(x => x.Id == id)
                   .FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    entity.IsBlocked = model.IsBlocked;
    entity.Modified = DateTimeOffset.UtcNow;

    _db.RegistrationRules.Update(entity);
    await _db.SaveChangesAsync();
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
      (x => EF.Functions.ILike(email, x.Value) && x.IsBlocked);

    if (isSpecificEmailBlocked)
    {
      return false;
    }

    //default to valid, unless we find a reason to block.
    var validEmail = true;

    //check for global block - set false if it exists
    var globalExists = await _db.RegistrationRules.AnyAsync(x => x.Value == "*");

    //check for specific block - set false if found
    var isEmailBlocked = await _db.RegistrationRules.AnyAsync(rule =>
      email.ToLowerInvariant().EndsWith(rule.Value) && rule.IsBlocked);

    if (isEmailBlocked || globalExists)
    {
      validEmail = false;
    }

    //check for allow - override to true if found
    var isEmailAllowed = await _db.RegistrationRules.AnyAsync(rule =>
      email.ToLowerInvariant().EndsWith(rule.Value) && !rule.IsBlocked);

    if (isEmailAllowed)
    {
      validEmail = true;
    }

    return validEmail;
  }
}
