namespace Codex.Api.Features.Notes.Types;

public record NoteLinkDto
{
	public Guid Id { get; init; }
	public required string Text { get; init; }
	public int StartIndex { get; init; }
	public int EndIndex { get; init; }
	public Guid SourceId { get; init; }
	public Guid TargetId { get; init; }
}
