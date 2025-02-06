namespace Codex.Api.Features.Notes.Types;

public record CreateNoteRequest
{
	public required string Title { get; set; }
	public required string Content { get; set; }
	public required string UserId { get; set; }
	public Guid BookmarkId { get; set; }
}
