namespace Codex.Api.Features.Bookmarks.Types;

public record class CreateBookmarkRequest
{
    public Guid? Id { get; set; }
    public required string Title { get; set; }
    public required string Url { get; set; }
    public string? Description { get; set; }
    public string? UserId { get; set; }
}
