using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformRepository : IPlatformRepository
{
    private readonly AppDbContext _dbContext;

    public PlatformRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Create(Platform platform)
    {
        if (platform == null)
        {
            throw new ArgumentNullException(nameof(platform));
        }

        _dbContext.Platforms.Add(platform);
    }

    public IEnumerable<Platform> GetAll()
    {
        return _dbContext.Platforms.ToList();
    }

    public Platform? GetById(int id)
    {
        return _dbContext.Platforms.FirstOrDefault(x => x.Id == id);
    }

    public bool SaveChanges()
    {
        return (_dbContext.SaveChanges() >= 0);
    }
}