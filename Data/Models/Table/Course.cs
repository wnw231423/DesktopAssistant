using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Data.Models.Table;

public class Course
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // 主键自增
    [JsonIgnore]
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
                + $"课程时间：{Weekday}  {GetCourseTime(StartSlot, EndSlot)}\n" 
                + $"上课地点：{Classroom}\n" + $"授课教师：{Teacher}\n");
    }
    
    // 输入一对slot Number,返回对应的时间段. 用于显示课程时间
    private string GetCourseTime(int startSlot, int endSlot)
    {
        TimeOnly startTime = Data.Models.Table.TableLayout.TimeSlots[startSlot - 1].StartTime;
        TimeOnly endTime = Data.Models.Table.TableLayout.TimeSlots[endSlot - 1].EndTime;
        return $"{startTime:HH:mm}-{endTime:HH:mm}";
    }


}