using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Data.Models.Table;
using GUI.ViewModels;

namespace GUI.Views;

public partial class TableView : UserControl
{
    private Grid _courseGrid;
    private TableViewModel _viewModel;

    public TableView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        _courseGrid = this.FindControl<Grid>("CourseGrid");
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        _viewModel = DataContext as TableViewModel;
        if (_viewModel != null)
        {
            // 初始化课表网格
            InitializeCourseGrid();
            
            // 显示课程
            DisplayCourses();

            // 监听课程变化
            _viewModel.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(TableViewModel.FilteredCourses))
                {
                    DisplayCourses();
                }
            };
        }
    }

    private void InitializeCourseGrid()
    {
        // 清除现有行定义
        _courseGrid.RowDefinitions.Clear();

        // 添加行定义 - 课节数量 - 使用Star布局单位
        foreach (var _ in _viewModel.TimeSlots)
        {
            _courseGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        }

        // 添加时间节次显示列
        for (int i = 0; i < _viewModel.TimeSlots.Count; i++)
        {
            var timeSlot = _viewModel.TimeSlots[i];
            
            var border = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0, 0, 1, 1),
                MinHeight = 50 // 使用最小高度而不是固定高度
            };
            
            var panel = new StackPanel
            {
                Margin = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            var textBlock1 = new TextBlock
            {
                Text = $"第{timeSlot.SlotNumber}节",
                HorizontalAlignment = HorizontalAlignment.Center
            };
            
            var textBlock2 = new TextBlock
            {
                Text = timeSlot.StartTime.ToString("HH:mm"),
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 10
            };
            
            var textBlock3 = new TextBlock
            {
                Text = timeSlot.EndTime.ToString("HH:mm"),
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 10
            };
            
            panel.Children.Add(textBlock1);
            panel.Children.Add(textBlock2);
            panel.Children.Add(textBlock3);
            
            border.Child = panel;
            
            Grid.SetRow(border, i);
            Grid.SetColumn(border, 0);
            
            _courseGrid.Children.Add(border);
        }

        // 添加基础网格线
        for (int row = 0; row < _viewModel.TimeSlots.Count; row++)
        {
            for (int col = 1; col <= 7; col++)
            {
                var cell = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    MinHeight = 50 // 使用最小高度而非固定高度
                };
                
                Grid.SetRow(cell, row);
                Grid.SetColumn(cell, col);
                
                _courseGrid.Children.Add(cell);
            }
        }
    }

    private void DisplayCourses()
    {
        if (_viewModel?.FilteredCourses == null) return;

        // 清除现有课程卡片
        for (int i = _courseGrid.Children.Count - 1; i >= 0; i--)
        {
            var child = _courseGrid.Children[i];
            if (child is Border border && border.Tag as string == "CourseCard")
            {
                _courseGrid.Children.RemoveAt(i);
            }
        }

        // 添加课程卡片
        foreach (var course in _viewModel.FilteredCourses)
        {
            var dayIndex = GetDayColumn(course.Weekday);
            if (dayIndex > 0)
            {
                // 创建课程卡片
                var card = new Border
                {
                    Background = new SolidColorBrush(Color.Parse("#ADD8E6")), // LightBlue
                    BorderBrush = new SolidColorBrush(Color.Parse("#87CEEB")), // SkyBlue
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(3),
                    Padding = new Thickness(5),
                    Margin = new Thickness(2),
                    Tag = "CourseCard"
                };
                
                var panel = new StackPanel();
                
                panel.Children.Add(new TextBlock
                {
                    Text = course.CourseName,
                    FontWeight = FontWeight.Bold,
                    TextWrapping = TextWrapping.Wrap
                });
                
                panel.Children.Add(new TextBlock
                {
                    Text = course.Classroom,
                    FontSize = 11,
                    TextWrapping = TextWrapping.Wrap
                });
                
                panel.Children.Add(new TextBlock
                {
                    Text = course.Teacher,
                    FontSize = 11,
                    TextWrapping = TextWrapping.Wrap
                });
                
                card.Child = panel;
                
                // 设置位置
                Grid.SetColumn(card, dayIndex);
                Grid.SetRow(card, course.StartSlot - 1); 
                Grid.SetRowSpan(card, course.EndSlot - course.StartSlot + 1);
                
                // 覆盖在网格上方
                card.ZIndex = 1;
                
                _courseGrid.Children.Add(card);
            }
        }
    }

    private int GetDayColumn(string weekday)
    {
        return weekday switch
        {
            "周一" => 1,
            "周二" => 2,
            "周三" => 3,
            "周四" => 4,
            "周五" => 5,
            "周六" => 6,
            "周日" => 7,
            _ => 0
        };
    }
}