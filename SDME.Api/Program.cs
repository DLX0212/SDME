using Microsoft.EntityFrameworkCore;
using SDME.Application.Interfaces;
using SDME.Application.Services;
using SDME.Application.Validators.Usuario;
using SDME.Domain.Interfaces;
using SDME.Persistence;
using SDME.Persistence.Context;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container
builder.Services.AddControllers();

// Configurar FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegistrarUsuarioValidator>();

// Configurar Swagger
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

// Configurar DbContext con PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SDMEDbContext>(options =>
    options.UseNpgsql(connectionString)
           .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
           .LogTo(Console.WriteLine, LogLevel.Information));

// Registrar Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Registrar servicios de aplicación
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

// Configurar CORS
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

// Configure the HTTP request pipeline
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

// Mensaje de bienvenida
app.MapGet("/", () => new
{
    mensaje = "Bienvenido a la API de D' Méndez Empanadas",
    version = "1.0",
    documentacion = "/swagger"
});

// Endpoint de salud
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