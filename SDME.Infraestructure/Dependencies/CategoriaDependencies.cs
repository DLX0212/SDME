using Microsoft.Extensions.DependencyInjection;
using SDME.Application.Interfaces;
using SDME.Application.Services;
using SDME.Domain.Interfaces.Core;
using SDME.Persistence.Repositories.Core;

namespace SDME.Infraestructure.Dependencies
{
// Inyección de dependencias del módulo de Categoría
 
    public static class CategoriaDependencies
    {
        public static IServiceCollection AddCategoriaDependencies(this IServiceCollection services)
        {
            // Repositorio - Capa de Persistencia
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();

            // Servicio - Capa de Aplicación
            services.AddTransient<ICategoriaService, CategoriaService>();

            return services;
        }
    }
}
