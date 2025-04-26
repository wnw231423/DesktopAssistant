using Data.Models;
using Data.Models.Table;

namespace Core.Table;

public class TableService
{
    // 获取数据库中的所有课程
    public List<Course> GetCourses()
    {
        using var ctx = new DaContext();
        var courses = ctx.Courses.ToList();
        return courses;
    }
    
    // 以字符串的形式返回所有课程的信息, 用于AI模块
    public string GetCoursesString()
    {
        try
        {
            var courses = GetCourses();
            if (courses == null || courses.Count == 0)
            {
                return "暂无课程信息";
            }
            var dataStrings = courses.Select(course => course.ToString());
            return String.Join("\n\n", dataStrings);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"转换字符串失败：{ex.Message}");
            return "转换字符串失败";
        }
    }

    // 获取今天的课程
    public List<Course> GetTodaysCourses()
    {
        try
        {
            string today;
            DateTime now = DateTime.Now;
            string dayOfWeek = now.DayOfWeek.ToString();
            switch (dayOfWeek)
            {
                case "Monday":
                    today = "周一";
                    break;
                case "Tuesday":
                    today = "周二";
                    break;
                case "Wednesday":
                    today = "周三";
                    break;
                case "Thursday":
                    today = "周四";
                    break;
                case "Friday":
                    today = "周五";
                    break;
                case "Saturday":
                    today = "周六";
                    break;
                case "Sunday":
                    today = "周日";
                    break;
                default:
                    today = "未知";
                    break;
            }
            using var context = new DaContext();
            var courses = context.Courses.Where(c => c.Weekday == today).ToList();
            return courses;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"获取今日课程失败{ex.Message}");
            return new List<Course>();
        }
    }
    
    // 添加课程
    public void AddCourse(Course course)
    {
        try
        {
            using var context = new DaContext();
            context.Courses.Add(course);
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"添加课程失败{ex.Message}");
        }
    }
    
    // 删除课程, 根据课程ID
    public void RemoveCourse(int id)
    {
        try
        {
            using var context = new DaContext();
            var course = context.Courses.Find(id);
            if (course != null)
            {
                context.Courses.Remove(course);
                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"删除课程失败{ex.Message}");
        }
    }
    
    // 更新课程
    public void UpdateCourse(Course course)
    {
        RemoveCourse(course.Id);
        AddCourse(course);
    }
}