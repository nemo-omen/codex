using System;

namespace Codex.Api.Features.Bookmarks.Types;

public record UpdateBookmarkRequest
{
	public Guid Id { get; init; }
	public string? Title { get; init; }
	public string? Url { get; init; }
	public string? Description { get; init; }
}
