using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Nurtricenter.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR — register all command/query handlers
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

        // FluentValidation — register all validators
        services.AddValidatorsFromAssembly(
            typeof(AssemblyReference).Assembly,
            includeInternalTypes: true);

        return services;
    }
}
