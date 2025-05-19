using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GUI.ViewModels
{
    public partial class EnsureDialogViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = "确认";

        [ObservableProperty]
        private string _message = "确定要执行此操作吗？";

        [ObservableProperty]
        private string _confirmButtonText = "确定";

        [ObservableProperty]
        private string _cancelButtonText = "取消";

        public event EventHandler? ConfirmRequested;
        public event EventHandler? CancelRequested;

        public EnsureDialogViewModel() { }

        public EnsureDialogViewModel(string title, string message)
        {
            Title = title;
            Message = message;
        }

        public EnsureDialogViewModel(string title, string message, string confirmButtonText, string cancelButtonText)
        {
            Title = title;
            Message = message;
            ConfirmButtonText = confirmButtonText;
            CancelButtonText = cancelButtonText;
        }

        [RelayCommand]
        private void Confirm()
        {
            ConfirmRequested?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void Cancel()
        {
            CancelRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}