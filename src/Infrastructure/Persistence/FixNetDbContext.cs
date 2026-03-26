using FixNet.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace FixNet.Infrastructure.Persistence;

public class FixNetDbContext(DbContextOptions<FixNetDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FixNetDbContext).Assembly);
    }
}