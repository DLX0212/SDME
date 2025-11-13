using Microsoft.Extensions.DependencyInjection;
using SDME.Application.Interfaces;
using SDME.Application.Services;
using SDME.Domain.Interfaces.Core;
using SDME.Persistence.Repositories.Core;

namespace SDME.Infraestructure.Dependencies
{

 // Inyección de dependencias del módulo de Usuario

    public static class UsuarioDependencies
    {
        public static IServiceCollection AddUsuarioDependencies(this IServiceCollection services)
        {
            // Repositorio - Capa de Persistencia
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            // Servicio - Capa de Aplicacion
            services.AddTransient<IUsuarioService, UsuarioService>();

            return services;
        }
    }
}
