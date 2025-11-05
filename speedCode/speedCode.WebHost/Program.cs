using System.Reflection;
using Microsoft.EntityFrameworkCore;
using speedCode.exercise;
using speedCode.Judge0;
using speedCode.Modularity;
using speedCode.Profile;
using speedCode.Room;
using speedCode.security;
using speedCode.Seeder;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>(optional: true);
AuthManager.ConfigureJwt(builder);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SpeedCode API", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath);
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "speedCode.WsRoom.xml"));
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "speedCode.Profile.xml"));
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "speedCode.exercise.xml"));
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "speedCode.Room.xml"));
});


builder.Services.AddControllers()
    .AddApplicationPart(typeof(speedCode.Profile.ProfileModule).Assembly)
    .AddApplicationPart(typeof(speedCode.exercise.exerciseModule).Assembly)
    .AddApplicationPart(typeof(speedCode.Judge0.Judge0Module).Assembly)
    .AddApplicationPart(typeof(speedCode.Room.RoomsModule).Assembly)
    .AddApplicationPart(typeof(speedCode.WsRoom.WsRoomModule).Assembly)
    .AddControllersAsServices()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.MaxDepth = 64;
    });

builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(8080); });
ModuleLoader.RegisterModules(builder.Services, builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Seeding database");
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        var roomsDb = serviceProvider.GetRequiredService<RoomsDb>();
        var exerciseDb = serviceProvider.GetRequiredService<ExerciseDb>();
        var profileDb = serviceProvider.GetRequiredService<ProfileDb>();
        //seed database
        SeederManager
            .CreateSupabaseAuthUsersAsync(builder.Configuration["Supabase:Url"],
                builder.Configuration["Supabase:ServiceRoleKey"]).GetAwaiter().GetResult();
        SeederManager.SeedDatabase(roomsDb, exerciseDb, profileDb);
    }
}

app.UseWebSockets();
// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();

public partial class Program
{
}