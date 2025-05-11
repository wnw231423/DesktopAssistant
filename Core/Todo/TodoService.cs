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
        //TODO
        throw new NotImplementedException();
    }
    
    // 获取今天的待办事项
    public List<TodoItem> GetTodaysTodoItems()
    {
        //TODO
        throw new NotImplementedException();
    }
    
    // 获取某一课程的待办事项
    public List<TodoItem> GetCourseTodoItems(string courseName)
    {
        //TODO
        throw new NotImplementedException();
    }
    
    // 添加待办事项
    public void AddTodoItem(TodoItem item)
    {
        //TODO
        throw new NotImplementedException();
    }
    
    // 删除待办事项, 根据待办事项ID
    public void RemoveTodoItem(int id)
    {
        //TODO
        throw new NotImplementedException();
    }
    
    // 更新待办事项
    public void UpdateTodoItem(TodoItem item)
    {
        RemoveTodoItem(item.Id);
        AddTodoItem(item);
    }
}