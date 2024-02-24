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
        modelBuilder.Entity<UserModel>().HasIndex(u => u.GitHubId);

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
                OwnedNavigationBuilder.OwnsMany(branding => branding.Wireframes, OwnedNavigationBuilder =>
                {
                    OwnedNavigationBuilder.OwnsOne(wireframe => wireframe.Image);
                });
                OwnedNavigationBuilder.OwnsMany(branding => branding.MoodBoards);
                OwnedNavigationBuilder.OwnsOne(branding => branding.Palette);
                OwnedNavigationBuilder.OwnsOne(branding => branding.Typography);

            });

            OwnedNavigationBuilder.OwnsOne(competitiveAnalysis => competitiveAnalysis.CompetitiveAnalysis, OwnedNavigationBuilder =>
            {
                OwnedNavigationBuilder.OwnsMany(competitiveAnalysis => competitiveAnalysis.Competitors);
                OwnedNavigationBuilder.OwnsOne(competitiveAnalysis => competitiveAnalysis.Swot);
                OwnedNavigationBuilder.OwnsOne(competitiveAnalysis => competitiveAnalysis.TargetAudience, OwnedNavigationBuilder =>
                {
                    OwnedNavigationBuilder.OwnsOne(targetAudience => targetAudience.Demographics);
                });
            });

            OwnedNavigationBuilder.OwnsOne(technology => technology.Technology, OwnedNavigationBuilder =>
            {
                OwnedNavigationBuilder.OwnsMany(technology => technology.TechStacks);
            });

        });

        modelBuilder.Entity<UserModel>().OwnsMany(user => user.Skills, OwnedNavigationBuilder =>
        {
            OwnedNavigationBuilder.ToJson();
        });
        modelBuilder.Entity<FeatureModel>().HasMany(ft => ft.Tasks).WithOne(t => t.Feature).IsRequired(false);
        modelBuilder.Entity<MemberModel>().HasMany(m => m.Tasks).WithOne(t => t.Member).IsRequired(false);

    }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<UserSessionModel> UserSessions { get; set; }
    public DbSet<TeamModel> Teams { get; set; }
    public DbSet<MemberModel> Members { get; set; }
    public DbSet<ProjectModel> Projects { get; set; }
    public DbSet<FeatureModel> Features { get; set; }
    public DbSet<TaskToDoModel> Tasks { get; set; }

}