using Avalonia.Controls;

namespace GUI.Views;

public partial class CourseCardView : UserControl
{
    public CourseCardView()
    {
        InitializeComponent();
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