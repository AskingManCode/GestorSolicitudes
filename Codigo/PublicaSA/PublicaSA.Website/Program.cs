using PublicaSA.Repository.Interfaces;
using PublicaSA.Repository.Repositories;
using PublicaSA.Services.Interfaces;
using PublicaSA.Services.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Habilita la sesión para identificar al usuario que consulta el reporte.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<IDBConnectionFactory, DBConnectionFactory>();

builder.Services.AddScoped<ISolicitudRepository, SolicitudRepository>();
builder.Services.AddScoped<IRepresentanteRepository, RepresentanteRepository>();
builder.Services.AddScoped<IEstadoSolicitudRepository, EstadoSolicitudRepository>();
builder.Services.AddScoped<IBitacoraRepository, BitacoraRepository>();

builder.Services.AddScoped<IBitacoraService, BitacoraService>();
builder.Services.AddScoped<ISolicitudService, SolicitudService>();

// Conecta la pantalla de SOL7 con su servicio y repositorio.
builder.Services.AddScoped<IReporteMontosRepository,
    ReporteMontosRepository>();

builder.Services.AddScoped<IReporteMontosService,
    ReporteMontosService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Permite leer la sesión antes de ejecutar las páginas.
app.UseSession();

app.UseAuthorization();
app.MapRazorPages();

app.Run();