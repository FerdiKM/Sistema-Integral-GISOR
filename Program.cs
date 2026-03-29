using GisorSystem.Repositories;
using GisorSystem.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SISTEMA_INTEGRAL_GISOR.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<GisorContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de Dependencias (Patrón Singleton / Scoped)

// 1. Repositorio Genérico
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// 2. Servicios de Negocio (Gestores y Validador)
builder.Services.AddScoped<ValidadorSistema>();

// Aquí registraremos los Gestores (Usuario, Incidente) en el siguiente paso.
builder.Services.AddScoped<GestorUsuarioService>();
builder.Services.AddScoped<GestorIncidenteService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

builder.Services.AddScoped<GisorSystem.Services.SimuladorAlertaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
