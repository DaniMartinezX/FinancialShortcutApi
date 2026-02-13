using Microsoft.EntityFrameworkCore;
using FinancialShortcutApi.Data;
using FinancialShortcutApi.Models;
using FinancialShortcutApi.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar Entity Framework con SQLite
builder.Services.AddDbContext<FinancialDbContext>(options =>
    options.UseSqlite("Data Source=financial.db"));

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

// Crear la base de datos al iniciar
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FinancialDbContext>();
    dbContext.Database.EnsureCreated();
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
