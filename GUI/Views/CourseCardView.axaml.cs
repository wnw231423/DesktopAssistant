using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GUI.ViewModels;

namespace GUI.Views;

public partial class CourseCardView : UserControl
{
    public CourseCardView()
    {
        InitializeComponent();
    }
    
    public void SetCourse(Data.Models.Table.Course course)
    {
        if (DataContext is CourseCardViewModel viewModel)
        {
            viewModel.Course = course;
        }
    }
}