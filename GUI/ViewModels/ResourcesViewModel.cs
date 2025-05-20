using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Table;
using Data.Models.Table;
using Avalonia;
using MsBox.Avalonia;
using GUI.Views.Dialogs;

namespace GUI.ViewModels
{
    public partial class ResourcesViewModel : ViewModelBase
    {
        private readonly TableService _tableService;

        [ObservableProperty]
        private Course _course;

        [ObservableProperty]
        private ObservableCollection<ResourceGroup> _resourceGroups;
        
        [ObservableProperty]
        private bool _isEditMode;
        
        public ResourcesViewModel(TableService tableService, Course course)
        {
            _tableService = tableService;
            _course = course;
            
            // 初始化资源组
            ResourceGroups = new ObservableCollection<ResourceGroup>();
            
            // 加载资源
            LoadResources();

            IsEditMode = false;
        }
        
        [RelayCommand]
        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
        }

        private void LoadResources()
        {
            // 清空当前资源组
            ResourceGroups.Clear();
            
            try
            {
                // 获取课程所有类型
                var resources = _tableService.GetCourseResourceTypes(Course.CourseName);
                // 遍历每个资源类型
                foreach (var resourceType in resources)
                {
                    // 创建资源组
                    var resourceGroup = new ResourceGroup
                    {
                        TypeName = resourceType,
                        IsExpanded = true,
                        Resources = new ObservableCollection<ResourceInfo>()
                    };
                    
                    // 获取该类型下的所有资源
                    var resourceInfos = _tableService.GetCourseResources(Course.CourseName, resourceType);
                    
                    // 添加到资源组
                    foreach (var resourceInfo in resourceInfos)
                    {
                        resourceGroup.Resources.Add(resourceInfo);
                    }
                    
                    // 添加到UI
                    ResourceGroups.Add(resourceGroup);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载资源失败：{ex.Message}");
            }
        }
        
        private void AddMockData()
        {
            // 创建几个测试资源组
            var lectureGroup = new ResourceGroup { TypeName = "讲义", IsExpanded = true };
            var homeworkGroup = new ResourceGroup { TypeName = "作业", IsExpanded = true };
            var referenceGroup = new ResourceGroup { TypeName = "参考资料", IsExpanded = false };
    
            // 为"讲义"组添加资源
            lectureGroup.Resources.Add(new ResourceInfo 
            { 
                ResourceName = "第一章 - 导论.pptx", 
                ResourcePath = "D:\\课程资料\\导论.pptx",
                ResourceType = "讲义",
                LastModified = DateTime.Now.AddDays(-10)
            });
    
            lectureGroup.Resources.Add(new ResourceInfo 
            { 
                ResourceName = "第二章 - 基本概念.pptx", 
                ResourcePath = "D:\\课程资料\\基本概念.pptx",
                ResourceType = "讲义",
                LastModified = DateTime.Now.AddDays(-5)
            });
    
            // 为"作业"组添加资源
            homeworkGroup.Resources.Add(new ResourceInfo 
            { 
                ResourceName = "作业1.pdf", 
                ResourcePath = "D:\\课程资料\\作业1.pdf",
                ResourceType = "作业",
                LastModified = DateTime.Now.AddDays(-8)
            });
    
            homeworkGroup.Resources.Add(new ResourceInfo 
            { 
                ResourceName = "作业2.pdf", 
                ResourcePath = "D:\\课程资料\\作业2.pdf",
                ResourceType = "作业",
                LastModified = DateTime.Now.AddDays(-3)
            });
    
            // 为"参考资料"组添加资源
            referenceGroup.Resources.Add(new ResourceInfo 
            { 
                ResourceName = "参考书目.pdf", 
                ResourcePath = "D:\\课程资料\\参考书目.pdf",
                ResourceType = "参考资料",
                LastModified = DateTime.Now.AddDays(-15)
            });
    
            // 添加到资源组集合
            ResourceGroups.Add(lectureGroup);
            ResourceGroups.Add(homeworkGroup);
            ResourceGroups.Add(referenceGroup);
        }

        [RelayCommand]
        private void ToggleExpand(ResourceGroup group)
        {
            if (group != null)
            {
                group.IsExpanded = !group.IsExpanded;
            }
        }
        
        [RelayCommand]
        private async Task AddResourceType()
        {
            // 创建输入对话框
            var dialog = new TextInputDialog
            {
                Title = "添加资源类型",
                Message = "请输入新的资源类型名称：",
                PlaceholderText = "例如: 作业, 课件, 参考资料等"
            };

            // 获取当前窗口
            var window = GetWindow();
            if (window == null) return;

            // 显示对话框并获取结果
            var result = await dialog.ShowAsync(window);
            
            // 如果用户输入了类型名称
            if (!string.IsNullOrWhiteSpace(result))
            {
                string newType = result.Trim();
                
                // 检查是否已存在该类型
                if (ResourceGroups.Any(g => g.TypeName.Equals(newType, StringComparison.OrdinalIgnoreCase)))
                {
                    await MessageBoxManager.GetMessageBoxStandard(
                        "提示", 
                        $"资源类型 \"{newType}\" 已存在！", 
                        MsBox.Avalonia.Enums.ButtonEnum.Ok).ShowAsync();
                    return;
                }
                
                // 创建资源组对应的文件夹
                try
                {
                    var courseResourceDir = Path.Combine(_tableService.GetResourcesBasePath(), Course.CourseName);
                    var resourceTypeDir = Path.Combine(courseResourceDir, newType);
                    
                    if (!Directory.Exists(courseResourceDir))
                    {
                        Directory.CreateDirectory(courseResourceDir);
                    }
                    
                    if (!Directory.Exists(resourceTypeDir))
                    {
                        Directory.CreateDirectory(resourceTypeDir);
                    }
                    
                    // 添加到UI
                    ResourceGroups.Add(new ResourceGroup 
                    { 
                        TypeName = newType, 
                        IsExpanded = true,
                        Resources = new ObservableCollection<ResourceInfo>()
                    });
                }
                catch (Exception ex)
                {
                    await MessageBoxManager.GetMessageBoxStandard(
                        "错误", 
                        $"创建资源类型失败：{ex.Message}", 
                        MsBox.Avalonia.Enums.ButtonEnum.Ok).ShowAsync();
                }
            }
        }

        [RelayCommand]
        private async Task AddResource(string resourceType)
        {
            var resourceGroup = ResourceGroups.FirstOrDefault(g => g.TypeName == resourceType);
            if (resourceGroup == null) return;

            // 打开文件对话框
            var dialog = new OpenFileDialog
            {
                Title = $"选择要添加到 {resourceType} 的文件",
                AllowMultiple = true
            };

            var window = GetWindow();
            if (window == null) return;

            // 获取用户选择的文件
            var result = await dialog.ShowAsync(window);
            
            if (result != null && result.Length > 0)
            {
                foreach (var filePath in result)
                {
                    try
                    {
                        // 使用TableService添加资源
                        _tableService.AddCourseResource(Course.CourseName, resourceType, filePath);
                        
                        // 获取文件信息
                        var fileInfo = new FileInfo(filePath);
                        string fileName = Path.GetFileName(filePath);
                        
                        // 构建目标路径
                        var targetPath = Path.Combine(
                            _tableService.GetResourcesBasePath(),
                            Course.CourseName,
                            resourceType,
                            fileName);
                        
                        // 添加到资源列表
                        var resource = new ResourceInfo
                        {
                            ResourceName = fileName,
                            ResourcePath = targetPath,
                            ResourceType = resourceType,
                            LastModified = fileInfo.LastWriteTime
                        };
                        
                        // 更新UI
                        resourceGroup.Resources.Add(resource);
                        
                        // 确保资源组按最后修改日期排序
                        var sortedResources = new ObservableCollection<ResourceInfo>(
                            resourceGroup.Resources.OrderByDescending(r => r.LastModified));
                        resourceGroup.Resources = sortedResources;
                    }
                    catch (Exception ex)
                    {
                        await MessageBoxManager.GetMessageBoxStandard(
                            "错误", 
                            $"添加资源失败：{ex.Message}", 
                            MsBox.Avalonia.Enums.ButtonEnum.Ok).ShowAsync();
                    }
                }
            }
        }

        [RelayCommand]
        private async Task OpenResource(ResourceInfo resource)
        {
            if (resource == null) return;

            try
            {
                string filePath = Path.Combine(
                    _tableService.GetResourcesBasePath(),
                    Course.CourseName,
                    resource.ResourceType,
                    resource.ResourceName);

                // 检查文件是否存在
                if (!File.Exists(filePath))
                {
                    await MessageBoxManager.GetMessageBoxStandard(
                        "错误",
                        $"找不到文件：{filePath}",
                        MsBox.Avalonia.Enums.ButtonEnum.Ok).ShowAsync();
                    return;
                }

                // 使用系统默认程序打开文件
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true, // 使用shell执行，让系统选择合适的程序打开
                    WorkingDirectory = Path.GetDirectoryName(filePath)
                };

                System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard(
                    "错误",
                    $"打开资源失败：{ex.Message}",
                    MsBox.Avalonia.Enums.ButtonEnum.Ok).ShowAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteResource(ResourceInfo resource)
        {
            if (resource == null) return;
            
            // 确认删除
            var result = await MessageBoxManager.GetMessageBoxStandard(
                "确认删除", 
                $"确定要删除资源 \"{resource.ResourceName}\" 吗？", 
                MsBox.Avalonia.Enums.ButtonEnum.YesNo).ShowAsync();
            
            if (result == MsBox.Avalonia.Enums.ButtonResult.Yes)
            {
                try
                {
                    // 使用TableService删除资源
                    _tableService.DeleteCourseResource(
                        Course.CourseName,
                        resource.ResourceType,
                        resource.ResourceName);
                    
                    // 从UI中移除
                    var group = ResourceGroups.FirstOrDefault(g => g.TypeName == resource.ResourceType);
                    if (group != null)
                    {
                        group.Resources.Remove(resource);
                    }
                }
                catch (Exception ex)
                {
                    await MessageBoxManager.GetMessageBoxStandard(
                        "错误", 
                        $"删除资源失败：{ex.Message}", 
                        MsBox.Avalonia.Enums.ButtonEnum.Ok).ShowAsync();
                }
            }
        }
        
        [RelayCommand]
        private async Task DeleteResourceType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return;
    
            // 确认删除
            var result = await MessageBoxManager.GetMessageBoxStandard(
                "确认删除", 
                $"确定要删除资源类型 \"{typeName}\" 及其所有资源吗？", 
                MsBox.Avalonia.Enums.ButtonEnum.YesNo).ShowAsync();
    
            if (result == MsBox.Avalonia.Enums.ButtonResult.Yes)
            {
                try
                {
                    // 使用TableService删除资源类型
                    _tableService.DeleteCourseResourceType(Course.CourseName, typeName);
            
                    // 从UI中移除
                    var group = ResourceGroups.FirstOrDefault(g => g.TypeName == typeName);
                    if (group != null)
                    {
                        ResourceGroups.Remove(group);
                    }
                }
                catch (Exception ex)
                {
                    await MessageBoxManager.GetMessageBoxStandard(
                        "错误", 
                        $"删除资源类型失败：{ex.Message}", 
                        MsBox.Avalonia.Enums.ButtonEnum.Ok).ShowAsync();
                }
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
    }
    
    public partial class ResourceGroup : ObservableObject
    {
        [ObservableProperty]
        private string _typeName = string.Empty;

        [ObservableProperty]
        private bool _isExpanded = true;

        [ObservableProperty]
        private ObservableCollection<ResourceInfo> _resources = new();
    }
}