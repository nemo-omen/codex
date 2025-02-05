using System;

namespace Codex.Api.Models;

public class Bookmark : BaseModel
{
    public required string Title { get; set; }
    public required string Url { get; set; }
    public string? Description { get; set; }
    public required string UserId { get; set; }
    public ApplicationUser User { get; set; }

    public ICollection<Note>? Notes { get; set; } = [];
}
