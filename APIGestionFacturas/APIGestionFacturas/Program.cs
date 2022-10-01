// Declaraciones para trabajar con Entity Frameworl
using Microsoft.EntityFrameworkCore;
using APIGestionFacturas.DataAccess;


var builder = WebApplication.CreateBuilder(args);


// Obtener conexión a SQL
const string CONNECTIONSTRING = "DbGestionFacturas";


var connectionString = builder.Configuration.GetConnectionString(CONNECTIONSTRING);

// Establecer contexto de base de datos

builder.Services.AddDbContext<GestionFacturasContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
