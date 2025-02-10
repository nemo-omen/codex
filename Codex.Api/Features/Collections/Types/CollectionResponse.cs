using Codex.Api.Features.Bookmarks.Types;
using Codex.Api.Features.Notes.Types;

namespace Codex.Api.Features.Collections.Types;

public record CollectionResponse
{
	public required string UserId { get; init; }
	public Guid Id { get; init; }
	public required string Name { get; init; }
	public string? Description { get; init; }
	public List<Guid> Notes { get; init; } = [];
	public List<Guid> Bookmarks { get; init; } = [];
}
