using Microsoft.EntityFrameworkCore;
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


    }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<OrganisationModel> Organisations { get; set; }
    public DbSet<MemberModel> Members { get; set; }
    public DbSet<ProjectModel> Projects { get; set; }
    public DbSet<TeamModel> Teams { get; set; }




}