using Data.Models;
using Data.Models.Todo;

namespace Core.Todo;

public class TodoService
{
    // 获取数据库中的所有待办事项
    public List<TodoItem> GetTodoItems()
    {
        using var ctx = new DaContext();
        var items = ctx.TodoItems.ToList();
        return items;
    }

    // 以字符串的形式返回所有代办事项, 用于AI模块
    public string GetTodoItemString()
    {
       try
        {
            var items = GetTodoItems();
            if (items == null || items.Count == 0)
            {
                return "暂无待办事项";
            }
            var dataStrings = items.Select(item => item.ToString());
            return String.Join("\n\n", dataStrings);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"转换字符串失败：{ex.Message}");
            return "转换字符串失败";
        }
    }
    
    // 获取今天的待办事项
    public List<TodoItem> GetTodaysTodoItems()
    {
       var today = DateTime.Today;
        var items = GetTodoItems();
        return items.Where(item => item.DueDate.Date == today.Date).ToList(); 
    }
    
    // 获取某一课程的待办事项
    public List<TodoItem> GetCourseTodoItems(string courseName)
    {
        var items = GetTodoItems();
        return items.Where(item => item.CourseName == courseName).ToList();
    }
    
    // 添加待办事项
    public void AddTodoItem(TodoItem item)
    {
        try
        {
            using var context = new DaContext();
            context.TodoItems.Add(item);
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"添加待办事项失败: {ex.Message}");
        }
    }
    
    // 删除待办事项, 根据待办事项ID
    public void RemoveTodoItem(int id)
    {
        try
        {
            using var context = new DaContext();
            var item = context.TodoItems.Find(id);
            if (item != null)
            {
                context.TodoItems.Remove(item);
                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"删除待办事项失败: {ex.Message}");
        }
    }
    
    // 更新待办事项
    public void UpdateTodoItem(TodoItem item)
    {
        RemoveTodoItem(item.Id);
        AddTodoItem(item);
    }
}
