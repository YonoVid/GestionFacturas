// Declaraciones para trabajar con Entity Frameworl
using Microsoft.EntityFrameworkCore;
using APIGestionFacturas.DataAccess;
using APIGestionFacturas.Services;

var builder = WebApplication.CreateBuilder(args);


// Obtener conexión a SQL
const string CONNECTIONSTRING = "DbGestionFacturas";


var connectionString = builder.Configuration.GetConnectionString(CONNECTIONSTRING);

// Establecer contexto de base de datos

builder.Services.AddDbContext<GestionFacturasContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.

builder.Services.AddControllers();

// Añadir servicios para los controladores
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEnterpriseService, EnterpriseService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Habilitar CORS
builder.Services.AddCors(options => 
    options.AddPolicy(name: "CorsPolicy", builder =>
    {
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    }
));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Indicar a la aplicación que utilize los CORS
app.UseCors("CorsPolicy");


app.MapControllers();

app.Run();
