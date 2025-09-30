using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using speedCode.codeTesting;
using speedCode.Data;
using speedCode.security;
using speedCode.Seeder;
using speedCode.services;

var builder = WebApplication.CreateBuilder(args);
AuthManager.ConfigureJwt(builder);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(8080); });


builder.Services.AddSingleton<Judge0Client>(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri(builder.Configuration["Judge0:BaseUrl"]) };
    return new Judge0Client(httpClient);
});
builder.Services.AddTransient<JudgeCodeService>();


var app = builder.Build();

// MigrationHelper.ApplyMigrations(app);

// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     SeederManager.SeedDatabase(db);
// }

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();