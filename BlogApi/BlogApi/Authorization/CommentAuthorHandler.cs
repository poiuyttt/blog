using BlogApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogApi.Authorization;

public class CommentAuthorHandler : AuthorizationHandler<CommentAuthorRequirement>
{
    private readonly AppDbContext _context;

    public CommentAuthorHandler(AppDbContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CommentAuthorRequirement requirement
    )
    {
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        var httpContext = context.Resource as HttpContext;
        var routeData = httpContext?.GetRouteData();
        var commentIdStr = routeData?.Values["id"]?.ToString();

        if (
            string.IsNullOrEmpty(commentIdStr) || !int.TryParse(commentIdStr, out var commentId)
        )
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

        var comment = await _context
            .Comments.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == commentId);
        if (comment == null || comment.UserId != userId)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}
