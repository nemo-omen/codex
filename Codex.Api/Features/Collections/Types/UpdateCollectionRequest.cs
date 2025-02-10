using Codex.Api.Features.Bookmarks.Types;
using Codex.Api.Models;

namespace Codex.Api.Features.Collections.Types;

public record UpdateCollectionRequest
{
	public Guid Id { get; init; }
	public string? UserId { get; set; }
	public string? Name { get; init; }
	public string? Description { get; init; }
	public List<string> Bookmarks { get; init; } = [];
	public List<string> Notes { get; init; } = [];
}
