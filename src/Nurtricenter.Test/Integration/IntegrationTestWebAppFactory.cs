namespace Nurtricenter.Test.Integration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Nurtricenter.Core.Interfaces.Services.ClinicService;
using Nurtricenter.Infrastructure.Data;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>
{
    public Mock<IClinicService> ClinicServiceMock { get; } = new Mock<IClinicService> { DefaultValue = DefaultValue.Mock };

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var efServices = services.Where(s => s.ServiceType.Namespace != null && s.ServiceType.Namespace.StartsWith("Microsoft.EntityFrameworkCore")).ToList();
            foreach (var s in efServices) services.Remove(s);
            
            var npgsqlServices = services.Where(s => s.ServiceType.Namespace != null && s.ServiceType.Namespace.StartsWith("Npgsql")).ToList();
            foreach (var s in npgsqlServices) services.Remove(s);

            string dbName = "TestDb_" + Guid.NewGuid().ToString();
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(dbName));

            services.RemoveAll(typeof(IClinicService));
            services.AddSingleton(ClinicServiceMock.Object);
        });
    }
}
