namespace Codex.Api.Features.Notes.Types;

public record NoteDto
{
	public Guid Id { get; init; }
	public required string Title { get; init; }
	public string? Content { get; init; }
	public Guid? BookmarkId { get; init; }
	public required string UserId { get; init; }
	public Guid? CollectionId { get; init; }
	public List<NoteLinkDto> IncomingLinks { get; init; } = [];
	public List<NoteLinkDto> OutgoingLinks { get; init; } = [];
}
