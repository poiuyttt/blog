using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogApi.Filters;

/// <summary>
/// 动作过滤器：自动记录每个 Action 的名称、参数和执行耗时
/// 实现 IActionFilter 接口
/// </summary>
public class ActionLoggingFilter : IActionFilter
{
    private readonly ILogger<ActionLoggingFilter> _logger;

    /// <summary>
    /// 构造函数注入日志器
    /// </summary>
    public ActionLoggingFilter(ILogger<ActionLoggingFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 在 Action 方法执行**之前**被调用
    /// </summary>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        string actionName = context.ActionDescriptor.DisplayName;

        var arguments = string.Join(
            ",",
            context.ActionArguments.Select(kvp => $"{kvp.Key}={kvp.Value}")
        );

        context.HttpContext.Items["ActionName"] = actionName;
        context.HttpContext.Items["Arguments"] = arguments;
        context.HttpContext.Items["StartTime"] = DateTime.Now;
    }

    /// <summary>
    /// 在 Action 方法执行**之后**、结果执行之前被调用
    /// </summary>
    /// <param name="context">Action 执行后的上下文，可能包含异常信息</param>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        string? actionName = context.HttpContext.Items["ActionName"] as string;
        DateTime startTime = (DateTime)context.HttpContext.Items["StartTime"];
        string? arguments = context.HttpContext.Items["Arguments"] as string;

        long elapsedMs = (long)(DateTime.Now - startTime).TotalMilliseconds;

        if (context.Exception != null)
        {
            _logger.LogError(
                context.Exception,
                $"Action[{actionName}]参数 [{arguments}]执行失败，耗时 {elapsedMs} ms，错误{context.Exception.Message}"
            );
        }
        else
        {
            _logger.LogInformation(
                $"Action [{actionName}] 参数 [{arguments}]执行成功，耗时 {elapsedMs} ms"
            );
        }
    }
}
