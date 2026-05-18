namespace BlogApi.Services;

/// <summary>
/// 文件服务实现类
/// 负责文件的物理存储与读取
/// </summary>
public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;
    private readonly string _uploadRoot;

    public FileService(ILogger<FileService> logger)
    {
        _logger = logger;
        _uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        if (!Directory.Exists(_uploadRoot))
        {
            Directory.CreateDirectory(_uploadRoot);
        }
    }

    /// <summary>
    /// 保存上传的文件
    /// </summary>
    public async Task<string> SaveFileAsync(IFormFile file)
    {
        _logger.LogInformation($"开始上传文件：{file.FileName}，大小：{file.Length} 字节");

        string fileExtension = Path.GetExtension(file.FileName);
        string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

        string dateFolder = DateTime.Now.ToString("yyyyMMdd");
        string targetFolder = Path.Combine(_uploadRoot, dateFolder);
        Directory.CreateDirectory(targetFolder);

        string fullPath = Path.Combine(targetFolder, uniqueFileName);

        using FileStream stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        _logger.LogInformation($"文件保存成功：{fullPath}");

        string relativePath = Path.Combine("uploads", dateFolder, uniqueFileName)
            .Replace("\\", "/");

        return relativePath;
    }

    /// <summary>
    /// 读取文件内容（用于下载）
    /// </summary>
    public async Task<byte[]> GetFileBytesAsync(string relativePath)
    {
        string fullPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), relativePath));

        if (!fullPath.StartsWith(_uploadRoot, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning($"非法文件访问尝试：{relativePath}");
            return Array.Empty<byte>();
        }

        if (!File.Exists(fullPath))
        {
            _logger.LogWarning($"文件不存在：{fullPath}");
            return Array.Empty<byte>();
        }

        return await File.ReadAllBytesAsync(fullPath);
    }
}
