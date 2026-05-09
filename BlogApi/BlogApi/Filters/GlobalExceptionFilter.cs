using BlogApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogApi.Middlewares;

/// <summary>
/// 全局异常过滤器
/// 作用：统一捕获所有 Controller 中抛出的未处理异常
/// 实现 IExceptionFilter 接口
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private ILogger<GlobalExceptionFilter> _logger;

    /// <summary>
    /// 构造函数注入日志器
    /// </summary>
    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 当 Controller 中发生未处理的异常时，此方法会被自动调用
    /// </summary>
    public void OnException(ExceptionContext context)
    {
        // 记录异常详情（包括堆栈跟踪）
        // LogError：记录 Error 级别日志，第二个参数是异常对象
        _logger.LogError(context.Exception, $"捕获到未处理异常：{context.Exception.Message}");

        // 构建统一错误响应
        var errorResponse = ApiResponse<object>.InternalError("服务器内部错误，请稍后重试");

        // 设置 HTTP 状态码
        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        // 设置响应类型为 JSON
        context.HttpContext.Response.ContentType = "application/json";
        // context.Result：设置最终返回给客户端的结果
        // ObjectResult 继承自 ObjectResult，自动序列化为 JSON
        context.Result = new ObjectResult(errorResponse)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        // 标记异常已处理，阻止异常继续向上抛出
        context.ExceptionHandled = true;
    }
}