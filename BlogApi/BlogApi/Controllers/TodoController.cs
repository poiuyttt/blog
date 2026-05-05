using BlogApi.Models.Dtos;
using BlogApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

// [ApiController]：自动处理模型验证错误等
[ApiController]
// [Route]：定义控制器的基础路径，通过 [controller] 自动使用控制器名（Todo）
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private ITodoService _service;

    private ILogger<TodoController> _logger;

    public TodoController(ITodoService service, ILogger<TodoController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // 1. 获取所有待办事项 (GET api/todo)
    [HttpGet]
    public IActionResult GetAll()
    {
        _logger.LogInformation("正在获取所有待办事项");
        var items = _service.GetAll();
        _logger.LogInformation("获取到 {Count} 个待办事项", items.Count());
        return Ok(items);
    }

    // 2. 创建新的待办事项 (POST api/todo)
    [HttpPost]
    public IActionResult Create([FromBody] CreateTodoDto createDto)
    {
        if (createDto == null || string.IsNullOrEmpty(createDto.Title))
        {
            _logger.LogWarning("创建待办事项失败，标题为空");
            return BadRequest(new { Message = "标题不能为空" });
        }

        var item = _service.Create(createDto.Title);
        _logger.LogInformation("创建待办事项 {Id}-{Title} 成功", item.Id, item.Title);
        // CreatedAtAction()：返回 HTTP 201 状态码，并在响应头中包含新资源的 URI
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    // 3. 获取单个待办事项 (GET api/todo/5)
    // {id} 是路由参数，通过方法参数接收
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        _logger.LogInformation($"正在获取待办事项:{id}");
        var item = _service.GetById(id);
        if (item == null)
        {
            _logger.LogWarning("获取待办事项:{Id} 失败，不存在", id);
            return NotFound();
        }
        _logger.LogInformation("获取待办事项:{Id} 成功", id);
        return Ok(item);
    }

    // 4. 更新待办事项 (PUT api/todo/5)
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] CreateTodoDto updatedDto)
    {
        if (!_service.Update(id, updatedDto.Title))
        {
            _logger.LogWarning("更新待办事项:{Id} 失败，不存在", id);
            return NotFound(new { Message = $"ID 为 {id} 的项不存在" });
        }
        _logger.LogInformation("更新待办事项:{Id} 成功", id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (!_service.Delete(id))
        {
            _logger.LogWarning("删除待办事项:{Id} 失败，不存在", id);
            return NotFound(new { Message = $"ID 为 {id} 的项不存在" });
        }
        _logger.LogInformation("删除待办事项:{Id} 成功", id);
        return NoContent();
    }
}