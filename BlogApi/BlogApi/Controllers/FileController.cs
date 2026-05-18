using BlogApi.Models;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

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
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<object>.BadRequest("没有文件被上传"));
        }

        const int maxFileSize = 5 * 1024 * 1024;
        if (file.Length > maxFileSize)
        {
            return BadRequest(ApiResponse<object>.BadRequest("文件大小不能超过 5MB"));
        }

        string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        string extension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest(
                ApiResponse<object>.BadRequest(
                    $"不支持的文件类型：{extension}，仅支持 {string.Join(", ", allowedExtensions)}"
                )
            );
        }

        string relativePath = await _fileService.SaveFileAsync(file);

        string fileUrl = $"{Request.Scheme}://{Request.Host}/{relativePath}";

        return Ok(
            ApiResponse<object>.Ok(
                new { Url = fileUrl, RelativePath = relativePath },
                "文件上传成功"
            )
        );
    }

    /// <summary>
    /// GET api/file/download?path=xxx — 下载文件
    /// </summary>
    [HttpGet("download")]
    public async Task<IActionResult> Download([FromQuery] string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return BadRequest(ApiResponse<object>.BadRequest("文件路径不能为空"));
        }

        byte[] fileBytes = await _fileService.GetFileBytesAsync(path);
        if (fileBytes.Length == 0)
        {
            return NotFound(ApiResponse<object>.NotFound("文件不存在"));
        }

        string extension = Path.GetExtension(path).ToLower();
        string contentType = extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream",
        };

        return File(fileBytes, contentType);
    }
}
