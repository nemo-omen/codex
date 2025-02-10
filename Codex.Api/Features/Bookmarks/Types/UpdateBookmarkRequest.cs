using System;
using Codex.Api.Features.Notes.Types;

namespace Codex.Api.Features.Bookmarks.Types;

public record UpdateBookmarkRequest
{
	public Guid Id { get; init; }
	public string? Title { get; init; }
	public string? Url { get; init; }
	public string? Description { get; init; }
	public string? UserId { get; init; }
	public Guid? CollectionId { get; init; }
	public List<NoteDto> Notes { get; init; } = [];
}
