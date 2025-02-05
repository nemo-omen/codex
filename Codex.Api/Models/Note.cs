using System;

namespace Codex.Api.Models;

public class Note : BaseModel
{
    public string Title { get; set; } = "Untitled";
    public string? Content { get; set; }
    public Guid BookmarkId { get; set; }
    public Bookmark Bookmark { get; set; }
    public required string UserId { get; set; }
    public ApplicationUser User { get; set; }
}
