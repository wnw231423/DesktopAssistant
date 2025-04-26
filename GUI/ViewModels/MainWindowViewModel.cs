using System;
using System.Windows.Input;
using Avalonia.Styling;
using GUI.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GUI.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private object _currentView;

        [ObservableProperty]
        private string _selectedPageTag;

        [ObservableProperty]
        private bool _isDarkTheme;

        public MainWindowViewModel()
        {
            // 检查当前主题设置
            var app = Avalonia.Application.Current;
            if (app?.ActualThemeVariant == ThemeVariant.Dark)
            {
                IsDarkTheme = true;
            }

            // 默认导航到课表页面
            Navigate("Table");
        }

        [RelayCommand]
        private void Navigate(string destination)
        {
            SelectedPageTag = destination;

            switch (destination)
            {
                case "Table":
                    CurrentView = new TableView();
                    break;
                case "Todo":
                    // 稍后实现
                    CurrentView = new PlaceholderView("待办事项功能即将上线");
                    break;
                case "Settings":
                    // 稍后实现
                    CurrentView = new PlaceholderView("设置页面开发中");
                    break;
                default:
                    CurrentView = new TableView();
                    break;
            }
        }
        
        [RelayCommand]
        private void ToggleTheme()
        {
            IsDarkTheme = !IsDarkTheme;

            var app = Avalonia.Application.Current;
            if (app != null)
            {
                app.RequestedThemeVariant = IsDarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;
            }
        }
    }

    // 简单的辅助视图 - 用于占位
    public class PlaceholderView : Avalonia.Controls.UserControl
    {
        public PlaceholderView(string message)
        {
            Content = new Avalonia.Controls.TextBlock
            {
                Text = message,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = 18
            };
        }
    }
}