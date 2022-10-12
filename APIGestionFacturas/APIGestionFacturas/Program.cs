// Imports to work with Entity Framework
using Microsoft.EntityFrameworkCore;
using APIGestionFacturas.DataAccess;
using APIGestionFacturas.Services;
using Microsoft.OpenApi.Models;
using DinkToPdf.Contracts;
using DinkToPdf;


var builder = WebApplication.CreateBuilder(args);


// Get SQL connection
const string CONNECTIONSTRING = "DbGestionFacturas";


var connectionString = builder.Configuration.GetConnectionString(CONNECTIONSTRING);

// Set context of data base

builder.Services.AddDbContext<GestionFacturasContext>(options => options.UseSqlServer(connectionString));

// Add JWT service authorization
builder.Services.AddJwtTokenService(builder.Configuration);


// Add services to the container.

builder.Services.AddControllers();

// Service to access HttpContext
builder.Services.AddHttpContextAccessor();

// Add services for the controllers
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEnterpriseService, EnterpriseService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IInvoiceLineService, InvoiceLineService>();

// Add authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnlyPolicy", policy => policy.RequireClaim("UserOnly", "Admin"));
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Configure Swagger in charge of the authorization
builder.Services.AddSwaggerGen(options =>
{
    // Define security settings
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Header de autenticación JWT usando esquema Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// Enable CORS
builder.Services.AddCors(options => 
    options.AddPolicy(name: "CorsPolicy", policy =>
        policy.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader()
));

// Include pdf generation service
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Tell app to use CORS
app.UseCors("CorsPolicy");


app.MapControllers();

app.Run();
