using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Table;
using Data.Models.Table;

namespace GUI.ViewModels;

public partial class TableViewModel : ViewModelBase
{
    private readonly TableService _tableService;

    [ObservableProperty]
    private int _currentWeek;

    [ObservableProperty]
    private ObservableCollection<Course> _filteredCourses;

    // 星期列表
    public List<string> WeekDays { get; } = new List<string>
        { "周一", "周二", "周三", "周四", "周五", "周六", "周日" };

    // 时间段列表
    public List<TimeSlot> TimeSlots => Table.TimeSlots;

    // 总周数
    public int TotalWeek => Table.TotalWeek;

    public TableViewModel(TableService tableService)
    {
        _tableService = tableService;
        
        // 初始化当前周
        _currentWeek = Table.GetCurrentWeek();
        if (_currentWeek > TotalWeek)
            _currentWeek = 1;
        
        // 加载本周课程
        _tableService.AddSampleCourses();
        LoadCourses();
    }

    private void LoadCourses()
    {
        var courses = _tableService.GetCourses();
        FilteredCourses = new ObservableCollection<Course>(
            courses.Where(c => c.IsInWeek(CurrentWeek)));
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
}