using BlogApi.Models;
using BlogApi.Models.Dtos;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private ICommentService _service;
        private ILogger<CommentController> _logger;

        public CommentController(ICommentService service, ILogger<CommentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// GET api/comment?postId=1 — 获取文章的所有评论
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetByPostId([FromQuery] int postId)
        {
            try
            {
                var comments = await _service.GetByPostIdAsync(postId);
                return Ok(ApiResponse<object>.Ok(comments, "获取评论成功"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "获取评论失败，目标文章不存在");
                return BadRequest(ApiResponse<object>.BadRequest($"文章{postId}不存在"));
            }
        }

        /// <summary>
        /// POST api/comment — 创建评论
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateCommentDto dto)
        {
            try
            {
                var comment = await _service.CreateAsync(dto.PostId, dto.Comment, dto.Author);
                return CreatedAtAction(
                    nameof(GetByPostId),
                    new { postId = dto.PostId },
                    ApiResponse<object>.Ok(comment, "评论创建成功")
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "创建评论失败，目标文章不存在");
                return BadRequest(ApiResponse<object>.BadRequest(ex.Message));
            }
        }

        /// <summary>
        /// DELETE api/comment/5 — 删除评论
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<Comment>.NotFound($"评论{id}不存在"));
            }
            return NoContent();
        }
    }
}
