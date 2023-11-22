using AI4Green4Students.Data.Entities.AI4Green;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Data;

public partial class AI4GreenDbContext : DbContext
{
    public AI4GreenDbContext()
    {
    }

    public AI4GreenDbContext(DbContextOptions<AI4GreenDbContext> options)
        : base(options) { }

    public DbSet<Compound> Compounds => Set<Compound>();

    public DbSet<CompoundDataErrorReport> CompoundDataErrorReports => Set<CompoundDataErrorReport>();

    public DbSet<Element> Elements => Set<Element>();

    public DbSet<HazardCode> HazardCodes => Set<HazardCode>();

    public DbSet<Solvent> Solvents => Set<Solvent>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Compound>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Compound_pkey");

            entity.HasOne(d => d.SolventNavigation).WithMany(p => p.Compounds)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Compound_solvent_fkey");
        });

        builder.Entity<CompoundDataErrorReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("CompoundDataErrorReport_pkey");

            entity.HasOne(d => d.CompoundNavigation).WithMany(p => p.CompoundDataErrorReports).HasConstraintName("CompoundDataErrorReport_compound_fkey");
        });

        builder.Entity<Element>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Element_pkey");
        });

        builder.Entity<HazardCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("HazardCode_pkey");
        });

        builder.Entity<Solvent>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("Solvent_pkey");
        });

        OnModelCreatingPartial(builder);
    }

    partial void OnModelCreatingPartial(ModelBuilder builder);
}
