using FluentValidation;
using Microsoft.AspNetCore.Connections;
using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Repository.Repositories;
using PublicaSA.Services.Interfaces;
using PublicaSA.Services.Services;
using PublicaSA.Website.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioRequestValidator>();
// Permite guardar el identificador del usuario en la sesión.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Conserva la conexión a la base de datos del proyecto.
builder.Services.AddSingleton<IDBConnectionFactory, DBConnectionFactory>();
builder.Services.AddScoped<IEstadosUsuarioRepository, EstadosUsuarioRepository>();
builder.Services.AddScoped<IEstadosUsuarioService, EstadosUsuarioService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IValidator<UsuarioEditRequest>, UsuarioEditValidator>();

builder.Services.AddScoped<ISolicitudRepository, SolicitudRepository>();
builder.Services.AddScoped<IRepresentanteRepository, RepresentanteRepository>();
builder.Services.AddScoped<IEstadoSolicitudRepository, EstadoSolicitudRepository>();
builder.Services.AddScoped<IBitacoraRepository, BitacoraRepository>();
builder.Services.AddScoped<IDesgloseRepository, DesgloseRepository>();
builder.Services.AddScoped<IReporteTareaRepository, ReporteTareaRepository>();

// Inyección de dependencias - Servicios
builder.Services.AddScoped<IReporteTareaService, ReporteTareaService>();
builder.Services.AddScoped<IBitacoraService, BitacoraService>();
builder.Services.AddScoped<ISolicitudService, SolicitudService>();
builder.Services.AddScoped<IDesgloseService, DesgloseService>();
builder.Services.AddScoped<IRepresentanteService, RepresentanteService>();

// Conecta las pantallas de TAR1 con su servicio y repositorio.
builder.Services.AddScoped<ITareaRepository, TareaRepository>();
builder.Services.AddScoped<ITareaService, TareaService>();
// Registra el acceso a datos y la lógica del vencimiento: SOL5.
builder.Services.AddScoped<ISolicitudVencimientoRepository,
    SolicitudVencimientoRepository>();

builder.Services.AddScoped<ISolicitudVencimientoService,
    SolicitudVencimientoService>();

// Ejecuta la revisión automática al iniciar la aplicación.
builder.Services.AddHostedService<
    SolicitudVencimientoBackgroundService>();

// Registra el repositorio y servicio de SOL7.
builder.Services.AddScoped<IReporteMontosRepository,
    ReporteMontosRepository>();

builder.Services.AddScoped<IReporteMontosService,
    ReporteMontosService>();



var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Habilita HttpContext.Session antes de ejecutar las páginas.
app.UseSession();

app.UseAuthorization();
app.MapRazorPages();

app.Run();
