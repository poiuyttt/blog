namespace BlogApi.Services
{
    /// <summary>
    /// 文件服务实现类
    /// 负责文件的物理存储与读取
    /// </summary>
    public class FileService : IFileService
    {
        private ILogger<FileService> _logger;
        // 文件存储根目录（项目根目录下的 uploads 文件夹）
        // Directory.GetCurrentDirectory()：获取当前应用程序的根目录路径
        private string _uploadRoot;

        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
            // Path.Combine：将多个路径片段合并为一个完整路径
            _uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            // Directory.CreateDirectory：创建文件夹，如果已存在则不做任何操作
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

            // 1. 生成唯一的文件名，避免重名覆盖
            // Guid.NewGuid()：生成全局唯一标识符
            // Path.GetExtension(file.FileName)：获取原文件的扩展名（如 .jpg, .png）
            string fileExtension = Path.GetExtension(file.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

            // 2. 按日期创建子目录，避免单个文件夹文件过多
            // DateTime.Now.ToString("yyyyMMdd")：获取当前日期字符串（如 20260509）
            string dateFolder = DateTime.Now.ToString("yyyyMMdd");
            // Path.Combine 多层拼接：./uploads/20260509/
            string targetFolder = Path.Combine(_uploadRoot, dateFolder);
            Directory.CreateDirectory(targetFolder);

            // 3. 完整的文件保存路径
            string fullPath = Path.Combine(targetFolder, uniqueFileName);

            // 4. 将上传的文件流异步写入磁盘
            // FileStream：文件流，用于读写文件
            // FileMode.Create：创建新文件，如果已存在则覆盖
            using FileStream stream = new FileStream(fullPath, FileMode.Create);
            // CopyToAsync：将上传文件的内容异步复制到文件流中
            await file.CopyToAsync(stream);

            _logger.LogInformation($"文件保存成功：{fullPath}");

            // 5. 返回文件的相对访问路径
            // 例如：uploads/20260509/abc123.jpg
            string relativePath = Path.Combine("uploads", dateFolder, uniqueFileName)
                .Replace("\\", "/");  // 统一使用前斜杠，适配 URL 格式

            return relativePath;
        }

        /// <summary>
        /// 读取文件内容（用于下载）
        /// </summary>
        public async Task<byte[]> GetFileBytesAsync(string relativePath)
        {
            // 拼接完整路径
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

            if (!File.Exists(fullPath))
            {
                _logger.LogWarning($"文件不存在：{fullPath}");
                return Array.Empty<byte>();
            }

            // File.ReadAllBytesAsync：异步读取文件的全部字节
            return await File.ReadAllBytesAsync(fullPath);
        }
    }
}