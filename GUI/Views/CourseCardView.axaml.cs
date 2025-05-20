using System;
using Avalonia.Controls;
using Avalonia.VisualTree;
using GUI.ViewModels;

namespace GUI.Views;

public partial class CourseCardView : UserControl
{
    public CourseCardView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }
    
    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        // 如果之前有 ViewModel，取消订阅事件
        if (DataContext is CourseCardViewModel oldViewModel)
        {
            oldViewModel.CloseRequest -= OnCloseRequest;
        }
        
        // 订阅新的 ViewModel 的事件
        if (DataContext is CourseCardViewModel viewModel)
        {
            viewModel.CloseRequest += OnCloseRequest;
        }
    }
    
    private void OnCloseRequest(object? sender, EventArgs e)
    {
        // 查找父窗口并关闭
        var window = this.FindAncestorOfType<Window>();
        window?.Close();
    }
    
    private void OnStartSlotChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!e.NewValue.HasValue) return;
        var endSlot = this.FindControl<NumericUpDown>("EndSlotInput");
        if (endSlot != null)
        {
            endSlot.Minimum = (decimal)e.NewValue;
        }
    }

    private void OnEndSlotChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!e.NewValue.HasValue) return;
        var startSlot = this.FindControl<NumericUpDown>("StartSlotInput");
        if (startSlot != null)
        {
            startSlot.Maximum = (decimal)e.NewValue;
        }
    }
}