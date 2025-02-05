namespace Codex.Api.Features.Identity.Types;

public record UserInfoRequest
{
    public required string Email { get; init; }
}