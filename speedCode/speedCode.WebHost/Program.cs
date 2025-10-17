using Microsoft.EntityFrameworkCore;
using speedCode.Excise;
using speedCode.Excise.service;
using speedCode.Judge0;
using speedCode.Modularity;
using speedCode.Profile;
using speedCode.Room;
using speedCode.rooms;
using speedCode.security;
using speedCode.Seeder;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>(optional: true);
AuthManager.ConfigureJwt(builder);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddControllers()
    .AddApplicationPart(typeof(speedCode.Profile.ProfileModule).Assembly)
    .AddApplicationPart(typeof(speedCode.Excise.ExciseModule).Assembly)
    .AddApplicationPart(typeof(speedCode.Judge0.Judge0Module).Assembly)
    .AddApplicationPart(typeof(speedCode.Room.RoomsModule).Assembly)
    .AddControllersAsServices()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(8080); });
ModuleLoader.RegisterModules(builder.Services, builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        var roomsDb = serviceProvider.GetRequiredService<RoomsDb>();
        var exciseDb = serviceProvider.GetRequiredService<ExciseDb>();
        var profileDb = serviceProvider.GetRequiredService<ProfileDb>();
 //seed database
        SeederManager
            .CreateSupabaseAuthUsersAsync(builder.Configuration["Supabase:Url"],
                builder.Configuration["Supabase:ServiceRoleKey"]).GetAwaiter().GetResult();
        SeederManager.SeedDatabase(roomsDb, exciseDb, profileDb);
    }
}

app.UseWebSockets();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();

public partial class Program
{
}