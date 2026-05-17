using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// POST api/file/upload — 上传文件
        /// </summary>
        /// <param name="file">上传的文件（通过 FormData 发送）</param>
        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // 检查是否有文件上传
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "没有文件被上传" });
            }

            // 检查文件大小（限制 5MB）
            const int maxFileSize = 5 * 1024 * 1024;  // 5MB = 5 * 1024 * 1024 字节
            if (file.Length > maxFileSize)
            {
                return BadRequest(new { Message = "文件大小不能超过 5MB" });
            }

            // 检查文件类型
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            string extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { Message = $"不支持的文件类型：{extension}，仅支持 {string.Join(", ", allowedExtensions)}" });
            }

            // 调用服务保存文件
            string relativePath = await _fileService.SaveFileAsync(file);

            // 构建完整的文件访问 URL
            // $\"{Request.Scheme}://{Request.Host}\"：获取当前请求的协议和主机名
            // 例如：https://localhost:5001
            string fileUrl = $"{Request.Scheme}://{Request.Host}/{relativePath}";

            // CreatedAtAction：返回 201，并在响应中附带文件 URL
            return Ok(new
            {
                Url = fileUrl,
                RelativePath = relativePath
            });
        }

        /// <summary>
        /// GET api/file/download?path=xxx — 下载文件
        /// </summary>
        [HttpGet("download")]
        public async Task<IActionResult> Download([FromQuery] string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return BadRequest(new { Message = "文件路径不能为空" });
            }

            byte[] fileBytes = await _fileService.GetFileBytesAsync(path);
            if (fileBytes.Length == 0)
            {
                return NotFound(new { Message = "文件不存在" });
            }

            // 根据扩展名确定文件的 MIME 类型
            string extension = Path.GetExtension(path).ToLower();
            string contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"  // 未知类型则按二进制流下载
            };

            // File：返回文件给客户端
            return File(fileBytes, contentType);
        }
    }
}
