using Microsoft.EntityFrameworkCore;
using FinancialShortcutApi.Data;
using FinancialShortcutApi.Models;
using FinancialShortcutApi.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar Entity Framework con SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Validar que existe la cadena de conexión
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "No se encontró la cadena de conexión 'DefaultConnection'. " +
        "Asegúrate de configurarla en Azure App Service Configuration → Connection strings.");
}

// Log de la cadena de conexión (ocultando la password para seguridad)
var sanitizedConnection = connectionString.Contains("Password=")
    ? System.Text.RegularExpressions.Regex.Replace(connectionString, @"Password=[^;]+", "Password=***")
    : connectionString;

Console.WriteLine($"[INFO] Cadena de conexión: {sanitizedConnection}");

builder.Services.AddDbContext<FinancialDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Aplicar migraciones automáticamente al iniciar (con manejo de errores)
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<FinancialDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Aplicando migraciones a la base de datos...");
        dbContext.Database.Migrate();
        logger.LogInformation("Migraciones aplicadas correctamente.");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error al aplicar migraciones a la base de datos: {Message}", ex.Message);
    throw; // Lanzar la excepción para que Azure muestre el error específico
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();


// Endpoint para crear transacciones
app.MapPost("/api/transacciones", async (TransaccionDto dto, FinancialDbContext db) =>
{
    var transaccion = new Transaccion
    {
        TipoMovimiento = dto.tipo_movimiento,
        Categoria = dto.categoria,
        Cantidad = dto.cantidad,
        Fecha = DateTime.UtcNow
    };

    db.Transacciones.Add(transaccion);
    await db.SaveChangesAsync();

    return Results.Created($"/api/transacciones/{transaccion.Id}", transaccion);
})
.WithName("CrearTransaccion")
.WithOpenApi();

// Endpoint para obtener todas las transacciones
app.MapGet("/api/transacciones", async (FinancialDbContext db) =>
{
    var transacciones = await db.Transacciones.ToListAsync();
    return Results.Ok(transacciones);
})
.WithName("ObtenerTransacciones")
.WithOpenApi();

app.Run();
