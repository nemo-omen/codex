using System;

namespace Codex.Api.Models;

public class Collection : BaseModel
{
	public required string Name { get; set; }
	public string? Description { get; set; }
	public required string UserId { get; set; }
	public ApplicationUser User { get; set; }
	public ICollection<Bookmark> Bookmarks { get; set; } = [];
	public ICollection<Note> Notes { get; set; } = [];
}
