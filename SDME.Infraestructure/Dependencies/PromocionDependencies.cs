using Microsoft.Extensions.DependencyInjection;
using SDME.Domain.Interfaces.Promociones;
using SDME.Persistence.Repositories.Promociones;

namespace SDME.Infraestructure.Dependencies
{
    /// Inyección de dependencias del módulo de Promoción
    public static class PromocionDependencies
    {
        public static IServiceCollection AddPromocionDependencies(this IServiceCollection services)
        {
            // Repositorio - Capa de Persistencia
            services.AddScoped<IPromocionRepository, PromocionRepository>();
            return services;
        }
    }
}
