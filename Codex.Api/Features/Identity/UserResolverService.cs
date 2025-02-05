namespace Codex.Api.Features.Identity;

public interface IUserResolverService
{
    string GetUserIdentityName();
}

public class UserResolverService : IUserResolverService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserResolverService> _logger;

    public UserResolverService(IHttpContextAccessor httpContextAccessor, ILogger<UserResolverService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public string GetUserIdentityName()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userIdName = string.Empty;

        if (httpContext is null)
        {
            return userIdName;
        }

        var sessionIdentity = httpContext.User.Identity;
        if (sessionIdentity is null || !sessionIdentity.IsAuthenticated)
        {
            return userIdName;
        }

        if (sessionIdentity.IsAuthenticated)
        {
            _logger.LogInformation("Session identity is authenticated");
        }

        if (sessionIdentity.Name != null)
        {
            userIdName = sessionIdentity.Name;
        }
        return userIdName;
    }

}