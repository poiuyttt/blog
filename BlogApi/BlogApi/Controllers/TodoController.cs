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

    public TodoController(ITodoService service)
    {
        _service = service;
    }

    // 1. 获取所有待办事项 (GET api/todo)
    [HttpGet]
    public IActionResult GetAll() => Ok(_service.GetAll());


    // 2. 获取单个待办事项 (GET api/todo/5)
    // {id} 是路由参数，通过方法参数接收
    [HttpGet("{id}")]
    public IActionResult GetById(int id) => Ok(_service.GetById(id));

    // 3. 创建新的待办事项 (POST api/todo)
    [HttpPost]
    public IActionResult Create([FromBody] CreateTodoDto createDto)
    {
        var item = _service.Create(createDto.Title);
        // CreatedAtAction()：返回 HTTP 201 状态码，并在响应头中包含新资源的 URI
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
      
    }


    // 4. 更新待办事项 (PUT api/todo/5)
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] CreateTodoDto updatedDto)
    {
        if (!_service.Update(id, updatedDto.Title))
            return NotFound(new { Message = $"ID 为 {id} 的项不存在" });
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (!_service.Delete(id))
            return NotFound(new { Message = $"ID 为 {id} 的项不存在" });
        return NoContent();
    }
}
