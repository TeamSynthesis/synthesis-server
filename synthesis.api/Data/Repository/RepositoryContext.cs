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
    }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<OrganisationModel> Organisations { get; set; }
    public DbSet<MemberModel> Members { get; set; }


}