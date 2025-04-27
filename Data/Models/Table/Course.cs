using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Table;

public class Course
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // 主键自增
    public int Id {get; set;}  // 主键
    public string CourseName { get; set; }
    public string? WeekRange { get; set; } // 周次范围, null时默认为所有周
    public string Weekday { get; set; }
    public int StartSlot { get; set; } // 开始节次
    public int EndSlot { get; set; } // 结束节次
    public string Classroom { get; set; }
    public string Teacher { get; set; }

    public Course(string courseName, string weekRange, string weekday, int startSlot, int endSlot, string classroom, string teacher)
    {
        CourseName = courseName;
        WeekRange = weekRange;
        Weekday = weekday;
        StartSlot = startSlot;
        EndSlot = endSlot;
        Classroom = classroom;
        Teacher = teacher;
    }

    public override string ToString()
    {
        return ($"课程名称：{CourseName}\n" 
                + $"课程时间：{Weekday}  {Table.GetCourseTime(StartSlot, EndSlot)}\n" 
                + $"上课地点：{Classroom}\n" + $"授课教师：{Teacher}\n");
    }

    public bool IsInWeek(int weekNumber)
    {
        // 判断课程是否在指定周次上课
        if (string.IsNullOrEmpty(WeekRange)) return true;
        
        var ranges = WeekRange.Split(',');
        foreach(var range in ranges)
        {
            if (range.Contains('-'))
            {
                var parts = range.Split('-');
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[0], out int start) && 
                        int.TryParse(parts[1], out int end))
                    {
                        if (weekNumber >= start && weekNumber <= end)
                            return true;
                    }
                }
            }
            else
            {
                if (int.TryParse(range, out int week) && week == weekNumber)
                    return true;
            }
        }
        return false;
    }
}