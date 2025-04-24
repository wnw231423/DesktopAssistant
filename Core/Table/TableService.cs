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
        //TODO
        throw new NotImplementedException();
    }

    // 获取今天的课程
    public List<Course> GetTodaysCourses()
    {
        //TODO
        throw new NotImplementedException();
    }
    
    // 添加课程
    public void AddCourse(Course course)
    {
        //TODO
        throw new NotImplementedException();
    }
    
    // 删除课程, 根据课程ID
    public void RemoveCourse(int id)
    {
        //TODO
        throw new NotImplementedException();
    }
    
    // 更新课程
    public void UpdateCourse(Course course)
    {
        RemoveCourse(course.Id);
        AddCourse(course);
    }
}