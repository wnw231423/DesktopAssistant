using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Table;

public class Course
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // 主键自增
    public int Id {get; set;}  // 主键
    public string CourseName { get; set; }
    public string Weekday { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Classroom { get; set; }
    public string Teacher { get; set; }

    public Course(string courseName, string weekday, DateTime startTime, DateTime endTime, string classroom, string teacher)
    {
        CourseName = courseName;
        Weekday = weekday;
        StartTime = startTime;
        EndTime = endTime;
        Classroom = classroom;
        Teacher = teacher;
    }

    //TODO: Add properties for Course class.
}