using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nurtricenter.Infrastructure.Data;

namespace Nurtricenter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // EF Core — NEON (PostgreSQL)
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<Core.Domain.Courier.Repositories.ICourierRepository,
                              Data.Repositories.CourierRepository>();
        services.AddScoped<Core.Domain.Route.Repositories.IRouteRepository,
                              Data.Repositories.RouteRepository>();

        // Unit of Work
        services.AddScoped<Joseco.DDD.Core.Abstractions.IUnitOfWork, Data.UnitOfWork>();

        return services;
    }
}
