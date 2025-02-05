namespace TheAggregate.Api.Features.Identity.Types;

public record UserWithRolesResponse
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public IList<string> Roles { get; set; } = [];
}