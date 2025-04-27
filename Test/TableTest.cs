using Core.Table;
using Data.Models;
using Data.Models.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Test
{
    public class TableTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly TableService _tableService;
        private readonly string _testCourseResourceDir;

        public TableTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _tableService = new TableService();
            // 测试前清空数据，确保测试环境干净
            _tableService.ClearCourses();
            
            // 设置测试用的资源目录
            _testCourseResourceDir = Path.Combine(Path.GetTempPath(), "test_course_resources");
            if (Directory.Exists(_testCourseResourceDir))
                Directory.Delete(_testCourseResourceDir, true);
            Directory.CreateDirectory(_testCourseResourceDir);
        }
        
        [Fact]
        public void AddCourse_ShouldAddCourseToDatabase()
        {
            // Arrange
            var course = new Course("测试课程", "1-16", "周一", 1, 2, "A101", "测试教授");

            // Act
            _tableService.AddCourse(course);
            var courses = _tableService.GetCourses();

            // Assert
            Assert.Single(courses);
            Assert.Equal("测试课程", courses[0].CourseName);
            Assert.Equal("周一", courses[0].Weekday);
        }

        [Fact]
        public void RemoveCourse_ShouldRemoveCourseFromDatabase()
        {
            // Arrange
            var course = new Course("测试课程", "1-16", "周一", 1, 2, "A101", "测试教授");
            _tableService.AddCourse(course);
            var courses = _tableService.GetCourses();
            int id = courses[0].Id;

            // Act
            _tableService.RemoveCourse(id);
            var coursesAfterRemove = _tableService.GetCourses();

            // Assert
            Assert.Empty(coursesAfterRemove);
        }

        [Fact]
        public void UpdateCourse_ShouldUpdateExistingCourse()
        {
            // Arrange
            var course = new Course("测试课程", "1-16", "周一", 1, 2, "A101", "测试教授");
            _tableService.AddCourse(course);
            var courses = _tableService.GetCourses();
            int id = courses[0].Id;

            // Act
            var updatedCourse = new Course("更新后课程", "1-8", "周二", 3, 4, "B202", "新教授")
            {
                Id = id
            };
            _tableService.UpdateCourse(updatedCourse);
            var coursesAfterUpdate = _tableService.GetCourses();

            // Assert
            Assert.Single(coursesAfterUpdate);
            Assert.Equal("更新后课程", coursesAfterUpdate[0].CourseName);
            Assert.Equal("周二", coursesAfterUpdate[0].Weekday);
            Assert.Equal("B202", coursesAfterUpdate[0].Classroom);
        }

        [Fact]
        public void GetCoursesByDate_ShouldReturnCoursesOnSpecificDate()
        {
            // Arrange
            _tableService.ClearCourses();
            
            // 设置测试数据，使用固定的日期
            Data.Models.Table.Table.First = new DateTime(2023, 9, 1); // 假设9月1日是周五
            
            // 添加两门课程，一门在周一，一门在周五
            var courseMonday = new Course("周一课程", "1-16", "周一", 1, 2, "A101", "测试教授");
            var courseFriday = new Course("周五课程", "1-16", "周五", 3, 4, "B202", "测试教授2");
            
            _tableService.AddCourse(courseMonday);
            _tableService.AddCourse(courseFriday);

            // Act
            // 测试2023年9月1日（周五）的课程
            var fridayCourses = _tableService.GetCoursesByDate(new DateTime(2023, 9, 1));
            // 测试2023年9月4日（周一）的课程
            var mondayCourses = _tableService.GetCoursesByDate(new DateTime(2023, 9, 4));

            // Assert
            Assert.Single(fridayCourses);
            Assert.Equal("周五课程", fridayCourses[0].CourseName);
            
            Assert.Single(mondayCourses);
            Assert.Equal("周一课程", mondayCourses[0].CourseName);
        }

        // 可以查看输出的字符串
        [Fact]
        public void GetCoursesString_ShouldReturnFormattedString()
        {
            // Arrange
            _tableService.ClearCourses();
            var course = new Course("测试课程", "1-16", "周一", 1, 2, "A101", "测试教授");
            _tableService.AddCourse(course);

            // Act
            var result = _tableService.GetCoursesString();
            _testOutputHelper.WriteLine(result);

            // Assert
            Assert.Contains("测试课程", result);
            Assert.Contains("周一", result);
            Assert.Contains("A101", result);
            Assert.Contains("测试教授", result);
        }

        [Fact]
        public void AddSampleCourses_ShouldAddSampleCoursesToDatabase()
        {
            // Arrange
            _tableService.ClearCourses();

            // Act
            _tableService.AddSampleCourses();
            var courses = _tableService.GetCourses();

            // Assert
            Assert.True(courses.Count >= 4);
            Assert.Contains(courses, c => c.CourseName == "高等数学");
            Assert.Contains(courses, c => c.CourseName == "线性代数");
            Assert.Contains(courses, c => c.CourseName == "程序设计");
            Assert.Contains(courses, c => c.CourseName == "数据结构");
        }
        
        [Fact]
        public void GetTodaysCourses_ShouldReturnCoursesForToday()
        {
            // Arrange
            _tableService.ClearCourses();
            
            // 设置今天为周一
            string today = DateTime.Today.DayOfWeek switch
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
            
            // 计算当前是第几周
            int daysDiff = (int)(DateTime.Today - Data.Models.Table.Table.First).TotalDays;
            int weekNumber = daysDiff < 0 ? 0 : (daysDiff / 7) + 1;
            string weekPattern = $"{weekNumber}";
            
            // 添加今天的课程和其他日的课程
            var todayCourse = new Course("今天课程", weekPattern, today, 1, 2, "A101", "测试教授");
            var otherDayCourse = new Course("其他日课程", weekPattern, today == "周一" ? "周二" : "周一", 3, 4, "B202", "测试教授2");
            
            _tableService.AddCourse(todayCourse);
            _tableService.AddCourse(otherDayCourse);

            // Act
            var todayCourses = _tableService.GetTodaysCourses();

            // Assert
            Assert.Single(todayCourses);
            Assert.Equal("今天课程", todayCourses[0].CourseName);
        }

        [Fact]
        public void ClearCourses_ShouldRemoveAllCourses()
        {
            // Arrange
            _tableService.AddSampleCourses();
            var coursesBeforeClear = _tableService.GetCourses();
            Assert.NotEmpty(coursesBeforeClear);

            // Act
            _tableService.ClearCourses();
            var coursesAfterClear = _tableService.GetCourses();

            // Assert
            Assert.Empty(coursesAfterClear);
        }

        [Fact]
        public void AddCourseResource_ShouldAddResourceToCorrectFolder()
        {
            // Arrange
            string courseName = "测试课程";
            string resourceType = "ppt";
            string sourceFilePath = Path.Combine(_testCourseResourceDir, "test.ppt");
            
            // 创建测试文件
            File.WriteAllText(sourceFilePath, "测试内容");
            
            // Act
            _tableService.AddCourseResource(courseName, resourceType, sourceFilePath);
            
            // Assert - 这里假设 AddCourseResource 会将资源复制到个人文档的 course 目录
            string targetDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "course", courseName, resourceType);
            string targetFile = Path.Combine(targetDir, "test.ppt");
            
            Assert.True(Directory.Exists(targetDir));
            Assert.True(File.Exists(targetFile));
        }
    }
}