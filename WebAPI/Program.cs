/*Este archivo es el punto de entrada de tu aplicación ASP.NET Core y 
 * configura toda la infraestructura del WebAPI*/

using System.Text.Json;
using System.Text.Json.Serialization;
using Data.DBConext;
using Microsoft.EntityFrameworkCore;
using Services.MaterialsType;
using Services.Recolections.cs.Services;
using Services.Users;

//Crea el constructor de la aplicación web con la configuración inicial
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Registra los servicios necesarios para los controladores MVC
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; //Ignora ciclos de referencia en la serialización JSON
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; //Convierte las propiedades de los objetos a camelCase
    });

//Configura Swagger/OpenAPI para documentación de la API (probablemente usando una librería como Swashbuckle)
builder.Services.AddOpenApi();

//Registra tu DbContext (RecolectionProjectContext) en el sistema de inyección de dependencias
//Especifica que usarás SQL Server como base de datos
//La cadena de conexión se obtiene de appsettings.json bajo la clave "Database"
builder.Services.AddDbContext<RecollectionProjectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

//Registra los servicios de la capa de servicios (en este caso, el de usuarios)
builder.Services.AddScoped<IUserService, UserService>();

//Registra los servicios de la capa de servicios (en este caso, el de recolecciones)
builder.Services.AddScoped<IRecolectionService, RecolectionService>();

//Registra los servicios de la capa de servicios (en este caso, el de tipos de materiales)
builder.Services.AddScoped<IMaterialsTypeService, MaterialTypeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
