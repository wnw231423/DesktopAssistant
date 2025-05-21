using Data.Models;
using Data.Models.Table;
using Newtonsoft.Json;

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
    
    // 获取指定日期的课程
    public List<Course> GetCoursesByDate(DateTime date)
    {
        // 计算是第几周
        int daysDiff = (int)(date - TableLayout.First).TotalDays;
        int weekNumber = daysDiff < 0 ? 0 : (daysDiff / 7) + 1;
        
        // 获取星期几
        string weekday = date.DayOfWeek switch
        {
            DayOfWeek.Monday => "周一",
            DayOfWeek.Tuesday => "周二",
            DayOfWeek.Wednesday => "周三",
            DayOfWeek.Thursday => "周四",
            DayOfWeek.Friday => "周五",
            DayOfWeek.Saturday => "周六",
            DayOfWeek.Sunday => "周日",
            _ => ""
        };
        
        // 筛选课程
        var allCourses = GetCourses();
        return allCourses.Where(c => c.Weekday == weekday && IsInWeek(c, weekNumber)).ToList();
    }

    // 获取今天的课程
    public List<Course> GetTodaysCourses()
    {
        return GetCoursesByDate(DateTime.Today);
    }
    
    // 添加课程
    public void AddCourse(Course course)
    {
        try
        {
            using var context = new DaContext();
            context.Courses.Add(course);
            context.SaveChanges();
            // 添加课程资源文件夹
            AddCourseResourceDir(course.CourseName);
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
                // 删除课程资源文件夹
                var courseResourceDir = Path.Combine(_courseResourseDir, course.CourseName);
                if (Directory.Exists(courseResourceDir))
                {
                    Directory.Delete(courseResourceDir, true);
                }
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
        var oldName = GetCourseById(course.Id).CourseName;
        var newName = course.CourseName;
        if (oldName != newName)
        {
            // 更新课程资源文件夹名字
            var oldCourseResourceDir = Path.Combine(_courseResourseDir, oldName);
            var newCourseResourceDir = Path.Combine(_courseResourseDir, newName);
            if (Directory.Exists(oldCourseResourceDir))
            {
                Directory.Move(oldCourseResourceDir, newCourseResourceDir);
            }
        }

        // 更新数据库
        RemoveCourse(course.Id);
        AddCourse(course);
    }
    
    // 根据ID获取课程
    public Course GetCourseById(int id)
    {
        using var context = new DaContext();
        return context.Courses.Find(id);
    }
    
    // 清空course
    public void ClearCourses()
    {
        using var context = new DaContext();
        context.Courses.RemoveRange(context.Courses);
        context.SaveChanges();
        
        // 删除课程资源文件夹下的所有课程资源
        if (Directory.Exists(_courseResourseDir))
        {
            Directory.Delete(_courseResourseDir, true);
        }
        
        // 重新创建课程资源文件夹
        Directory.CreateDirectory(_courseResourseDir);
        
    }
    
    /****************/
    /*** AI using ***/
    /****************/
    
    // 以字符串的形式返回所有课程的信息, 用于AI模块
    public string GetCoursesString()
    {
        try
        {
            var courses = GetTodaysCourses();
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
    
    /*****************/
    /*** GUI using ***/
    /*****************/
    
    // 用于GUI添加测试数据
    public void AddSampleCourses()
    {
        ClearCourses();
        var courses = new List<Course>
        {
            new Course("高等数学", "1-2", "周一", 1, 2, "A202", "张教授"),
            new Course("线性代数", "3-5", "周二", 3, 5, "B305", "李教授"),
            new Course("程序设计", "6-8", "周三", 6, 8, "计算机楼C304", "王教授"),
            new Course("数据结构", "11-13", "周四", 1, 2, "A402", "赵教授")
        };

        foreach (var course in courses)
        {
            AddCourse(course);
        }
    }
    
    // 获取当前是第几周
    public int GetCurrentWeek()
    {
        var daysDiff = (int) (DateTime.Today - TableLayout.First).TotalDays;
        return daysDiff < 0 ? 0 : (daysDiff / 7) + 1;
    }
    
    public bool IsInWeek(Course course, int weekNumber)
    {
        // 判断课程是否在指定周次上课
        if (string.IsNullOrEmpty(course.WeekRange)) return true;
        
        var ranges = course.WeekRange.Split(',');
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
    
    /*********************/
    /*** 课程资源管理部分 ***/
    /*********************/
    
    // 课程资源存储的总文件夹
    private static string _courseResourseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "course");
    
    // 创建课程资源文件夹, 每当用户添加一个课程, 则会添加一个课程资源文件夹
    private void AddCourseResourceDir(string courseName)
    {
        var courseResourceDir = Path.Combine(_courseResourseDir, courseName);
        
        if (!Directory.Exists(courseResourceDir))
        {
            Directory.CreateDirectory(courseResourceDir);
        }
    }
    
    // 用户在gui会先创建类别, 然后再往类别里添加文件
    public void AddResourceType(string courseName, string resourceType)
    {
        var courseResourceDir = Path.Combine(_courseResourseDir, courseName);
        
        var resourceTypeDir = Path.Combine(courseResourceDir, resourceType);
        if (!Directory.Exists(resourceTypeDir))
        {
            Directory.CreateDirectory(resourceTypeDir);
        }
        else
        {
            throw new Exception($"资源类型 {resourceType} 已存在");
        }
    }

    // 向课程资源文件夹中添加课程资源
    // 例如, 课程名称为"高等数学", 资源类型为"ppt", 文件路径为"C:\Users\user\Desktop\高数课件.zip"
    // 则将文件添加到 _courseResourceDir\高等数学\ppt\高数课件.zip
    public void AddCourseResource(string courseName, string resourceType, string filePath)
    {
        var courseResourceDir = Path.Combine(_courseResourseDir, courseName);
        var resourceTypeDir = Path.Combine(courseResourceDir, resourceType);
        if (!Directory.Exists(resourceTypeDir))
        {
            Directory.CreateDirectory(resourceTypeDir);
        }
        string fileName = Path.GetFileName(filePath);
        string targetFilePath = Path.Combine(resourceTypeDir, fileName);
        if (File.Exists(targetFilePath))
        {
            throw new Exception($"文件{fileName}已存在");
        }
        File.Copy(filePath, targetFilePath);
    }
    
    // 获取课程资源类型
    public List<string> GetCourseResourceTypes(string courseName)
    {
        var courseResourceDir = Path.Combine(_courseResourseDir, courseName);
        if (!Directory.Exists(courseResourceDir))
        {
            return new List<string>();
        }
        var resourceTypes = Directory.GetDirectories(courseResourceDir)
            .Select(Path.GetFileName)
            .ToList();
        return resourceTypes;
    }

    // 读取某个课程, 某个类型下的所有课程资源
    public List<ResourceInfo> GetCourseResources(string courseName, string resourceType)
    {
        var resources = new List<ResourceInfo>();
        var courseResourceDir = Path.Combine(_courseResourseDir, courseName);
        if (!Directory.Exists(courseResourceDir))
        {
            return resources;
        }
        
        var resourceTypeDir = Path.Combine(courseResourceDir, resourceType);
        foreach (var file in Directory.GetFiles(resourceTypeDir))
        {
            string fileName = Path.GetFileName(file);
            FileInfo fileInfo = new FileInfo(file);
            resources.Add(new ResourceInfo
            {
                ResourceName = fileName,
                ResourcePath = file,
                ResourceType = resourceType,
                LastModified = fileInfo.LastWriteTime
            });
        }
        
        return resources;
    }

    // 删除课程资源
    // 例如, 课程名称为"高等数学", 资源类型为"ppt", 文件名为"高数课件.zip"
    // 则删除 _courseResourceDir\高等数学\ppt\高数课件.zip
    public void DeleteCourseResource(string courseName, string resourceType, string fileName)
    {
        var courseResourceDir = Path.Combine(_courseResourseDir, courseName);
        var resourceTypeDir = Path.Combine(courseResourceDir, resourceType);
        string targetFilePath = Path.Combine(resourceTypeDir, fileName);
        if (File.Exists(targetFilePath))
        {
            File.Delete(targetFilePath);
        }
    }
    
    public void DeleteCourseResourceType(string courseName, string resourceType)
    {
        var courseResourceDir = Path.Combine(_courseResourseDir, courseName);
        var resourceTypeDir = Path.Combine(courseResourceDir, resourceType);
        if (Directory.Exists(resourceTypeDir))
        {
            Directory.Delete(resourceTypeDir, true);
        }
    }

    // 在文件资源管理器打开课程资源的文件夹
    public void OpenCourseResourceDir(string courseName)
    {
        var courseResourceDir = Path.Combine(_courseResourseDir, courseName);
        if (Directory.Exists(courseResourceDir))
        {
            System.Diagnostics.Process.Start("explorer.exe", courseResourceDir);
        }
        else
        {
            throw new Exception("文件不存在");
        }
    }
    
    // 直接打开某资源
    public void OpenCourseResource(string courseName, string resourceType, string fileName)
    {
        var courseResourceDir = Path.Combine(_courseResourseDir, courseName);
        var resourceTypeDir = Path.Combine(courseResourceDir, resourceType);
        string targetFilePath = Path.Combine(resourceTypeDir, fileName);
        if (File.Exists(targetFilePath))
        {
            System.Diagnostics.Process.Start(targetFilePath);
        }
        else
        {
            throw new Exception("文件不存在");
        }
    }
    
    public string GetResourcesBasePath()
    {
        return _courseResourseDir;
    }

    // 将课程信息导出为一个json文件
    public void ExportCoursesToJson(string filePath)
    {
        List<Course> courses = GetCourses();
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };
        string json = JsonConvert.SerializeObject(courses, settings);
        File.WriteAllText(filePath, json);
    }

    // 将课程信息导入
    public void ImportCoursesFromJson(string filePath)
    {
        try
        {
            string jsonContent = File.ReadAllText(filePath);
            List<Course> courses = JsonConvert.DeserializeObject<List<Course>>(jsonContent);
            foreach (var course in courses)
            {
                AddCourse(course);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"导入课程失败{ex.Message}");
        }
    }
}