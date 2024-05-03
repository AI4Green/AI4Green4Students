using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Data.Entities.SectionTypeData;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
  {
  }

  public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();

  public DbSet<RegistrationRule> RegistrationRules => Set<RegistrationRule>();
  public DbSet<ProjectGroup> ProjectGroups => Set<ProjectGroup>();
  public DbSet<Project> Projects => Set<Project>();
  public DbSet<Comment> Comments => Set<Comment>();
  public DbSet<Field> Fields => Set<Field>();
  public DbSet<FieldResponse> FieldResponses => Set<FieldResponse>();
  public DbSet<FieldResponseValue> FieldResponseValues => Set<FieldResponseValue>();
  public DbSet<Plan> Plans => Set<Plan>();
  public DbSet<Note> Notes => Set<Note>();
  public DbSet<Report> Reports => Set<Report>();
  public DbSet<LiteratureReview> LiteratureReviews => Set<LiteratureReview>();
  public DbSet<InputType> InputTypes => Set<InputType>();
  public DbSet<SectionType> SectionTypes => Set<SectionType>();
  public DbSet<Stage> Stages => Set<Stage>();
  public DbSet<StageType> StageTypes => Set<StageType>();
  public DbSet<StagePermission> StagePermissions => Set<StagePermission>();
  public DbSet<Section> Sections => Set<Section>();
  public DbSet<SelectFieldOption> SelectFieldOptions => Set<SelectFieldOption>();

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.Entity<Project>()
      .HasIndex(x => x.Name)
      .IsUnique();

    builder.Entity<Field>()
      .HasMany(a => a.FieldResponses)
      .WithOne(a => a.Field);
    
    builder.Entity<FieldResponseValue>()
      .Property(x=>x.Value)
      .HasColumnType("jsonb");
    
    // Configure Plan and Note one-to-one relationship
    builder.Entity<Plan>()
      .HasOne(x => x.Note)
      .WithOne(x => x.Plan)
      .HasForeignKey<Note>(x => x.PlanId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
