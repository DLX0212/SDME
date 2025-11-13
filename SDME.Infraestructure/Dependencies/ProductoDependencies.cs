using Microsoft.Extensions.DependencyInjection;
using SDME.Application.Interfaces;
using SDME.Application.Services;
using SDME.Domain.Interfaces.Core;
using SDME.Persistence.Repositories.Core;

namespace SDME.Infraestructure.Dependencies
{
// Inyección de dependencias del módulo de Producto

    public static class ProductoDependencies
    {
        public static IServiceCollection AddProductoDependencies(this IServiceCollection services)
        {
            // Repositorio - Capa de Persistencia
            services.AddScoped<IProductoRepository, ProductoRepository>();

            // Servicio - Capa de Aplicación
            services.AddTransient<IProductoService, ProductoService>();

            return services;
        }
    }
}
