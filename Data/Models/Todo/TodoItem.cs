using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Todo;

public class TodoItem
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // 主键自增
    public int Id {get; set;}  // 主键
    
    // 课程标签. 注意一个TodoItem可能是不带课程标签的普通todo, 也可能是带课程标签的todo.
    public string CourseTag {get; set;}  
    
    //TODO: Add properties for TodoItem class.
}