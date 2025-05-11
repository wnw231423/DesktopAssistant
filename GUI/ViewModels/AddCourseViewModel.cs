using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Table;
using Data.Models.Table;
using Avalonia.Controls;

namespace GUI.ViewModels;

public partial class AddCourseViewModel : ViewModelBase
{
    private readonly TableService _tableService;
    private readonly Window _parentWindow;
    private readonly string[] _weekdays = { "周一", "周二", "周三", "周四", "周五", "周六", "周日" };

    [ObservableProperty]
    private string _courseName = string.Empty;

    [ObservableProperty]
    private string _weekRange = string.Empty;

    [ObservableProperty]
    private int _selectedWeekdayIndex = 0;

    [ObservableProperty]
    private int _startSlot = 1;

    [ObservableProperty]
    private int _endSlot = 2;

    [ObservableProperty]
    private string _classroom = string.Empty;

    [ObservableProperty]
    private string _teacher = string.Empty;

    [ObservableProperty]
    private string _coursePreview = string.Empty;

    public int MaxWeek => TableLayout.TotalWeek;

    public int MaxSlot => TableLayout.TimeSlots.Count;

    public AddCourseViewModel(TableService tableService, Window parentWindow)
    {
        _tableService = tableService;
        _parentWindow = parentWindow;

        // 订阅属性更改事件以更新预览
        PropertyChanged += (sender, args) => UpdateCoursePreview();
    }

    private void UpdateCoursePreview()
    {
        try
        {
            var weekday = _weekdays[SelectedWeekdayIndex];
            var weekRange = string.IsNullOrWhiteSpace(WeekRange) ? null : WeekRange;
            
            var course = new Course(
                CourseName,
                weekRange,
                weekday,
                StartSlot,
                EndSlot,
                Classroom,
                Teacher);

            CoursePreview = course.ToString();
        }
        catch
        {
            CoursePreview = "预览无法生成，请检查输入数据";
        }
    }

    [RelayCommand]
    private void AddCourse()
    {
        // 输入验证
        if (string.IsNullOrWhiteSpace(CourseName))
        {
            // TODO: 处理错误，例如显示错误消息
            return;
        }

        if (EndSlot < StartSlot)
        {
            // TODO: 处理错误，例如显示错误消息
            return;
        }

        var weekday = _weekdays[SelectedWeekdayIndex];
        
        // 创建新课程
        var newCourse = new Course(
            CourseName,
            string.IsNullOrWhiteSpace(WeekRange) ? null : WeekRange,
            weekday,
            StartSlot,
            EndSlot,
            Classroom,
            Teacher);

        // 添加到课程服务
        _tableService.AddCourse(newCourse);

        // 关闭窗口
        _parentWindow.Close();
    }

    [RelayCommand]
    private void Cancel()
    {
        _parentWindow.Close();
    }
}