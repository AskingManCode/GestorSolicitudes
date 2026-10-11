using PublicaSA.BusinessLogic.Interfaces;
using PublicaSA.BusinessLogic.Services;
using PublicaSA.DataAccess.Interfaces;
using PublicaSA.DataAccess.Repositories;
using PublicaSA.WebPresentation.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Habilita las páginas Razor del sistema.
builder.Services.AddRazorPages();

// Guarda los datos de sesión en la memoria del servidor.
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization();

// Obtiene la configuración privada de conexión a MySQL.
builder.Services.AddScoped<IDbConnectionFactory>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();

    var connectionString = configuration.GetConnectionString("MySql")
        ?? throw new InvalidOperationException(
            "No se encontró la configuración ConnectionStrings:MySql.");

    return new DbConnectionFactory(connectionString);
});

// Registra la bitácora utilizada por los servicios.
builder.Services.AddScoped<IBitacoraRepository,
    BitacoraRepository>();

// Registra los componentes de administración de tareas: TAR1.
builder.Services.AddScoped<ITareaSolicitudRepository,
    TareaSolicitudRepository>();

builder.Services.AddScoped<ITareaSolicitudService,
    TareaSolicitudService>();

// Registra los componentes del reporte mensual: SOL7.
builder.Services.AddScoped<IReporteMontosRepository,
    ReporteMontosRepository>();

builder.Services.AddScoped<IReporteMontosService,
    ReporteMontosService>();

// Registra los componentes de vencimiento de solicitudes: SOL5.
builder.Services.AddScoped<ISolicitudVencimientoRepository,
    SolicitudVencimientoRepository>();

builder.Services.AddScoped<ISolicitudVencimientoService,
    SolicitudVencimientoService>();

// Inicia la revisión automática junto con la aplicación.
builder.Services.AddHostedService<
    SolicitudVencimientoBackgroundService>();

var app = builder.Build();

// Configura el manejo de errores fuera del entorno de desarrollo.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Habilita la sesión antes de ejecutar las páginas.
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();