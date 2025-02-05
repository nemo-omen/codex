using TheAggregate.Api.Shared.Util;

namespace TheAggregate.Api.Features.Identity;

public interface IUserResolverService
{
    string GetUserIdentityName();
}

public class UserResolverService : IUserResolverService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserResolverService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
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
            Banner.Log(sessionIdentity.Name ?? "No name???");
        }

        if (sessionIdentity.Name != null)
        {
            userIdName = sessionIdentity.Name;
        }
        return userIdName;
    }

}