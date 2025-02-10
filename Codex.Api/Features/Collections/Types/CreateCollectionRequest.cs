namespace Codex.Api.Features.Collections.Types;

public record CreateCollectionRequest
{
    public Guid Id { get; init; }
    public string? UserId { get; set; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}
