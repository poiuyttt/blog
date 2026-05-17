namespace BlogApi.Middlewares;

/// <summary>
/// 自定义中间件：记录每个 HTTP 请求的执行时间
/// 中间件必须包含一个 InvokeAsync 方法
/// </summary>
public class RequestTimingMiddleware
{
    // RequestDelegate：下一个中间件的引用
    private RequestDelegate _next;

    public RequestTimingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Stopwatch：高精度计时器
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // 在响应头中添加自定义信息（响应前可以修改）
        context.Response.Headers["X-Request-Timing"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        await _next(context);

        stopwatch.Stop();

        long elapsedMs = stopwatch.ElapsedMilliseconds;

        if (!context.Response.HasStarted)
        {
            context.Response.Headers["X-Response-Time-Ms"] = elapsedMs.ToString();
        }
        Console.WriteLine($"[{context.Request.Method}] {context.Request.Path} 执行耗时：{elapsedMs}ms");
    }
}