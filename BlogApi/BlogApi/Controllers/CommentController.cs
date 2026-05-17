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

        public CommentController(ICommentService service)
        {
            _service = service;
        }

        /// <summary>
        /// GET api/comment?postId=1 — 获取文章的所有评论
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetByPostId([FromQuery] int postId)
        {
            var comments = await _service.GetByPostIdAsync(postId);
            return Ok(ApiResponse<object>.Ok(comments, "获取评论成功"));
        }

        /// <summary>
        /// POST api/comment — 创建评论
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateCommentDto dto)
        {
            var comment = await _service.CreateAsync(dto.PostId, dto.Comment, dto.Author);
            if (comment == null)
                return BadRequest(ApiResponse<object>.BadRequest($"文章{dto.PostId}不存在"));

            return CreatedAtAction(
                nameof(GetByPostId),
                new { postId = dto.PostId },
                ApiResponse<object>.Ok(comment, "评论创建成功")
            );
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
