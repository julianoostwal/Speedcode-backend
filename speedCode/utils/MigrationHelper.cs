using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using speedCode.Data;

static class MigrationHelper
{
    public static void ApplyMigrations(WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return;

        CreateMigration();
        UpdateDatabase(app);
    }

    private static void CreateMigration()
    {
        Console.Write("Enter migration name: ");
        var migrationName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(migrationName))
        {
            migrationName = DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"ef migrations add {migrationName}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false
            }
        };

        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        Console.WriteLine(output);

        if (!string.IsNullOrEmpty(error))
            Console.WriteLine("Error: " + error);
    }

    private static void UpdateDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
}