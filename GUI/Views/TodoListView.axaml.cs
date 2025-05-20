using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Data.Models.Todo;
using GUI.ViewModels;

namespace GUI.Views
{
    public partial class TodoListView : UserControl
    {
        public TodoListView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnAddItemKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext is TodoListViewModel viewModel)
            {
                viewModel.AddItemCommand.Execute(null);
                e.Handled = true;
            }
        }

        private void ContentEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox &&
                textBox.DataContext is TodoItem item &&
                DataContext is TodoListViewModel viewModel)
            {
                viewModel.UpdateItemContentCommand.Execute(item);
            }
        }
    }
}