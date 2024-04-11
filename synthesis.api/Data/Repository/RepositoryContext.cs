using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Octokit;
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

        modelBuilder.Entity<InviteModel>().HasIndex(i => i.Code);

        // modelBuilder.Entity<ProjectModel>().OwnsOne(project => project.PrePlan, ownedNavigationBuilder =>
        // {
        //     ownedNavigationBuilder.ToJson();
        //
        //     ownedNavigationBuilder.OwnsOne(project => project.Overview, ownedNavigationBuilder =>
        //     {
        //         ownedNavigationBuilder.OwnsMany(overview => overview.SuggestedNames);
        //         ownedNavigationBuilder.OwnsMany(overview => overview.SuggestedDomains);
        //     });
        //
        //     ownedNavigationBuilder.OwnsOne(branding => branding.Branding, ownedNavigationBuilder =>
        //     {
        //         ownedNavigationBuilder.OwnsOne(branding => branding.Icon);
        //         ownedNavigationBuilder.OwnsOne(branding => branding.Wireframe, ownedNavigationBuilder =>
        //         {
        //             ownedNavigationBuilder.OwnsOne(wireframe => wireframe.Image);
        //         });
        //         ownedNavigationBuilder.OwnsOne(branding => branding.MoodBoard);
        //         ownedNavigationBuilder.OwnsOne(branding => branding.Palette);
        //         ownedNavigationBuilder.OwnsOne(branding => branding.Typography);
        //
        //     });
        //
        //     ownedNavigationBuilder.OwnsOne(competitiveAnalysis => competitiveAnalysis.CompetitiveAnalysis, ownedNavigationBuilder =>
        //     {
        //         ownedNavigationBuilder.OwnsMany(competitiveAnalysis => competitiveAnalysis.Competitors);
        //         ownedNavigationBuilder.OwnsOne(competitiveAnalysis => competitiveAnalysis.Swot);
        //         ownedNavigationBuilder.OwnsOne(competitiveAnalysis => competitiveAnalysis.TargetAudience, ownedNavigationBuilder =>
        //         {
        //             ownedNavigationBuilder.OwnsOne(targetAudience => targetAudience.Demographics);
        //         });
        //     });
        //
        //     ownedNavigationBuilder.OwnsOne(technology => technology.Technology, ownedNavigationBuilder =>
        //     {
        //         ownedNavigationBuilder.OwnsMany(technology => technology.TechStacks);
        //     });
        //
        // });
        
        // modelBuilder.Entity<PrePlanModel>().OwnsOne(prePlan => prePlan.Plan, ownedNavigationBuilder =>
        // {
        //     ownedNavigationBuilder.ToJson();
        //     
        //     ownedNavigationBuilder.OwnsOne(project => project.Overview, ownedNavigationBuilder =>
        //     {
        //         ownedNavigationBuilder.OwnsMany(overview => overview.SuggestedNames);
        //         ownedNavigationBuilder.OwnsMany(overview => overview.SuggestedDomains);
        //     });
        //
        //     ownedNavigationBuilder.OwnsOne(branding => branding.Branding, ownedNavigationBuilder =>
        //     {
        //         ownedNavigationBuilder.OwnsOne(branding => branding.Icon);
        //         ownedNavigationBuilder.OwnsOne(branding => branding.Wireframe, ownedNavigationBuilder =>
        //         {
        //             ownedNavigationBuilder.OwnsOne(wireframe => wireframe.Image);
        //         });
        //         ownedNavigationBuilder.OwnsOne(branding => branding.MoodBoard);
        //         ownedNavigationBuilder.OwnsOne(branding => branding.Palette);
        //         ownedNavigationBuilder.OwnsOne(branding => branding.Typography);
        //
        //     });
        //
        //     ownedNavigationBuilder.OwnsOne(competitiveAnalysis => competitiveAnalysis.CompetitiveAnalysis, ownedNavigationBuilder =>
        //     {
        //         ownedNavigationBuilder.OwnsMany(competitiveAnalysis => competitiveAnalysis.Competitors);
        //         ownedNavigationBuilder.OwnsOne(competitiveAnalysis => competitiveAnalysis.Swot);
        //         ownedNavigationBuilder.OwnsOne(competitiveAnalysis => competitiveAnalysis.TargetAudience, ownedNavigationBuilder =>
        //         {
        //             ownedNavigationBuilder.OwnsOne(targetAudience => targetAudience.Demographics);
        //         });
        //     });
        //
        //     ownedNavigationBuilder.OwnsMany(features => features.Features, ownedNavigationBuilder =>
        //     {
        //         ownedNavigationBuilder.OwnsMany(feature => feature.Tasks);
        //     });
        //     
        //     ownedNavigationBuilder.OwnsOne(technology => technology.Technology, ownedNavigationBuilder =>
        //     {
        //         ownedNavigationBuilder.OwnsMany(technology => technology.TechStacks);
        //     });
        //
        //     
        // });

        
        modelBuilder.Entity<FeatureModel>().HasMany(ft => ft.Tasks).WithOne(t => t.Feature).IsRequired(false);
        modelBuilder.Entity<MemberModel>().HasMany(m => m.Tasks).WithOne(t => t.Member).IsRequired(false);


    }

    public DbSet<UserModel> Users { get; init; }
    public DbSet<TeamModel> Teams { get; init; }
    public DbSet<InviteModel> Invites { get; init; }
    public DbSet<MemberModel> Members { get; init; }
    public DbSet<ProjectModel> Projects { get; init; }
    public DbSet<PrePlanModel> PrePlans { get; init; }
    public DbSet<FeatureModel> Features { get; init; }
    public DbSet<TaskToDoModel> Tasks { get; init; }

}