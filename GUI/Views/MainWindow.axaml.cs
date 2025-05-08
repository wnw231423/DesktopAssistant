using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Core.Table;
using GUI.ViewModels;

namespace GUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
    
        var titleBar = this.FindControl<Border>("TitleBarHost");
        if (titleBar != null)
        {
            // 使标题栏可以响应拖动
            this.PointerPressed += (s, e) =>
            {
                if (e.GetPosition(titleBar).Y <= titleBar.Height)
                {
                    this.BeginMoveDrag(e);
                }
            };
        }
    }
    
    private void ShowDailyReport(object sender, TappedEventArgs e)
    {
        var dailyReport = new DailyReportWindow();
        dailyReport.ShowDialog(this);
    }
}