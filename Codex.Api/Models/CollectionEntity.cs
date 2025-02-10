using System;

namespace Codex.Api.Models;

public class CollectionEntity : BaseModel
{
	public required string Name { get; set; }
	public string? Description { get; set; }
	public required string UserId { get; set; }
	public ApplicationUser User { get; set; }
	public ICollection<BookmarkEntity> Bookmarks { get; set; } = [];
	public ICollection<NoteEntity> Notes { get; set; } = [];
}
