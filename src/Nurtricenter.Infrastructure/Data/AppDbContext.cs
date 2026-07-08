using Microsoft.EntityFrameworkCore;
using Nurtricenter.Core.Domain.Courier;
using Nurtricenter.Core.Domain.Route;

namespace Nurtricenter.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Courier> Couriers => Set<Courier>();
    public DbSet<Route> Routes => Set<Route>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
