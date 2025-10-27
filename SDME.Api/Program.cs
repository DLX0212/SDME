using SDME.Infrastructure.Dependencies;
using SDME.Persistence.Context;
using FluentValidation;
using FluentValidation.AspNetCore;
using SDME.Application.Validators.Usuario;

var builder = WebApplication.CreateBuilder(args);

// CONFIGURACIÓN DE LOGGING 
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

//CONFIGURACIÓN DE SERVICIOS

// Registrar TODAS las dependencias con el método de extensión
//    Esto incluye: DbContext, Repositories, UnitOfWork, Services y Logger
builder.Services.AddDependencies(builder.Configuration);

//  Controllers
builder.Services.AddControllers();

// FluentValidation para validar DTOs
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegistrarUsuarioValidator>();

// Swagger para documentación de API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "D' Méndez Empanadas API",
        Version = "v1",
        Description = "API para la plataforma de pedidos de D' Méndez Empanadas",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Dalexa Matos",
            Email = "dalexaeloina@gmail.com"
        }
    });
});

// CORS para permitir peticiones desde el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// CONSTRUCCIÓN DE LA APP
var app = builder.Build();

// CONFIGURACIÓN DEL PIPELINE HTTP 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "D' Méndez API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

//  ENDPOINTS ADICIONALES

// Mensaje de bienvenida
app.MapGet("/", () => new
{
    mensaje = "Bienvenido a la API de D' Méndez Empanadas",
    version = "1.0",
    documentacion = "/swagger"
});

// health check
app.MapGet("/health", async (SDMEDbContext context) =>
{
    try
    {
        await context.Database.CanConnectAsync();
        return Results.Ok(new
        {
            status = "Saludable",
            database = "Conectado",
            timestamp = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Title = "Error de conexión",
            Detail = ex.Message,
            Status = 503
        });
    }
});

app.Run();