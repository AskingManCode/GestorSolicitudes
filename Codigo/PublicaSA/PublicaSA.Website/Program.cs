using Microsoft.AspNetCore.Connections;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Repository.Repositories;
using PublicaSA.Services.Interfaces;
using PublicaSA.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// CORS / Razor Pages
builder.Services.AddRazorPages();

// Inyección de dependencias - Conexión DB
builder.Services.AddSingleton<IDBConnectionFactory, DBConnectionFactory>();

// Inyección de dependencias - Repositorios
builder.Services.AddScoped<ISolicitudRepository, SolicitudRepository>();
builder.Services.AddScoped<IRepresentanteRepository, RepresentanteRepository>();
builder.Services.AddScoped<IEstadoSolicitudRepository, EstadoSolicitudRepository>();
builder.Services.AddScoped<IBitacoraRepository, BitacoraRepository>();
builder.Services.AddScoped<IReporteTareaRepository, ReporteTareaRepository>();

// Inyección de dependencias - Servicios
builder.Services.AddScoped<IReporteTareaService, ReporteTareaService>();
builder.Services.AddScoped<IBitacoraService, BitacoraService>();
builder.Services.AddScoped<ISolicitudService, SolicitudService>();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();