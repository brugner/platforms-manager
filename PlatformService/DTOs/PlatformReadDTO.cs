namespace PlatformService.DTOs;

public class PlatformReadDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Publisher { get; set; } = default!;
    public string Cost { get; set; } = default!;
}