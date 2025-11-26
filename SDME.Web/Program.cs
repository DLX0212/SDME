using SDME.Infraestructure.Dependencies;
using SDME.Web.Services;
using SDME.Web.Services.Base;
using SDME.Web.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Sesiones (para el carrito)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Config HttpClient para consumir la API REST
builder.Services.AddHttpClient("SDMEAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5113/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// HttpClient: Singleton (reutilizable y eficiente)
builder.Services.AddSingleton<IHttpClientService, HttpClientService>();

// Servicios de negocio: Scoped (aislados por request)
builder.Services.AddScoped<IProductoService, ProductoApiService>();
builder.Services.AddScoped<ICategoriaService, CategoriaApiService>();
builder.Services.AddScoped<IPedidoService, PedidoApiService>();
builder.Services.AddScoped<IUsuarioService, UsuarioApiService>();

// Dependencias
builder.Services.AddInfrastructureDependencies(builder.Configuration);
builder.Services.AddCategoriaDependencies();
builder.Services.AddProductoDependencies();
builder.Services.AddUsuarioDependencies();
builder.Services.AddPedidoDependencies();
builder.Services.AddPromocionDependencies();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();