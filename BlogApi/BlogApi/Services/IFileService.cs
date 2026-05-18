namespace BlogApi.Services;

/// <summary>
/// 文件服务接口
/// </summary>
public interface IFileService
{
    /// <summary>
    /// 保存上传的文件到本地
    /// </summary>
    /// <param name="file">上传的文件对象</param>
    /// <returns>文件访问的相对路径</returns>
    Task<string> SaveFileAsync(IFormFile file);

    /// <summary>
    /// 根据文件路径读取文件内容（用于下载）
    /// </summary>
    /// <param name="relativePath">文件相对路径</param>
    /// <returns>文件字节数组，文件不存在则返回 null</returns>
    Task<byte[]> GetFileBytesAsync(string relativePath);
}
