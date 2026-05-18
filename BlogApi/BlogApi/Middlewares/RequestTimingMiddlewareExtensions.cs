namespace BlogApi.Middlewares;

/// <summary>
/// 为 IApplicationBuilder 添加自定义扩展方法
/// 使其可以像内置中间件一样调用 app.UseRequestTiming()
/// </summary>
public static class RequestTimingMiddlewareExtensions
{
    /// <summary>
    /// 注册请求计时中间件
    /// </summary>
    /// <param name="app">应用程序构建器实例</param>
    /// <returns>原实例（支持链式调用）</returns>
    public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder app) =>
        app.UseMiddleware<RequestTimingMiddleware>();
}