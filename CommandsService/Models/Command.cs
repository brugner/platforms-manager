using System.ComponentModel.DataAnnotations;

namespace CommandsService.Models;

public class Command
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string HowTo { get; set; } = default!;

    [Required]
    public string CommandLine { get; set; } = default!;

    [Required]
    public int PlatformId { get; set; }
    public Platform Platform { get; set; } = default!;
}