using CommandsService.Models;

namespace CommandsService.Data;

public class CommandRepository : ICommandRepository
{
    private readonly AppDbContext _dbContext;

    public CommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void CreateCommand(int platformId, Command command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        command.PlatformId = platformId;
        _dbContext.Commands.Add(command);
    }

    public void CreatePlatform(Platform platform)
    {
        if (platform == null)
        {
            throw new ArgumentNullException(nameof(platform));
        }

        _dbContext.Platforms.Add(platform);
    }

    public bool ExternalPlatformExists(int externalId)
    {
        return _dbContext.Platforms.Any(x => x.ExternalId == externalId);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _dbContext.Platforms.ToList();
    }

    public Command? GetCommand(int platformId, int commandId)
    {
        return _dbContext.Commands.FirstOrDefault(x => x.PlatformId == platformId && x.Id == commandId);
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return _dbContext.Commands.Where(x => x.PlatformId == platformId).OrderBy(x => x.Platform.Name);
    }

    public bool PlatformExists(int platformId)
    {
        return _dbContext.Platforms.Any(x => x.Id == platformId);
    }

    public bool SaveChanges()
    {
        return (_dbContext.SaveChanges() >= 0);
    }
}