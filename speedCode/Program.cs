using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using speedCode.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(8080); });

// auto migration
var app = builder.Build();

MigrationHelper.ApplyMigrations(app);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();