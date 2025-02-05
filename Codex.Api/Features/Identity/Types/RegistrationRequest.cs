namespace Codex.Api.Features.Identity.Types;

public record RegistrationRequest
{
    public string? Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}