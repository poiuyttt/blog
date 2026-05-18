using BlogApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogApi.Authorization;

public class IsAuthorHandler : AuthorizationHandler<IsAuthorRequirement>
{
    private readonly AppDbContext _context;

    public IsAuthorHandler(AppDbContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        IsAuthorRequirement requirement
    )
    {
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        var httpContext = context.Resource as HttpContext;
        var routeData = httpContext?.GetRouteData();
        var postIdStr = routeData?.Values["id"]?.ToString();

        if (string.IsNullOrEmpty(postIdStr) || !int.TryParse(postIdStr, out var postId))
        {
            context.Fail();
            return;
        }

        var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            context.Fail();
            return;
        }

        var post = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == postId);
        if (post == null || post.UserId != userId)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}
