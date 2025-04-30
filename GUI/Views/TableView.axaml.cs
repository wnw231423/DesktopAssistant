using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
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
            
            // 更新星期列的日期, 用于按了上一周或下一周的按钮后
            UpdateWeekdayHeaders();

            // 监听课程变化
            _viewModel.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(TableViewModel.FilteredCourses))
                {
                    DisplayCourses();
                }
                if (args.PropertyName == nameof(TableViewModel.CurrentWeek))
                {
                    System.Diagnostics.Debug.WriteLine("CurrentWeek changed to: " + _viewModel.CurrentWeek);
                    UpdateWeekdayHeaders();
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
                // 为不同课程创建不同的颜色
                var courseColor = GetCourseColor(course.CourseName);
                var textColor = ShouldUseDarkText(courseColor) ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);

                // 创建课程卡片
                var card = new Border
                {
                    Background = courseColor,
                    BorderThickness = new Thickness(0),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(6),
                    Margin = new Thickness(3),
                    Tag = "CourseCard",
                    BoxShadow = new BoxShadows(new BoxShadow
                    {
                        OffsetX = 1,
                        OffsetY = 1,
                        Blur = 3,
                        Spread = 0,
                        Color = Color.FromArgb(40, 0, 0, 0)
                    })
                };

                var panel = new StackPanel();

                panel.Children.Add(new TextBlock
                {
                    Text = course.CourseName,
                    FontWeight = FontWeight.Bold,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = textColor,
                    Margin = new Thickness(0, 0, 0, 2)
                });

                if (!string.IsNullOrEmpty(course.Classroom))
                {
                    panel.Children.Add(new TextBlock
                    {
                        Text = course.Classroom,
                        FontSize = 11,
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = textColor,
                        Opacity = 0.9
                    });
                }

                if (!string.IsNullOrEmpty(course.Teacher))
                {
                    panel.Children.Add(new TextBlock
                    {
                        Text = course.Teacher,
                        FontSize = 11,
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = textColor,
                        Opacity = 0.9
                    });
                }

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
    
    private void UpdateWeekdayHeaders()
    {
        if (_viewModel == null) return;

        var semesterStart = TableLayout.First;
        var currentWeekFirstDay = semesterStart.AddDays((_viewModel.CurrentWeek - 1) * 7);

        var tableGrid = this.FindControl<Grid>("TableGrid");
        if (tableGrid == null) return;

        var headerGrid = tableGrid.Children.FirstOrDefault() as Grid;
        if (headerGrid == null) return;
        
        // // 从应用资源中获取前景色
        // IBrush textBrush;
        // if (this.TryFindResource("PrimaryForegroundBrush", this.ActualThemeVariant, out var res))
        // {
        //     textBrush = (SolidColorBrush) res;
        // }
        // else
        // {
        //     // 如果找不到资源，使用默认值
        //     bool isDarkTheme = Application.Current?.ActualThemeVariant == ThemeVariant.Dark;
        //     textBrush = new SolidColorBrush(isDarkTheme ? Colors.White : Colors.Black);
        // }

        // 遍历星期列
        for (int i = 1; i <= 7; i++)
        {
            var currentDate = currentWeekFirstDay.AddDays(i - 1);

            if (i < headerGrid.Children.Count)
            {
                var border = headerGrid.Children[i] as Border;
                if (border != null)
                {
                    // 设置通用表头样式
                    border.Background = new SolidColorBrush(Colors.Transparent);
                    border.BorderThickness = new Thickness(0, 0, 1, 1);

                    var panel = new StackPanel
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 4, 0, 4)
                    };

                    var dayOfWeek = (int)currentDate.DayOfWeek;
                    if (dayOfWeek == 0) dayOfWeek = 7;

                    // 使用你定义的前景色资源
                    var weekdayText = new TextBlock
                    {
                        Text = GetWeekdayText(dayOfWeek),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontWeight = FontWeight.Normal,
                    };
                    weekdayText.Bind(ForegroundProperty, Resources.GetResourceObservable("PrimaryForegroundBrush"));

                    var dateText = new TextBlock
                    {
                        Text = currentDate.ToString("MM/dd"),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontSize = 11,
                        Opacity = 0.85,
                        Margin = new Thickness(0, 2, 0, 0)
                    };
                    dateText.Bind(ForegroundProperty, Resources.GetResourceObservable("PrimaryForegroundBrush"));

                    panel.Children.Add(weekdayText);
                    panel.Children.Add(dateText);

                    border.Child = panel;
                }
            }
        }
    }
    
    private string GetWeekdayText(int column)
    {
        return column switch
        {
            1 => "周一",
            2 => "周二",
            3 => "周三",
            4 => "周四",
            5 => "周五",
            6 => "周六",
            7 => "周日",
            _ => ""
        };
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
    
    // 根据课程名生成唯一颜色
    private SolidColorBrush GetCourseColor(string courseName)
    {
        var colors = new[]
        {
            GetResourceColor("LuojiaGreenColor", "#115740"),  // 珞珈绿
            GetResourceColor("DonghuBlueColor", "#41B6E6"),   // 东湖蓝
            GetResourceColor("QiuguiYellowColor", "#FFA300"), // 秋桂黄
            GetResourceColor("LuoyingPinkColor", "#F8A3BC"),  // 珞樱粉
            GetResourceColor("ChuntenPurpleColor", "#33058D"),// 春藤紫
            GetResourceColor("HongwaGreenColor", "#00797C"),  // 黉瓦绿
        };

        // 使用静态字典记录每个课程名对应的颜色索引
        // 如果此为空，则为类的静态成员（需添加到类定义中）
        if (_courseColorIndices == null)
        {
            _courseColorIndices = new System.Collections.Generic.Dictionary<string, int>();
        }

        // 查看该课程是否已分配颜色
        if (!_courseColorIndices.ContainsKey(courseName))
        {
            // 如果没有分配，则分配下一个可用颜色索引
            int nextIndex = _courseColorIndices.Count % colors.Length;
            _courseColorIndices[courseName] = nextIndex;
        }

        // 使用已分配的颜色索引
        int colorIndex = _courseColorIndices[courseName];
        return new SolidColorBrush(colors[colorIndex]);
    }

    // 添加到类定义，用于记录课程颜色分配
    private static System.Collections.Generic.Dictionary<string, int> _courseColorIndices;

    // 辅助方法：获取资源颜色
    private Color GetResourceColor(string resourceKey, string fallback)
    {
        if (Application.Current?.Resources.TryGetResource(resourceKey, null, out var resource) == true)
        {
            if (resource is Color color)
            {
                return color;
            }
            else if (resource is SolidColorBrush brush)
            {
                return brush.Color;
            }
        }

        return Color.Parse(fallback);
    }

    // 辅助方法：判断背景色是否应该使用深色文本
    private bool ShouldUseDarkText(IBrush background)
    {
        if (background is SolidColorBrush brush)
        {
            var color = brush.Color;
            // 使用YIQ公式计算亮度 - 超过128使用深色文本
            double yiq = ((color.R * 299) + (color.G * 587) + (color.B * 114)) / 1000;
            return yiq >= 128;
        }
        return true;
    }
}