using BlogApi.Models;

namespace BlogApi.Services;

public interface ITodoService
{
    IEnumerable<TodoItem> GetAll();
    TodoItem? GetById(int id);
    TodoItem Create(string title);
    bool Update(int id, string title);
    bool Delete(int id);
}