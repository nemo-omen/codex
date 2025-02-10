using System;

namespace Codex.Api.Features.Collections;

public class Collection
{
	public Guid Id { get; set; }
	public required string UserId { get; set; }
	public required string Name { get; set; }
	public string? Description { get; set; }
	// public List<Note> Notes { get; set; } = [];
	// public List<Bookmark> Bookmarks { get; set; } = [];
}
