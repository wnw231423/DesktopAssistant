using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Table;
using Data.Models.Table;
using GUI.Views;

namespace GUI.ViewModels;

public partial class TableViewModel : ViewModelBase
{
    private readonly TableService _tableService;

    [ObservableProperty]
    private int _currentWeek;

    [ObservableProperty]
    private ObservableCollection<Course> _filteredCourses;

    // 时间段列表
    public List<TimeSlot> TimeSlots => TableLayout.TimeSlots;

    // 总周数
    public int TotalWeek => TableLayout.TotalWeek;

    public TableViewModel(TableService tableService)
    {
        _tableService = tableService;
        
        // 初始化当前周
        _currentWeek = _tableService.GetCurrentWeek();
        if (_currentWeek > TotalWeek)
            _currentWeek = 1;
        
        // 加入一些sample课程
        //TODO： 在开发结束后删除
        _tableService.AddSampleCourses();
        
        // 加载课程
        LoadCourses();
    }

    private void LoadCourses()
    {
        var courses = _tableService.GetCourses();
        FilteredCourses = new ObservableCollection<Course>(
            courses.Where(c => _tableService.IsInWeek(c, CurrentWeek)));
    }

    [RelayCommand]
    private void NextWeek()
    {
        if (CurrentWeek < TotalWeek)
        {
            CurrentWeek++;
            LoadCourses();
        }
    }

    [RelayCommand]
    private void PreviousWeek()
    {
        if (CurrentWeek > 1)
        {
            CurrentWeek--;
            LoadCourses();
        }
    }

    [RelayCommand]
    private void AddCourse()
    {
        // TODO
    }
    
    [RelayCommand]
    private async Task OpenCourseCard(Course course)
    {
        if (course == null) return;

        // 创建窗口并设置 CourseCardView 作为内容
        var cardView = new CourseCardView
        {
            DataContext = new CourseCardViewModel(course)
        };

        var window = new Window
        {
            Title = $"课程信息 - {course.CourseName}",
            Content = cardView,
            Width = 600,
            Height = 700,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        // 打开窗口
        var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;
        
        await window.ShowDialog(mainWindow);
        
        // 关闭窗口后重新加载课程
        LoadCourses();
    }
}