using BlogApi.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

// [ApiController]：自动处理模型验证错误等
[ApiController]
// [Route]：定义控制器的基础路径，通过 [controller] 自动使用控制器名（Todo）
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    // 用于临时存储的静态列表：模拟数据库
    private static List<TodoItem> _items = new List<TodoItem>
    {
        new TodoItem { Id = 1, Title = "学习 ASP.NET Core", IsComplete = false },
        new TodoItem { Id = 2, Title = "练习 Vue3", IsComplete = true }
    };

    // 1. 获取所有待办事项 (GET api/todo)
    [HttpGet]
    public ActionResult<List<TodoDto>> GetAll()
    {
        var dtos = _items.Select(i => new TodoDto()
        {
            Id = i.Id,
            Title = i.Title,
            IsComplete = i.IsComplete
        }).ToList();
        // Ok()：返回 HTTP 200 状态码，并将参数序列化为 JSON
        return Ok(dtos);
    }

    // 2. 获取单个待办事项 (GET api/todo/5)
    // {id} 是路由参数，通过方法参数接收
    [HttpGet("{id}")]
    public ActionResult<TodoDto> GetById(int id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item == null)
            // NotFound()：返回 HTTP 404 状态码
            return NotFound(new { Message = $"ID 为 {id} 的项不存在" });
        var dto = new TodoDto()
        {
            Id = item.Id,
            Title = item.Title,
            IsComplete = item.IsComplete
        };

        return Ok(dto);
    }

    // 3. 创建新的待办事项 (POST api/todo)
    [HttpPost]
    public ActionResult<TodoDto> Create([FromBody] CreateTodoDto createDto)
    {
        if (createDto == null || string.IsNullOrWhiteSpace(createDto.Title))
            // BadRequest()：返回 HTTP 400 状态码，表示客户端请求有误
            return BadRequest(new { Message = "标题不能为空" });

        var newItem = new TodoItem()
        {
            Id = _items.Count > 0 ? _items.Max(i => i.Id) + 1 : 1,
            Title = createDto.Title,
            IsComplete = false
        };
        _items.Add(newItem);
        
        var returnDto = new TodoDto()
        {
            Id = newItem.Id,
            Title = newItem.Title,
            IsComplete = newItem.IsComplete
        };
        // CreatedAtAction()：返回 HTTP 201 状态码，并在响应头中包含新资源的 URI
        return CreatedAtAction(nameof(GetById), new { id = returnDto.Id }, returnDto);
    }

    // 4. 更新待办事项 (PUT api/todo/5)
    [HttpPut("{id}")]
    public ActionResult<TodoDto> Update(int id, [FromBody] CreateTodoDto updatedDto)
    {
        if (updatedDto == null || string.IsNullOrWhiteSpace(updatedDto.Title))
            return BadRequest(new { Message = "标题不能为空" });
        var existingItem = _items.FirstOrDefault(i => i.Id == id);
        if (existingItem == null)
            return NotFound(new { Message = $"ID 为 {id} 的项不存在" });

        // 更新属性
        existingItem.Title = updatedDto.Title;

        // NoContent()：返回 HTTP 204 状态码，表示更新成功但无内容返回
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var existingItem = _items.FirstOrDefault(i => i.Id == id);
        if (existingItem == null)
            return NotFound(new { Message = $"ID 为 {id} 的项不存在" });

        _items.Remove(existingItem);

        // 删除成功也返回 204 No Content
        return NoContent();
    }
}

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsComplete { get; set; }
}