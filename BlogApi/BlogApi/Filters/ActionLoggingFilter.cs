using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogApi.Filters
{
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
        /// <param name="context">Action 执行上下文，包含路由数据、参数等信息</param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // ActionDescriptor.DisplayName：当前 Action 的完整名称（如 "BlogApi.Controllers.PostController.Create"）
            string actionName = context.ActionDescriptor.DisplayName;

            // context.ActionArguments：一个字典，包含所有绑定后的参数
            // 将参数序列化为可读的字符串
            var arguments = string.Join(
                ",",
                context.ActionArguments.Select(kvp => $"{kvp.Key}={kvp.Value}")
            );

            // 将 Action 名称和参数存入 HttpContext.Items，供 OnActionExecuted 取出
            // HttpContext.Items：一个请求级别的键值对存储，跨过滤器和中间件共享
            context.HttpContext.Items["ActionName"] = actionName;
            context.HttpContext.Items["Arguments"] = arguments;
            // 记录开始时间（用于 OnActionExecuted 中计算耗时）
            context.HttpContext.Items["StartTime"] = DateTime.Now;
        }

        /// <summary>
        /// 在 Action 方法执行**之后**、结果执行之前被调用
        /// </summary>
        /// <param name="context">Action 执行后的上下文，可能包含异常信息</param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // 从 Items 中取出 OnActionExecuting 阶段存储的数据
            string? actionName = context.HttpContext.Items["ActionName"] as string;
            DateTime startTime = (DateTime)context.HttpContext.Items["StartTime"];
            string? arguments = context.HttpContext.Items["Arguments"] as string;

            // 计算执行耗时
            long elapsedMs = (long)(DateTime.Now - startTime).TotalMilliseconds;

            //如果 Action 执行过程中发生了异常，context.Exception 不为 null
            if (context.Exception != null)
            {
                _logger.LogError(
                    context.Exception,
                    "Action[{ActionName}]参数 [{Arguments}]执行失败，耗时 {Elapsed} ms，错误{Message}",
                    actionName,
                    arguments,
                    elapsedMs,
                    context.Exception.Message
                );
            }
            else
            {
                _logger.LogInformation(
                    "Action [{ActionName}] 参数 [{Arguments}]执行成功，耗时 {Elapsed} ms",
                    actionName,
                    arguments,
                    elapsedMs
                );
            }
        }
    }
}