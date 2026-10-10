using Microsoft.AspNetCore.Connections;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Repository.Repositories;

var builder = WebApplication.CreateBuilder(args);

// CORS (Permite que cualquier origen consuma el API)
builder.Services.AddRazorPages();

// FluentValidation
// builder.Services.AddValidatorsFromAssemblyContaining<Clase>();

// Inyección de dependencias
builder.Services.AddSingleton<IDBConnectionFactory, DBConnectionFactory>();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();