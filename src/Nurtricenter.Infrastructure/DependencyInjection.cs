using Joseco.DDD.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nurtricenter.Core.Domain.Courier.Repositories;
using Nurtricenter.Core.Domain.Route.Repositories;
using Nurtricenter.Core.Interfaces.Services.ClinicService;
using Nurtricenter.Infrastructure.Data;
using Nurtricenter.Infrastructure.Data.Repositories;
using Nurtricenter.Core.Options;
using Nurtricenter.Infrastructure.Options;
using Nurtricenter.Infrastructure.Services;

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
        services.AddScoped<ICourierRepository,
                              CourierRepository>();
        services.AddScoped<IRouteRepository,
                              RouteRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Options
        services.Configure<ClinicServiceOptions>(
            configuration.GetSection(ClinicServiceOptions.SectionName));

        services.Configure<BranchCoordinatesOptions>(
            configuration.GetSection(BranchCoordinatesOptions.SectionName));

        // External services

        services.AddHttpClient<IClinicService, ClinicService>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ClinicServiceOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        return services;
    }
}
