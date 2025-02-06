using Codex.Api.Features.Notes.Types;

namespace Codex.Api.Features.Bookmarks.Types;

public record BookmarkResponse
{
	public Guid Id { get; init; }
	public required string Title { get; init; }
	public required string Url { get; init; }
	public string? Description { get; init; }
	public required string UserId { get; init; }
	public List<NoteResponse> Notes { get; init; } = [];
}
