using SDME.Infrastructure.Dependencies;
using SDME.Persistence.Context;
using FluentValidation;
using FluentValidation.AspNetCore;
using SDME.Application.Validators.Usuario;

var builder = WebApplication.CreateBuilder(args);

// CONFIGURACI�N DE LOGGING 
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

//CONFIGURACI�N DE SERVICIOS

// Registrar TODAS las dependencias con el m�todo de extensi�n
//    Esto incluye: DbContext, Repositories, UnitOfWork, Services y Logger
builder.Services.AddDependencies(builder.Configuration);

//  Controllers
builder.Services.AddControllers();

// FluentValidation para validar DTOs
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegistrarUsuarioValidator>();

// Swagger para documentaci�n de API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "D' M�ndez Empanadas API",
        Version = "v1",
        Description = "API para la plataforma de pedidos de D' M�ndez Empanadas",
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

// CONSTRUCCI�N DE LA APP
var app = builder.Build();

// CONFIGURACI�N DEL PIPELINE HTTP 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "D' M�ndez API v1");
        c.RoutePrefix = string.Empty; // Swagger en la ra�z
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
    mensaje = "Bienvenido a la API de D' M�ndez Empanadas",
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
            Title = "Error de conexi�n",
            Detail = ex.Message,
            Status = 503
        });
    }
});

app.Run();