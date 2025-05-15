using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Table;
using Data.Models.Table;
using MsBox.Avalonia.Base;
using Core.AI;
using Data.Models.AI;
using System.ComponentModel.DataAnnotations;
using MsBox.Avalonia;


namespace GUI.ViewModels;

public partial class ConfigViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _input1 = string.Empty;

    [ObservableProperty]
    private string _input2 = string.Empty;

    [RelayCommand]
    private void Confirm()
    {
        try
        {
            // 调用AiService写入配置
            AiService.WriteApiKey(Input1.Trim(), Input2.Trim());

            // 显示成功提示
            ShowMessage("配置成功", "API密钥已保存到本地配置文件");
        }
        catch (ArgumentException ex)
        {
            ShowError("参数错误", ex.Message);
        }
        catch (Exception ex)
        {
            ShowError("保存失败", $"配置保存失败：{ex.Message}");
        }
    }

    private async void ShowMessage(string title, string message)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            title,
            message,
            MsBox.Avalonia.Enums.ButtonEnum.Ok);

        await box.ShowAsync();
    }

    private async void ShowError(string title, string message)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            title,
            message,
            MsBox.Avalonia.Enums.ButtonEnum.Ok,
            MsBox.Avalonia.Enums.Icon.Error);

        await box.ShowAsync();
    }
}