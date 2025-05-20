using System;
using System.Windows.Input;
using Avalonia.Styling;
using GUI.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.AI;
using Core.Table;
using Core.Todo;
using Data.Models.Table;

namespace GUI.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        // Should change to ViewModelBase when no placeholder
        private Object _currentView;

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
            
            //TODO: 应用初始化, 应当放在别处, 目前暂时放在这里
            InitalizeCommands();
        }
        
        private void InitalizeCommands()
        {
            AiService.AiInit();
        }

        [RelayCommand]
        private void Navigate(string destination)
        {
            SelectedPageTag = destination;

            switch (destination)
            {
                case "Table":
                    CurrentView = new TableViewModel(new TableService());
                    break;
                case "Resources":
                    CurrentView = new ResourcesMainViewModel(new TableService());
                    break;
                case "Todo":
                    CurrentView = new TodoListViewModel(new TodoService());
                    break;
                case "Settings":
                    CurrentView = new ConfigViewModel();
                    break;
                default:
                    CurrentView = new TableViewModel(new TableService());
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