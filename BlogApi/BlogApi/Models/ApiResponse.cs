namespace BlogApi.Models;

/// <summary>
/// 统一 API 响应结果
/// 所有 Controller 的返回值都使用此格式
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// 状态码（200 = 成功，4xx/5xx = 失败）
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 提示信息
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 返回的数据（成功时有值）
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 错误列表（失败时可能有值）
    /// </summary>
    public List<string>? Errors { get; set; }

    // ==================== 静态工厂方法 ====================

    /// <summary>
    /// 成功响应
    /// </summary>
    /// <param name="data">返回的数据</param>
    /// <param name="message">提示信息，默认 "操作成功"</param>
    public static ApiResponse<T> Ok(T data, string message = "操作成功")
    {
        return new ApiResponse<T>
        {
            Code = StatusCodes.Status200OK,
            Success = true,
            Message = message,
            Data = data,
        };
    }

    /// <summary>
    /// 成功响应（无返回数据）
    /// </summary>
    public static ApiResponse<T> Ok()
    {
        return Ok(default(T)!);
    }

    /// <summary>
    /// 失败响应（通用）
    /// </summary>
    /// <param name="code">HTTP 状态码</param>
    /// <param name="message">错误消息</param>
    /// <param name="errors">错误列表</param>
    public static ApiResponse<T> Fail(int code, string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Code = code,
            Success = false,
            Message = message,
            Errors = errors,
        };
    }

    /// <summary>
    /// 400 Bad Request
    /// </summary>
    public static ApiResponse<T> BadRequest(string message, List<string>? errors = null)
    {
        return Fail(StatusCodes.Status400BadRequest, message, errors);
    }

    /// <summary>
    /// 404 Not Found
    /// </summary>
    public static ApiResponse<T> NotFound(string message, List<string>? errors = null)
    {
        return Fail(StatusCodes.Status404NotFound, message, errors);
    }

    /// <summary>
    /// 500 Internal Server Error
    /// </summary>
    public static ApiResponse<T> InternalError(string message)
    {
        return Fail(StatusCodes.Status500InternalServerError, message);
    }
}