using System.ComponentModel.DataAnnotations;

namespace ShareBook.API.DTOs;

public class UpdateBookDto
{
    [MinLength(1)]
    public required string CurrentUser { get; set; } // TODO: Temporary until authentication is implemented.
    [MinLength(1)]
    public required string Title { get; set; }
    [MinLength(1)]
    public required string Author { get; set; }
    [Range(1, int.MaxValue)]
    public required int Pages { get; set; }
    public IEnumerable<string> Labels { get; set; }
    public bool SharedByOwner { get; set; }
}