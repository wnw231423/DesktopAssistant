using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Table;
using Data.Models.Table;

namespace GUI.ViewModels
{
    public partial class ResourcesViewModel : ViewModelBase
    {
        private readonly TableService _tableService;

        [ObservableProperty]
        private Course _course;

        [ObservableProperty]
        private ObservableCollection<ResourceGroup> _resourceGroups;

        public ResourcesViewModel(TableService tableService, Course course)
        {
            _tableService = tableService;
            _course = course;
            
            // 初始化资源组
            ResourceGroups = new ObservableCollection<ResourceGroup>();
            
            // 加载资源
            LoadResources();
        }

        private void LoadResources()
        {
            // 清空当前资源组
            ResourceGroups.Clear();
            
            try
            {
                // 获取课程所有资源
                var resources = _tableService.GetCourseResources(Course.CourseName);
                
                // 按资源类型分组
                var groupedResources = resources
                    .GroupBy(r => r.ResourceType)
                    .OrderBy(g => g.Key);
                
                foreach (var group in groupedResources)
                {
                    ResourceGroups.Add(new ResourceGroup
                    {
                        TypeName = group.Key,
                        IsExpanded = true,
                        Resources = new ObservableCollection<ResourceInfo>(
                            group.OrderByDescending(r => r.LastModified))
                    });
                }
                
                // TODO: 这里是添加测试数据， 后面
                // 如果没有资源，添加一些默认分组
                if (ResourceGroups.Count == 0)
                {
                    AddMockData();
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
        
        // 这些命令将来可以实现添加、打开、删除功能
        [RelayCommand]
        private void AddResource(string resourceType)
        {
            // 将来可以实现
        }
        
        [RelayCommand]
        private void OpenResource(ResourceInfo resource)
        {
            // 将来可以实现
        }
        
        [RelayCommand]
        private void DeleteResource(ResourceInfo resource)
        {
            // 将来可以实现
        }
        
        [RelayCommand]
        private void AddResourceType()
        {
            // 将来可以实现
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