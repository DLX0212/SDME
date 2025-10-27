using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SDME.Application.Interfaces;
using SDME.Application.Logging;
using SDME.Application.Services;
using SDME.Domain.Interfaces;
using SDME.Domain.Interfaces.Core;
using SDME.Domain.Interfaces.Pagos;
using SDME.Domain.Interfaces.Promociones;
using SDME.Persistence;
using SDME.Persistence.Context;
using SDME.Persistence.Repositories.Core;
using SDME.Persistence.Repositories.Pagos;
using SDME.Persistence.Repositories.Promociones;

namespace SDME.Infrastructure.Dependencies
{
    
    /// Clase para registrar todas las dependencias del proyecto
    /// Usa un método de extensión (this) para poder llamarlo desde Program.cs
  
    public static class DependencyInjection
    {
        
        /// Método de extensión que registra TODAS las dependencias
        /// Se llama con: builder.Services.AddDependencies(builder.Configuration);
        
        public static IServiceCollection AddDependencies(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 1. PRIMERO: Context de la base de datos
            services.AddDbContext<SDMEDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection")
                )
            );

            // 2. SEGUNDO: Repositorios - CAPA DE PERSISTENCIA
            //    AddScoped = Una instancia por HTTP request
            //    Se usa para ACCESO A DATOS (repositorios)
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IPagoRepository, PagoRepository>();
            services.AddScoped<IPromocionRepository, PromocionRepository>();

            // 3. Unit of Work - COORDINADOR DE REPOSITORIOS
            //    También Scoped porque maneja la transacción del request completo
            //    IMPORTANTE: Debe estar DESPUÉS de los repositorios
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 4. TERCERO: Services - CAPA DE APLICACIÓN (LÓGICA DE NEGOCIO)
            //    AddTransient = Nueva instancia cada vez que se necesita
            //    Se usa para SERVICIOS sin estado (stateless)
            //    Aquí van TODAS las validaciones de negocio del SRS/SAD
            services.AddTransient<IUsuarioService, UsuarioService>();
            services.AddTransient<IProductoService, ProductoService>();
            services.AddTransient<ICategoriaService, CategoriaService>();
            services.AddTransient<IPedidoService, PedidoService>();

            // 5. CUARTO: Componentes transversales - INFRAESTRUCTURA
            //    Estos se usan en TODA la aplicación (cross-cutting concerns)

            // Logger: Scoped para trazabilidad por request
            // El typeof(...) es para tipos genéricos IAppLogger<T>
            services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));

            return services;
        }
    }
}