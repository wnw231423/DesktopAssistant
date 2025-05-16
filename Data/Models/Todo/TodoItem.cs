using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Todo;

public class TodoItem
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // 主键自增
    public int Id {get; set;}  // 主键
    
    // 课程标签. 注意一个TodoItem可能是不带课程标签的普通todo, 也可能是带课程标签的todo.
    public string CourseTag {get; set;}  
    
    // 待办事项内容
        public string Content { get; set; }

        // 起始时间
        public DateTime StartTime { get; set; }

        // 终止时间（可为空，表示长期持续）
        public DateTime? EndTime { get; set; }

        // 是否为长期持续任务
        public bool IsLongTerm { get; set; }

        // 是否被删除（逻辑删除，实际删除由前端或后端控制器处理）
        public bool IsDeleted { get; set; } = false;

        // 构造函数
        public TodoItem(string content, DateTime startTime, DateTime? endTime = null, bool isLongTerm = false, string? courseTag = null)
        {
            Content = content;
            StartTime = startTime;
            EndTime = endTime;
            IsLongTerm = isLongTerm;
            CourseTag = courseTag;
        }

        public override string ToString()
        {
            string endTimeDisplay = IsLongTerm ? "长期持续" : EndTime?.ToString("yyyy-MM-dd HH:mm") ?? "未设定";
            return $"[{CourseTag ?? "无标签"}] {Content}（{StartTime:yyyy-MM-dd HH:mm} - {endTimeDisplay}）";
        }
    }

}
