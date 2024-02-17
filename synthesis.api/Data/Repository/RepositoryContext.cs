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

        modelBuilder.Entity<ProjectModel>().OwnsOne(project => project.ProjectMetadata, OwnedNavigationBuilder =>
        {
            OwnedNavigationBuilder.ToJson();

            OwnedNavigationBuilder.OwnsOne(project => project.Overview, OwnedNavigationBuilder =>
            {
                OwnedNavigationBuilder.OwnsMany(overview => overview.SuggestedNames);
                OwnedNavigationBuilder.OwnsMany(overview => overview.SuggestedDomains);
            });

            OwnedNavigationBuilder.OwnsOne(branding => branding.Branding, OwnedNavigationBuilder =>
            {
                OwnedNavigationBuilder.OwnsOne(branding => branding.Icon);
                OwnedNavigationBuilder.OwnsMany(branding => branding.Wireframes);
                OwnedNavigationBuilder.OwnsMany(branding => branding.MoodBoards);
                OwnedNavigationBuilder.OwnsOne(branding => branding.Palette);
                OwnedNavigationBuilder.OwnsOne(branding => branding.Typography);

            });

            OwnedNavigationBuilder.OwnsOne(competitiveAnalysis => competitiveAnalysis.CompetitiveAnalysis, OwnedNavigationBuilder =>
            {
                OwnedNavigationBuilder.OwnsMany(competitiveAnalysis => competitiveAnalysis.Competitors);
                OwnedNavigationBuilder.OwnsOne(competitiveAnalysis => competitiveAnalysis.Swot);
                OwnedNavigationBuilder.OwnsOne(competitiveAnalysis => competitiveAnalysis.TargetAudience);
            });

            OwnedNavigationBuilder.OwnsOne(feature => feature.Features);

            OwnedNavigationBuilder.OwnsOne(technology => technology.Technology);

        });
    }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<OrganisationModel> Organisations { get; set; }
    public DbSet<MemberModel> Members { get; set; }
    public DbSet<ProjectModel> Projects { get; set; }
    public DbSet<TeamModel> Teams { get; set; }




}