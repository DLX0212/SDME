using Microsoft.Extensions.DependencyInjection;
using SDME.Application.Interfaces;
using SDME.Application.Services;
using SDME.Domain.Interfaces.Core;
using SDME.Domain.Interfaces.Pagos;
using SDME.Persistence.Repositories.Core;
using SDME.Persistence.Repositories.Pagos;

namespace SDME.Infraestructure.Dependencies
{
// Inyección de dependencias del módulo de Pedido

    public static class PedidoDependencies
    {
        public static IServiceCollection AddPedidoDependencies(this IServiceCollection services)
        {
            // Repositorios - Capa de Persistencia
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IPagoRepository, PagoRepository>();

            // Servicios - Capa de Aplicación
            services.AddTransient<IPedidoService, PedidoService>();
            return services;
        }
    }
}
