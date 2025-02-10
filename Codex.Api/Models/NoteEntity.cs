using System;
using System.ComponentModel.DataAnnotations;

namespace Codex.Api.Models;

public class NoteEntity : BaseModel
{
	[MaxLength(256)]
	public required string Title { get; set; } = "Untitled";
	[MaxLength(int.MaxValue)]
	public string? Content { get; set; }
	public Guid? BookmarkId { get; set; }
	public BookmarkEntity? Bookmark { get; set; } = null!;
	[MaxLength(36)]
	public required string UserId { get; set; }
	public ApplicationUser User { get; set; } = null!;
	public Guid? CollectionId { get; set; }
	public CollectionEntity? Collection { get; set; }

	public ICollection<NoteLinkEntity> OutgoingLinks { get; set; } = [];
	public ICollection<NoteLinkEntity> IncomingLinks { get; set; } = [];
}
