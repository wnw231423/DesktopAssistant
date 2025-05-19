using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GUI.ViewModels;

namespace GUI.Views
{
    public partial class EnsureDialogView : Window
    {
        public EnsureDialogView()
        {
            InitializeComponent();
        }

        public EnsureDialogView(EnsureDialogViewModel viewModel) : this()
        {
            DataContext = viewModel;
            
            // 当确认或取消按钮被点击时关闭对话框
            viewModel.ConfirmRequested += (_, _) => Close(true);
            viewModel.CancelRequested += (_, _) => Close(false);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}