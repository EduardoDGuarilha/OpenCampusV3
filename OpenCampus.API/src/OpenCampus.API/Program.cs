using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenCampus.API.Configuration.DependencyInjection;
using OpenCampus.API.Data;
using OpenCampus.API.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

// DB
var connectionString = configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=opencampus.db";

services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// AutoMapper
services.AddAutoMapper(typeof(Program));

services.AddScoped<DatabaseSeeder>();

// Controllers
services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Swagger
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// CORS para o frontend
services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Migrations + Seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Swagger UI apenas em dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
