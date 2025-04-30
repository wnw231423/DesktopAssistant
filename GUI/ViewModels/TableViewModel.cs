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
        
        // 加载本周课程
        _tableService.AddSampleCourses();
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
}