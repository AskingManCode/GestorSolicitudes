var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configuración de Base de Datos (Dapper / MySQL)
builder.Services.AddSingleton<PublicaSA.DataAccess.Interfaces.IDbConnectionFactory, PublicaSA.DataAccess.Repositories.DbConnectionFactory>();

// Repositorios
builder.Services.AddScoped<PublicaSA.DataAccess.Interfaces.ISolicitudRepository, PublicaSA.DataAccess.Repositories.SolicitudRepository>();
builder.Services.AddScoped<PublicaSA.DataAccess.Interfaces.IRepresentanteRepository, PublicaSA.DataAccess.Repositories.RepresentanteRepository>();
builder.Services.AddScoped<PublicaSA.DataAccess.Interfaces.IEstadoSolicitudRepository, PublicaSA.DataAccess.Repositories.EstadoSolicitudRepository>();
builder.Services.AddScoped<PublicaSA.DataAccess.Interfaces.IBitacoraRepository, PublicaSA.DataAccess.Repositories.BitacoraRepository>();

// Servicios de Negocio
builder.Services.AddScoped<PublicaSA.BusinessLogic.Interfaces.IBitacoraService, PublicaSA.BusinessLogic.Services.BitacoraService>();
builder.Services.AddScoped<PublicaSA.BusinessLogic.Interfaces.ISolicitudService, PublicaSA.BusinessLogic.Services.SolicitudService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
