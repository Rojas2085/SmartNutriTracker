using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartNutriTracker.Web.Core.Data;
using SmartNutriTracker.Web.Core.Security;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// 1. BASE DE DATOS (SQLite por defecto para los 25-30 estudiantes; SQL Server comentada)
// ============================================================================
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var sqliteConnection = builder.Configuration.GetConnectionString("SqliteConnection")
                           ?? "Data Source=SmartNutriTracker.db";
    options.UseSqlite(sqliteConnection);

    // Para usar SQL Server local o en producción, descomenta la siguiente línea:
    // options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"));
});

// ============================================================================
// 2. SEGURIDAD DUAL: COOKIES (Vistas Razor) + JWT BEARER (API REST /api/...)
// ============================================================================
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]
    ?? "ClaveSecretaSuperSegura_SmartNutriTracker_ColegioInnovare_2026_Min32Bytes!";
var issuer = jwtSettings["Issuer"] ?? "SmartNutriTracker.Innovare";
var audience = jwtSettings["Audience"] ?? "SmartNutriTracker.Equipos";

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "SmartNutriTracker.Innovare.Auth";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.SlidingExpiration = true;
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ============================================================================
// 3. RAZOR PAGES (Soporta /Pages y /Modules/0X_...) + API CONTROLLERS + CORS
// ============================================================================
builder.Services.AddRazorPages(options =>
{
    options.RootDirectory = "/";
    options.Conventions.AddPageRoute("/Pages/Account/Login", "");
});

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ============================================================================
// 4. SWAGGER UI (/swagger) CON DEFINICIÓN DE SEGURIDAD BEARER JWT
// ============================================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SmartNutriTracker API - Colegio Innovare",
        Version = "v1.0",
        Description = "API REST Modular (6 Equipos de Desarrollo) con Autenticación JWT Bearer y Roles."
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingresa el token JWT en formato: Bearer {tu_token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// ============================================================================
// 5. SIEMBRA AUTOMÁTICA DE LAS 6 CREDENCIALES POR EQUIPO AL ARRANCAR
// ============================================================================
await DbInitializer.InitializeAsync(app.Services);

// ============================================================================
// 6. PIPELINE HTTP MIDDLEWARE
// ============================================================================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartNutriTracker API v1 - Colegio Innovare");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Swagger UI - SmartNutriTracker API";
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
