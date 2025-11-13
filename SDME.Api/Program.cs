using FluentValidation;
using FluentValidation.AspNetCore;
using SDME.Application.Validators.Usuario;
using SDME.Infraestructure.Dependencies;
using SDME.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// DbContext, UnitOfWork, Logger
builder.Services.AddInfrastructureDependencies(builder.Configuration);
// Módulos de dominio 
builder.Services.AddUsuarioDependencies();
builder.Services.AddProductoDependencies();
builder.Services.AddCategoriaDependencies();
builder.Services.AddPedidoDependencies();
builder.Services.AddPromocionDependencies();
// Controllers con validación automática
builder.Services.AddControllers();

// FluentValidation para DTOs
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegistrarUsuarioValidator>();

// Swagger para documentación
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "D' Méndez Empanadas API",
        Version = "v1",
        Description = "API REST para la plataforma de pedidos de D' Méndez Empanadas",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Dalexa Matos",
            Email = "dalexaeloina@gmail.com"
        }
    });

});

// CORS para permitir peticiones desde frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();


// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "D' Méndez API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz (/)
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();



// Endpoint de bienvenida
app.MapGet("/api", () => new
{
    message = "Bienvenido a la API de D' Méndez Empanadas",
    version = "1.0.0",
    documentation = "/swagger",
    healthCheck = "/api/health"
}).WithTags("Root");

// Health Check - Verifica estado de la BD
app.MapGet("/api/health", async (SDMEDbContext context) =>
{
    try
    {
        await context.Database.CanConnectAsync();
        return Results.Ok(new
        {
            status = "Healthy",
            database = "Connected",
            timestamp = DateTime.UtcNow,
            environment = app.Environment.EnvironmentName
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Title = "Unhealthy",
            Detail = $"Database connection failed: {ex.Message}",
            Status = 503
        });
    }
}).WithTags("Health");

app.Run();