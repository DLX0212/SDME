using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SDME.Application.Logging;
using SDME.Domain.Interfaces;
using SDME.Persistence;
using SDME.Persistence.Context;

namespace SDME.Infraestructure.Dependencies
{
// Registro de dependencias transversales Incluye: DbContext, UnitOfWork, Logger
 
    public static class InfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            //DbContext
            services.AddDbContext<SDMEDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("SDME.Persistence")
                )
            );

            //Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Logger
            services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));

            return services;
        }
    }
}
