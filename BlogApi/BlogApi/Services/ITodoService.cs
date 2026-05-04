using BlogApi.Controllers;

namespace BlogApi.Services;

public interface IToService
{
    IEnumerable<TodoItem> GetAll();
    TodoItem? GetById(int id);
    TodoItem Create(string title);
    bool Update(int id, string title);
    bool Delete(int id);
}