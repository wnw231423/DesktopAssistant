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
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
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
            //调用AiService写入配置
            AiService.WriteApiKey(Input1.Trim(), Input2.Trim());
            if (Input1.Trim() == "" && Input2.Trim() != "")
            {
                ShowMessage("配置失败", "             API_KEY为空             ");
            }
            else if(Input1.Trim() != "" && Input2.Trim() == "")
            {
                ShowMessage("配置失败", "           SECRET_KEY为空           ");
            }
            else if(Input1.Trim() == "" && Input2.Trim() == "")
            {
                ShowMessage("配置失败", "    API_KEY和SECRET_KEY为空    ");
            }
            else
            {
                //显示成功提示
                ShowMessage("配置成功", "API密钥已保存到本地配置文件");
            }
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

    [RelayCommand]
    private async Task ImportCourses()
    {
        try
        {
            // 创建打开文件对话框
            var dialog = new OpenFileDialog
            {
                Title = "选择课程表文件",
                AllowMultiple = false
            };

            // 设置文件过滤器，假设导入文件是 JSON 格式
            dialog.Filters.Add(new FileDialogFilter
            {
                Name = "课程表文件",
                Extensions = new System.Collections.Generic.List<string> { "json" }
            });

            // 获取当前窗口
            var window = GetWindow();
            if (window == null) return;

            // 显示对话框
            var result = await dialog.ShowAsync(window);
        
            if (result != null && result.Length > 0)
            {
                string filePath = result[0];
            
                // 调用 TableService 的导入方法
                var tableService = new TableService(); // 如果已有实例应该通过依赖注入获取
                tableService.ImportCoursesFromJson(filePath);
            
                // 显示成功消息
                ShowMessage("导入成功", "课程表已成功导入");
            }
        }
        catch (Exception ex)
        {
            ShowError("导入失败", $"导入课程表时出错：{ex.Message}");
        }
    }
    
    [RelayCommand]
    private async Task ExportCourses()
    {
        try
        {
            // 创建选择文件夹对话框
            var dialog = new OpenFolderDialog
            {
                Title = "选择导出位置"
            };

            // 获取当前窗口
            var window = GetWindow();
            if (window == null) return;

            // 显示对话框
            var folderPath = await dialog.ShowAsync(window);

            if (!string.IsNullOrEmpty(folderPath))
            {
                // 在选择的文件夹中创建导出文件路径
                string exportPath = Path.Combine(folderPath, "courses_export.json");
            
                // 调用 TableService 的导出方法
                var tableService = new TableService();
                tableService.ExportCoursesToJson(exportPath);

                // 显示成功消息
                ShowMessage("导出成功", $"课程表已成功导出到：\n{exportPath}");
            }
        }
        catch (Exception ex)
        {
            ShowError("导出失败", $"导出课程表时出错：{ex.Message}");
        }
    }

    // 辅助方法：获取当前窗口
    private Window GetWindow()
    {
        if (Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        return null;
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