using System;

namespace Codex.Api.Models;

public class BookmarkEntity : BaseModel
{
	public required string Title { get; set; }
	public required string Url { get; set; }
	public string? Description { get; set; }
	public required string UserId { get; set; }
	public ApplicationUser User { get; set; } = null!;
	public Guid? CollectionId { get; set; }
	public CollectionEntity? Collection { get; set; }

	public ICollection<NoteEntity>? Notes { get; set; } = [];
}
