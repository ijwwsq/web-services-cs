using System.ComponentModel.DataAnnotations;

namespace GamesApi.Models;

public class Game
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Genre { get; set; } = string.Empty;

    [Required]
    public string Developer { get; set; } = string.Empty;

    [Range(1970, 2100)]
    public int ReleaseYear { get; set; }

    [Range(0, 1000)]
    public decimal Price { get; set; }
}
