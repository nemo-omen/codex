namespace Codex.Api.Features.Notes.Types;

public record NoteResponse
{
	public Guid Id { get; init; }
	public string? Title { get; init; }
	public string? Content { get; init; }
	public required string UserId { get; set; }
}
