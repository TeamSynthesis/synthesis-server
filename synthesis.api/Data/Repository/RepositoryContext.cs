using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using synthesis.api.Data.Models;

namespace synthesis.api.Data.Repository;

public class RepositoryContext : DbContext
{
    public RepositoryContext(DbContextOptions<RepositoryContext> dbContext)
    : base(dbContext)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserModel>().HasIndex(u => u.UserName);
        modelBuilder.Entity<UserModel>().HasIndex(u => u.Email);

        modelBuilder.Entity<TeamModel>()
           .HasMany(t => t.Developers)
           .WithMany(m => m.Teams)
           .UsingEntity(j => j.ToTable("TeamMembers"));

        modelBuilder.Entity<ProjectModel>().OwnsOne(project => project.ProjectMetadata, OwnedNavigationBuilder =>
        {
            OwnedNavigationBuilder.ToJson();

            OwnedNavigationBuilder.OwnsOne(overview => overview.Overview, OwnedNavigationBuilder =>
            {
                OwnedNavigationBuilder.OwnsMany(suggestedName => suggestedName.SuggestedNames);
                OwnedNavigationBuilder.OwnsMany(suggestedDomain => suggestedDomain.SuggestedDomains);
            });

            OwnedNavigationBuilder.OwnsOne(moodBoard => moodBoard.MoodBoard, OwnedNavigationBuilder =>
            {
                OwnedNavigationBuilder.OwnsMany(image => image.Images);
            });

            OwnedNavigationBuilder.OwnsOne(branding => branding.Branding, OwnedNavigationBuilder =>
            {
                OwnedNavigationBuilder.OwnsMany(icon => icon.Icons);

            });

            OwnedNavigationBuilder.OwnsOne(competitiveAnalysis => competitiveAnalysis.CompetitiveAnalysis, OwnedNavigationBuilder =>
            {
                OwnedNavigationBuilder.OwnsMany(competitor => competitor.Competitors);
                OwnedNavigationBuilder.OwnsOne(swot => swot.Swot);
            });

            OwnedNavigationBuilder.OwnsOne(colorPalette => colorPalette.ColorPalette);

            OwnedNavigationBuilder.OwnsOne(mockups => mockups.Mockups, OwnedNavigationBuilder =>
            {
                OwnedNavigationBuilder.OwnsMany(image => image.Images);
            });

            OwnedNavigationBuilder.OwnsMany(wireframe => wireframe.Wireframes);

            OwnedNavigationBuilder.OwnsOne(typography => typography.Typography);

            OwnedNavigationBuilder.OwnsOne(features => features.Features);

            OwnedNavigationBuilder.OwnsOne(technology => technology.Technology, OwnedNavigationBuilder =>
            {
                OwnedNavigationBuilder.OwnsMany(techstack => techstack.Stacks);
            });

            OwnedNavigationBuilder.OwnsOne(targetAudience => targetAudience.TargetAudience, OwnedNavigationBuilder =>
            {
                OwnedNavigationBuilder.OwnsOne(demographics => demographics.Demographics);
            });

        });
    }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<OrganisationModel> Organisations { get; set; }
    public DbSet<MemberModel> Members { get; set; }
    public DbSet<ProjectModel> Projects { get; set; }
    public DbSet<TeamModel> Teams { get; set; }




}