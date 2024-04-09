using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data.Entities.Identity;
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
  public DbSet<PlanFieldResponse> PlanFieldResponses => Set<PlanFieldResponse>();
  public DbSet<NoteFieldResponse> NoteFieldResponses => Set<NoteFieldResponse>();
  public DbSet<ReportFieldResponse> ReportFieldResponses => Set<ReportFieldResponse>();
  public DbSet<ProjectGroupFieldResponse> ProjectGroupFieldResponses => Set<ProjectGroupFieldResponse>();
  public DbSet<LiteratureReviewFieldResponse> LiteratureReviewFieldResponses => Set<LiteratureReviewFieldResponse>();
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

    // Configure ProjectGroupFieldResponse
    builder.Entity<ProjectGroupFieldResponse>()
      .HasKey(x => new { x.ProjectGroupId, x.FieldResponseId });
    
    builder.Entity<ProjectGroupFieldResponse>()
      .HasOne(x => x.ProjectGroup)
      .WithMany(x => x.ProjectGroupFieldResponses)
      .HasForeignKey(x => x.ProjectGroupId);

    builder.Entity<ProjectGroupFieldResponse>()
      .HasOne(x => x.FieldResponse)
      .WithMany(x => x.ProjectGroupFieldResponses)
      .HasForeignKey(x => x.FieldResponseId);
    
    // Configure LiteratureReviewFieldResponse
    builder.Entity<LiteratureReviewFieldResponse>()
      .HasKey(x => new { x.LiteratureReviewId, x.FieldResponseId });

    builder.Entity<LiteratureReviewFieldResponse>()
      .HasOne(x => x.LiteratureReview)
      .WithMany(x => x.LiteratureReviewFieldResponses)
      .HasForeignKey(x => x.LiteratureReviewId);

    builder.Entity<LiteratureReviewFieldResponse>()
      .HasOne(x => x.FieldResponse)
      .WithMany(x => x.LiteratureReviewFieldResponses)
      .HasForeignKey(x => x.FieldResponseId);
    
    // Configure PlanFieldResponse
    builder.Entity<PlanFieldResponse>()
      .HasKey(x => new { x.PlanId, x.FieldResponseId });

    builder.Entity<PlanFieldResponse>()
      .HasOne(x => x.Plan)
      .WithMany(x => x.PlanFieldResponses)
      .HasForeignKey(x => x.PlanId);

    builder.Entity<PlanFieldResponse>()
      .HasOne(x => x.FieldResponse)
      .WithMany(x => x.PlanFieldResponses)
      .HasForeignKey(x => x.FieldResponseId);

    // Configure NoteFieldResponse
    builder.Entity<NoteFieldResponse>()
      .HasKey(x => new { x.NoteId, x.FieldResponseId });

    builder.Entity<NoteFieldResponse>()
      .HasOne(x => x.Note)
      .WithMany(x => x.NoteFieldResponses)
      .HasForeignKey(x => x.NoteId);

    builder.Entity<NoteFieldResponse>()
      .HasOne(x => x.FieldResponse)
      .WithMany(x => x.NoteFieldResponses)
      .HasForeignKey(x => x.FieldResponseId);
    
    // Configure ReportFieldResponse
    builder.Entity<ReportFieldResponse>()
      .HasKey(x => new { x.ReportId, x.FieldResponseId });

    builder.Entity<ReportFieldResponse>()
      .HasOne(x => x.Report)
      .WithMany(x => x.ReportFieldResponses)
      .HasForeignKey(x => x.ReportId);

    builder.Entity<ReportFieldResponse>()
      .HasOne(x => x.FieldResponse)
      .WithMany(x => x.ReportFieldResponses)
      .HasForeignKey(x => x.FieldResponseId);
    
    // Configure Plan and Note one-to-one relationship
    builder.Entity<Plan>()
      .HasOne(x => x.Note)
      .WithOne(x => x.Plan)
      .HasForeignKey<Note>(x => x.PlanId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
