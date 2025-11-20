using SDME.Infraestructure.Dependencies;

var builder = WebApplication.CreateBuilder(args);

// ===== SERVICIOS =====

//  MVC
builder.Services.AddControllersWithViews();

// Sesiones (para el carrito)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//  Configurar HttpClient para la API
builder.Services.AddHttpClient("SDMEAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5113/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// REGISTRAR SERVICIOS DE API 
builder.Services.AddScoped<SDME.Web.Services.ProductoApiService>();
builder.Services.AddScoped<SDME.Web.Services.CategoriaApiService>();
builder.Services.AddScoped<SDME.Web.Services.PedidoApiService>();
builder.Services.AddScoped<SDME.Web.Services.UsuarioApiService>();

//  Dependencias de infraestructura 
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

// Habilitar sesiones
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();