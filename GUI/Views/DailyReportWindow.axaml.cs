using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Markdown.Avalonia;
using GUI.ViewModels;
using System;
using Core.Table;
using Core.Todo;

namespace GUI.Views;

public partial class DailyReportWindow : Window
{
    public DailyReportWindow()
    {
        InitializeComponent();
        InitializeAsync();
    }
    private async void InitializeAsync()
    {
        var viewModel = new DailyReportWindowViewModel(new TableService(), new TodoService());
        DataContext = viewModel;
        await viewModel.LoadAiResponse();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}