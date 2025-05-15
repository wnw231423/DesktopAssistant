using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using GUI.ViewModels;
using System;

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
        var viewModel = new DailyReportWindowViewModel();
        DataContext = viewModel;
        await viewModel.LoadAiResponse();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}