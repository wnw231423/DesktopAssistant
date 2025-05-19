using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Table;
using Data.Models.Table;
using GUI.Views;
using MsBox.Avalonia;

namespace GUI.ViewModels;

public partial class CourseCardViewModel : ObservableObject
{
    [ObservableProperty]
    private Course _course;

    [ObservableProperty]
    private bool _isEditing;

    private Course _originalCourse; // 用于存储原始值以便取消编辑
    
    public ResourcesViewModel ResourcesViewModel { get; }

    public string EditButtonText => IsEditing ? "完成" : "编辑";

    public ObservableCollection<string> WeekdayOptions { get; }
    public int MaxSlot { get; } = TableLayout.TimeSlots.Count;
    
    private readonly TableService _tableService = new TableService();
    
    public event EventHandler CloseRequest;

    public CourseCardViewModel(Course course)
    {
        _course = course ?? throw new ArgumentNullException(nameof(course));
        _originalCourse = CloneCourse(course);
        ResourcesViewModel = new ResourcesViewModel(new TableService(), course);

        // 初始化星期选项
        WeekdayOptions = new ObservableCollection<string>
        {
            "周一", "周二", "周三", "周四", "周五", "周六", "周日"
        };
    }

    [RelayCommand]
    private void ToggleEditMode()
    {
        if (IsEditing)
        {
            // 如果当前是编辑模式，切换到非编辑模式
            IsEditing = false;
        }
        else
        {
            // 如果当前是非编辑模式，切换到编辑模式，先保存原始值
            _originalCourse = CloneCourse(Course);
            IsEditing = true;
        }
    }

    [RelayCommand]
    private void SaveChanges()
    {
        _tableService.UpdateCourse(Course);
        IsEditing = false;
        // 为了触发UI更新，需要重新赋值Course
        var updatedCourse = CloneCourse(Course);
        Course = updatedCourse;
        
        // 通知ResourcesViewModel课程可能已更改
        ResourcesViewModel.Course = Course;
    }

    [RelayCommand]
    private void CancelEditing()
    {
        // 恢复原始值
        Course = CloneCourse(_originalCourse);
        IsEditing = false;
    }
    
    [RelayCommand]
    private async Task DeleteCourse()
    {
        var dialog = new EnsureDialogView(new EnsureDialogViewModel(
                "删除确认", 
                $"确定要删除课程{Course.CourseName}吗？此操作不可撤销。",
            "删除",
            "取消"
        ));
    
        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        var result = await dialog.ShowDialog<bool>(mainWindow);
    
        if (result)
        {
            // 用户确认删除
            _tableService.RemoveCourse(Course.Id);
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
    
    private Course CloneCourse(Course source)
    {
        // 创建课程对象的深拷贝
        return new Course(
            source.CourseName,
            source.WeekRange,
            source.Weekday,
            source.StartSlot,
            source.EndSlot,
            source.Classroom,
            source.Teacher
        )
        {
            Id = source.Id
        };
    }
}