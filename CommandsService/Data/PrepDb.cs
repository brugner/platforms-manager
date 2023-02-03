using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>()!;

            var platforms = grpcClient.ReturnAllPlatforms();

            SeedData(serviceScope.ServiceProvider.GetService<ICommandRepository>()!, platforms);
        }
    }

    private static void SeedData(ICommandRepository repository, IEnumerable<Platform> platforms)
    {
        System.Console.WriteLine("--> Seeding new platforms..");

        foreach (var platform in platforms)
        {
            if (!repository.ExternalPlatformExists(platform.Id))
            {
                repository.CreatePlatform(platform);
            }
        }

        repository.SaveChanges();
    }
}