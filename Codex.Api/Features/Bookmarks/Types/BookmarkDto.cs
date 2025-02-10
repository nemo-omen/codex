using System.Collections.ObjectModel;
using Codex.Api.Features.Notes.Types;
using Codex.Api.Models;

namespace Codex.Api.Features.Bookmarks.Types;

public record BookmarkDto
{
	public Guid Id { get; init; }
	public required string Title { get; init; }
	public required string Url { get; init; }
	public string? Description { get; init; }
	public required string UserId { get; init; }
	public Guid? CollectionId { get; init; }
	public CollectionEntity? Collection { get; init; }
	public List<NoteDto> Notes { get; init; } = [];
}
