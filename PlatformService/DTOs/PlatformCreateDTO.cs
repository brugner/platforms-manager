using System.ComponentModel.DataAnnotations;

namespace PlatformService.DTOs;

public class PlatformCreateDTO
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string Publisher { get; set; } = default!;

    [Required]
    public string Cost { get; set; } = default!;
}