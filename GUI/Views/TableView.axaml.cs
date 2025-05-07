using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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
    private bool _isInitialized = false;
    
    // 存储课程名称与颜色的对应关系
    private static Dictionary<string, int> _courseColorIndices;

    public TableView()
    {
        InitializeComponent();
        
        // 在加载完成后执行一次性初始化
        Loaded += (_, _) => OnControlLoaded();
        
        // 监听DataContext变化
        DataContextChanged += OnDataContextChanged;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        _courseGrid = this.FindControl<Grid>("CourseGrid");
    }
    
    /// <summary>
    /// 控件加载完成时执行初始化
    /// </summary>
    private void OnControlLoaded()
    {
        if (!_isInitialized && DataContext is TableViewModel)
        {
            _isInitialized = true;
            // 控件已完全加载，可以安全地初始化
        }
    }

    /// <summary>
    /// 当DataContext发生变化时调用
    /// </summary>
    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        // 清理旧的事件订阅，防止内存泄漏
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        // 获取新的ViewModel
        _viewModel = DataContext as TableViewModel;
        
        if (_viewModel != null)
        {
            // 添加属性变更监听
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            
            // 初始化UI界面
            InitializeView();
        }
    }
    
    /// <summary>
    /// 处理ViewModel属性变更事件
    /// </summary>
    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(TableViewModel.FilteredCourses))
        {
            DisplayCourses();
        }
        else if (args.PropertyName == nameof(TableViewModel.CurrentWeek))
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"当前周变更为: {_viewModel.CurrentWeek}");
                UpdateWeekdayHeaders();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"更新周标题时出错: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// 初始化整个视图
    /// </summary>
    private void InitializeView()
    {
        try
        {
            // 初始化课表网格
            InitializeCourseGrid();
            
            // 显示课程
            DisplayCourses();
            
            // 更新星期头部显示
            UpdateWeekdayHeaders();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"初始化视图失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 初始化课表网格基础结构
    /// </summary>
    private void InitializeCourseGrid()
    {
        if (_viewModel == null || _courseGrid == null) return;
        
        try
        {
            // 清除现有行定义
            _courseGrid.RowDefinitions.Clear();

            // 添加行定义 - 课节数量
            foreach (var _ in _viewModel.TimeSlots)
            {
                _courseGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
            }

            // 添加时间节次显示列
            CreateTimeSlotColumn();

            // 添加基础网格线
            CreateGridLines();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"初始化课表网格失败: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 创建时间节次列
    /// </summary>
    private void CreateTimeSlotColumn()
    {
        if (_viewModel == null || _courseGrid == null) return;
        
        for (int i = 0; i < _viewModel.TimeSlots.Count; i++)
        {
            var timeSlot = _viewModel.TimeSlots[i];

            var border = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0, 0, 1, 1),
                MinHeight = 50
            };

            var panel = new StackPanel
            {
                Margin = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            panel.Children.Add(new TextBlock
            {
                Text = $"第{timeSlot.SlotNumber}节",
                HorizontalAlignment = HorizontalAlignment.Center
            });

            panel.Children.Add(new TextBlock
            {
                Text = timeSlot.StartTime.ToString("HH:mm"),
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 10
            });

            panel.Children.Add(new TextBlock
            {
                Text = timeSlot.EndTime.ToString("HH:mm"),
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 10
            });

            border.Child = panel;

            Grid.SetRow(border, i);
            Grid.SetColumn(border, 0);

            _courseGrid.Children.Add(border);
        }
    }
    
    /// <summary>
    /// 创建课表网格线
    /// </summary>
    private void CreateGridLines()
    {
        if (_viewModel == null || _courseGrid == null) return;
        
        for (int row = 0; row < _viewModel.TimeSlots.Count; row++)
        {
            for (int col = 1; col <= 7; col++)
            {
                var cell = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    MinHeight = 50
                };

                Grid.SetRow(cell, row);
                Grid.SetColumn(cell, col);

                _courseGrid.Children.Add(cell);
            }
        }
    }

    /// <summary>
    /// 显示课程卡片
    /// </summary>
    private void DisplayCourses()
    {
        if (_viewModel?.FilteredCourses == null || _courseGrid == null) return;
        
        try
        {
            // 清除现有课程卡片
            RemoveExistingCourseCards();

            // 添加新的课程卡片
            AddCourseCards();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"显示课程时出错: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 清除现有课程卡片
    /// </summary>
    private void RemoveExistingCourseCards()
    {
        if (_courseGrid == null) return;
        
        for (int i = _courseGrid.Children.Count - 1; i >= 0; i--)
        {
            var child = _courseGrid.Children[i];
            if (child is Border border && border.Tag as string == "CourseCard")
            {
                _courseGrid.Children.RemoveAt(i);
            }
        }
    }
    
    /// <summary>
    /// 添加课程卡片到网格
    /// </summary>
    private void AddCourseCards()
    {
        if (_viewModel?.FilteredCourses == null || _courseGrid == null) return;
        
        foreach (var course in _viewModel.FilteredCourses)
        {
            var dayIndex = GetDayColumn(course.Weekday) + 1; // +1是因为第0列是时间节次列
            if (dayIndex > 0)
            {
                // 创建课程卡片
                var courseCard = CreateCourseCard(course);

                // 设置位置
                Grid.SetColumn(courseCard, dayIndex);
                Grid.SetRow(courseCard, course.StartSlot - 1);
                Grid.SetRowSpan(courseCard, course.EndSlot - course.StartSlot + 1);

                // 设置Z轴层级，确保覆盖在网格上方
                courseCard.ZIndex = 1;

                _courseGrid.Children.Add(courseCard);
            }
        }
    }

    /// <summary>
    /// 创建课程卡片
    /// </summary>
    /// <param name="course">课程信息</param>
    /// <returns>表示课程的Border控件</returns>
    private Border CreateCourseCard(Course course)
    {
        // 为不同课程创建不同的颜色
        var courseColor = GetCourseColor(course.CourseName);
        var textColor = ShouldUseDarkText(courseColor) ? 
            new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);

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

        var panel = CreateCourseCardContent(course, textColor);
        card.Child = panel;

        // 添加双击事件 - 打开课程详情
        card.DoubleTapped += (_, _) => {
            _viewModel?.OpenCourseCardCommand.Execute(course);
        };

        // 添加鼠标悬停效果
        AddHoverEffects(card);

        return card;
    }
    
    /// <summary>
    /// 创建课程卡片内容
    /// </summary>
    private StackPanel CreateCourseCardContent(Course course, IBrush textColor)
    {
        var panel = new StackPanel();

        // 课程名称
        panel.Children.Add(new TextBlock
        {
            Text = course.CourseName,
            FontWeight = FontWeight.Bold,
            TextWrapping = TextWrapping.Wrap,
            Foreground = textColor,
            Margin = new Thickness(0, 0, 0, 2)
        });

        // 教室
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

        // 教师
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

        return panel;
    }
    
    /// <summary>
    /// 为卡片添加悬停效果
    /// </summary>
    private static void AddHoverEffects(Border card)
    {
        card.PointerEntered += (_, _) => {
            card.Opacity = 0.85;
            card.Cursor = new Cursor(StandardCursorType.Hand);
        };

        card.PointerExited += (_, _) => {
            card.Opacity = 1.0;
            card.Cursor = Cursor.Default;
        };
    }

    /// <summary>
    /// 更新星期标题，包括周几和日期
    /// </summary>
    private void UpdateWeekdayHeaders()
    {
        if (_viewModel == null) return;

        try
        {
            var semesterStart = TableLayout.First;
            var currentWeekFirstDay = semesterStart.AddDays((_viewModel.CurrentWeek - 1) * 7);

            var tableGrid = this.FindControl<Grid>("TableGrid");
            if (tableGrid == null) return;

            var headerGrid = tableGrid.Children.FirstOrDefault() as Grid;
            if (headerGrid == null) return;

            // 遍历星期列
            for (int i = 1; i <= 7; i++)
            {
                if (i >= headerGrid.Children.Count) continue;
                
                var currentDate = currentWeekFirstDay.AddDays(i - 1);
                var border = headerGrid.Children[i] as Border;
                if (border == null) continue;

                UpdateWeekdayHeader(border, currentDate);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"更新星期标题时出错: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 更新单个星期标题
    /// </summary>
    private void UpdateWeekdayHeader(Border border, DateTime date)
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

        var dayOfWeek = (int)date.DayOfWeek;

        // 创建星期文本
        var weekdayText = new TextBlock
        {
            Text = GetWeekdayText(dayOfWeek),
            HorizontalAlignment = HorizontalAlignment.Center,
            FontWeight = FontWeight.Normal
        };

        // 创建日期文本
        var dateText = new TextBlock
        {
            Text = date.ToString("MM/dd"),
            HorizontalAlignment = HorizontalAlignment.Center,
            FontSize = 11,
            Opacity = 0.85,
            Margin = new Thickness(0, 2, 0, 0)
        };

        // 尝试绑定前景色资源
        try
        {
            var resource = Resources.GetResourceObservable("PrimaryForegroundBrush");
            if (resource != null)
            {
                weekdayText.Bind(ForegroundProperty, resource);
                dateText.Bind(ForegroundProperty, resource);
            }
            else
            {
                // 资源不存在，使用默认颜色
                weekdayText.Foreground = new SolidColorBrush(Colors.Black);
                dateText.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
        catch
        {
            // 绑定失败，使用默认颜色
            weekdayText.Foreground = new SolidColorBrush(Colors.Black);
            dateText.Foreground = new SolidColorBrush(Colors.Black);
        }

        panel.Children.Add(weekdayText);
        panel.Children.Add(dateText);

        border.Child = panel;
    }

    /// <summary>
    /// 获取星期几的显示文本
    /// </summary>
    private string GetWeekdayText(int column)
    {
        return column switch
        {
            0 => "周日",
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

    /// <summary>
    /// 将星期文本转换为列索引
    /// </summary>
    private int GetDayColumn(string weekday)
    {
        return weekday switch
        {
            "周日" => 0,
            "周一" => 1,
            "周二" => 2,
            "周三" => 3,
            "周四" => 4,
            "周五" => 5,
            "周六" => 6,
            _ => 0
        };
    }

    /// <summary>
    /// 根据课程名获取唯一颜色
    /// </summary>
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

        // 初始化课程颜色索引字典
        if (_courseColorIndices == null)
        {
            _courseColorIndices = new Dictionary<string, int>();
        }

        // 为课程分配唯一颜色
        if (!_courseColorIndices.ContainsKey(courseName))
        {
            int nextIndex = _courseColorIndices.Count % colors.Length;
            _courseColorIndices[courseName] = nextIndex;
        }

        // 获取已分配的颜色
        int colorIndex = _courseColorIndices[courseName];
        return new SolidColorBrush(colors[colorIndex]);
    }

    /// <summary>
    /// 从应用资源中获取颜色，如果获取失败则使用备用颜色
    /// </summary>
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

    /// <summary>
    /// 判断背景色是否应该使用深色文本
    /// </summary>
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