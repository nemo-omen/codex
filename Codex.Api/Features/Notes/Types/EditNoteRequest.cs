using Codex.Api.Models;

namespace Codex.Api.Features.Notes.Types;

public record EditNoteRequest
{
	public Guid Id { get; set; }
	public string? Title { get; set; }
	public string? Content { get; set; }
	public Guid? BookmarkId { get; set; }
	public required string UserId { get; set; }
	public List<EditNoteLinkRequest>? IncomingLinks { get; set; }
	public List<EditNoteLinkRequest>? OutgoingLinks { get; set; }
}
