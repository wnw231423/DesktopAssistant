using Avalonia.Controls;
using GUI.ViewModels;

namespace GUI.Views;

public partial class ConfigView : UserControl
{
    public ConfigView()
    {
        InitializeComponent();
        DataContext = new ConfigViewModel();
    }
}