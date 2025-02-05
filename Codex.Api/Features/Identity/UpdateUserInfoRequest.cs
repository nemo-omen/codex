using Microsoft.AspNetCore.Identity.Data;

namespace TheAggregate.Api.Features.Identity;

public sealed class UpdateUserInfoRequest
{
    /// <summary>
    /// The optional new email address for the authenticated user. This will replace the old email address if there was one. The email will not be updated until it is confirmed.
    /// </summary>
    public string? NewEmail { get; init; }

    /// <summary>
    /// The optional new password for the authenticated user. If a new password is provided, the <see cref="P:Microsoft.AspNetCore.Identity.Data.InfoRequest.OldPassword" /> is required.
    /// If the user forgot the old password, use the "/forgotPassword" endpoint instead.
    /// </summary>
    public string? NewPassword { get; init; }

    /// <summary>
    /// The old password for the authenticated user. This is only required if a <see cref="P:Microsoft.AspNetCore.Identity.Data.InfoRequest.NewPassword" /> is provided.
    /// </summary>
    public string? OldPassword { get; init; }

    // custom ApplicationUser properties
    public string? Name { get; init; }
}