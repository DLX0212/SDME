
using FluentValidation;

using SDME.Application.Validators.Usuario;
using SDME.Infraestructure.Dependencies;

namespace SDME.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Services.AddInfrastructureDependencies(builder.Configuration);
            builder.Services.AddUsuarioDependencies();
            builder.Services.AddProductoDependencies();
            builder.Services.AddCategoriaDependencies();
            builder.Services.AddPedidoDependencies();
            builder.Services.AddPromocionDependencies();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();




            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); 
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }


            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
