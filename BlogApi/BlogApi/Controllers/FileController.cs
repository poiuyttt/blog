using BlogApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private IFileService _fileService;
        private ILogger<FileController> _logger;

        public FileController(IFileService fileService, ILogger<FileController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        /// <summary>
        /// POST api/file/upload — 上传文件
        /// </summary>
        /// <param name="file">上传的文件（通过 FormData 发送）</param>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            // 检查是否有文件上传
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "没有文件上传" });
            }

            // 检查文件大小（限制 5MB）
            const int maxFileSize = 5 * 1024 * 1024;
            if


        }
    }
}
