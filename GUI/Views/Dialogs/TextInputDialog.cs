using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace GUI.Views.Dialogs
{
    public class TextInputDialog : Window
    {
        private readonly TextBox _inputTextBox;
        private readonly TaskCompletionSource<string> _resultSource = new();

        public string Title { get; set; }
        public string Message { get; set; }
        public string PlaceholderText { get; set; }

        public TextInputDialog()
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            MinWidth = 350;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            CanResize = false;
            
            _inputTextBox = new TextBox 
            { 
                Watermark = "请输入...",
                MinWidth = 250
            };
            
            var messageTextBlock = new TextBlock 
            { 
                Text = "请输入:",
                Margin = new Thickness(0, 0, 0, 10)
            };
            
            var okButton = new Button 
            { 
                Content = "确定",
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 15, 0, 0)
            };
            
            okButton.Click += (s, e) => 
            {
                _resultSource.TrySetResult(_inputTextBox.Text);
                Close();
            };
            
            Content = new StackPanel 
            {
                Margin = new Thickness(20),
                Children = { messageTextBlock, _inputTextBox, okButton }
            };
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            this.Title = Title;
            ((TextBlock)((StackPanel)Content).Children[0]).Text = Message;
            _inputTextBox.Watermark = PlaceholderText;
            _inputTextBox.Focus();
        }

        public Task<string> ShowAsync(Window owner)
        {
            this.ShowDialog(owner);
            return _resultSource.Task;
        }
    }
}