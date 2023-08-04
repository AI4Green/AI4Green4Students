using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options) { }
  public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();
  
  public DbSet<RegistrationRule> RegistrationRules => Set<RegistrationRule>();
  
  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);
  }
}
