using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class DbSeeder
{
    public static void Seed(IApplicationBuilder applicationBuilder, IWebHostEnvironment environment)
    {
        using (var scope = applicationBuilder.ApplicationServices.CreateScope())
        {
            using (var dbContext = scope.ServiceProvider.GetService<AppDbContext>()!)
            {
                SeedData(dbContext, environment);
            }
        }
    }

    private static void SeedData(AppDbContext dbContext, IWebHostEnvironment environment)
    {
        if (environment.IsProduction())
        {
            try
            {
                System.Console.WriteLine("Applying migrations");
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Could not apply migrations: {ex}");
                throw;
            }
        }

        if (!dbContext.Platforms.Any())
        {
            Console.WriteLine("Seeding data..");
            dbContext.Platforms.Add(new Platform { Name = "Platform 1", Publisher = "Publisher 1", Cost = "High" });
            dbContext.Platforms.Add(new Platform { Name = "Platform 2", Publisher = "Publisher 2", Cost = "Medium" });
            dbContext.Platforms.Add(new Platform { Name = "Platform 3", Publisher = "Publisher 3", Cost = "Low" });

            dbContext.SaveChanges();
        }
    }
}