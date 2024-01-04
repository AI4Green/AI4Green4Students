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
  public DbSet<ProjectGroup> ProjectGroups => Set<ProjectGroup>();
  public DbSet<Project> Projects => Set<Project>();
  public DbSet<Experiment> Experiments => Set<Experiment>();
  public DbSet<ExperimentType> ExperimentTypes => Set<ExperimentType>();
  public DbSet<ExperimentReaction> ExperimentReactions => Set<ExperimentReaction>();
  public DbSet<Comment> Comments => Set<Comment>();
  public DbSet<Conversation> Conversations => Set<Conversation>();
  public DbSet<Field> Fields => Set<Field>();
  public DbSet<FieldResponse> FieldResponses => Set<FieldResponse>();
  public DbSet<FieldResponseValue> FieldResponseValues => Set<FieldResponseValue>();
  public DbSet<InputType> InputTypes => Set<InputType>();
  public DbSet<Section> Sections => Set<Section>();
  public DbSet<SelectFieldOption> SelectFieldOptions => Set<SelectFieldOption>();

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);
    
    builder.Entity<Project>()
    .HasIndex(x => x.Name)
    .IsUnique();

    builder.Entity<FieldResponse>()
            .HasOne(a => a.Conversation)
            .WithOne(a => a.FieldResponse)
            .HasForeignKey<Conversation>(c => c.FieldResponseId);

    builder.Entity<Field>()
      .HasMany(a => a.FieldResponses)
      .WithOne(a => a.Field);
  }
}
