using BlogApi.Models;

namespace BlogApi.Services;

public class TodoService : ITodoService
{
    private static List<TodoItem> _items = new();

    public TodoService()
    {
        if (_items.Count == 0)
        {
            _items.Add(new TodoItem { Id = 1, Title = "学习 ASP.NET Core", IsComplete = false });
            _items.Add(new TodoItem { Id = 2, Title = "练习 Vue3", IsComplete = true });
        }
    }

    public IEnumerable<TodoItem> GetAll() => _items;

    public TodoItem? GetById(int id) => _items.FirstOrDefault(i => i.Id == id);

    public TodoItem Create(string title)
    {
        var newItem = new TodoItem()
        {
            Id = _items.Count > 0 ? _items.Max(i => i.Id) + 1 : 1,
            Title = title,
            IsComplete = false
        };
        _items.Add(newItem);
        return newItem;
    }

    public bool Update(int id, string title)
    {
        var existingItem = _items.FirstOrDefault(i => i.Id == id);
        if (existingItem == null)
            return false;
        existingItem.Title = title;
        return true;
    }

    public bool Delete(int id)
    {
        var existingItem = _items.FirstOrDefault(i => i.Id == id);
        if (existingItem == null)
            return false;
        _items.Remove(existingItem);
        return true;
    }
}