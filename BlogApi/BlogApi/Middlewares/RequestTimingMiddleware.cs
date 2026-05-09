namespace BlogApi.Middlewares;

/// <summary>
/// 自定义中间件：记录每个 HTTP 请求的执行时间
/// 中间件必须包含一个 InvokeAsync 方法
/// </summary>
public class RequestTimingMiddleware
{
    // RequestDelegate：下一个中间件的引用
    // 作用：调用 _next(context) 就会把请求传递给管道中的下一个中间件
    private RequestDelegate _next;

    // 构造函数注入 RequestDelegate
    // 框架会自动把管道中的下一个中间件传给此参数
    public RequestTimingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// 每个 HTTP 请求都会调用此方法
    /// </summary>
    /// <param name="context">HTTP 上下文，包含请求和响应的所有信息</param>
    public async Task InvokeAsync(HttpContext context)
    {
        // Stopwatch：高精度计时器
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // 在响应头中添加自定义信息（响应前可以修改）
        context.Response.Headers["X-Request-Timing"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // 调用下一个中间件
        // 如果这是管道中最后一个中间件，则直接处理请求并返回
        await _next(context);

        // 当响应从后续中间件返回后，代码继续执行到这里
        stopwatch.Stop();

        // 获取执行耗时（单位：毫秒）
        long elapsedMs = stopwatch.ElapsedMilliseconds;


        // ✅ 在修改响应头前，检查响应是否已经开始发送
        if (!context.Response.HasStarted)
        {
            context.Response.Headers["X-Response-Time-Ms"] = elapsedMs.ToString();
        }
        // 记录日志
        Console.WriteLine($"[{context.Request.Method}] {context.Request.Path} 执行耗时：{elapsedMs}ms");
    }
}