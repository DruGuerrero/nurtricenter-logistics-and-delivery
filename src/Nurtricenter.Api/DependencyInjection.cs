using FluentValidation.AspNetCore;
using Scalar.AspNetCore;
using Serilog;

namespace Nurtricenter.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();

        // FluentValidation — auto-validate requests in MVC pipeline
        services.AddFluentValidationAutoValidation();

        services.AddOpenApi();

        services.AddAuthorization();

        return services;
    }

    public static WebApplication UseApiMiddleware(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.MapGet("/", () => Results.Redirect("/scalar/v1"));
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.Title = "Nurtricenter API";
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
