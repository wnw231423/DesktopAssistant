using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Todo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Data.Models.Todo;

namespace GUI.ViewModels
{
    public partial class TodoListViewModel : ViewModelBase
    {
        private readonly TodoService _todoService;

        [ObservableProperty]
        private bool isCourseSpecified;

        [ObservableProperty]
        private string? _specificCourseTag;

        [ObservableProperty]
        private string _newItemContent = string.Empty;

        [ObservableProperty]
        private ObservableCollection<TodoItem> _todoItems = new();

        [ObservableProperty]
        private bool _showCompletedTasks = true;

        [ObservableProperty]
        private string _selectedSortOption = "开始时间";

        [ObservableProperty]
        private bool _isDetailedAddMode = false;

        [ObservableProperty]
        private DateTime _startTime = DateTime.Now;

        [ObservableProperty]
        private DateTime? _endTime = DateTime.Now.AddDays(1);

        [ObservableProperty]
        private bool _isLongTerm = false;

        [ObservableProperty]
        private string _courseTag = string.Empty;

        [ObservableProperty]
        private string _selectedViewMode = "所有任务";

        public List<string> SortOptions { get; } = new List<string> { "开始时间", "结束时间" };
        public List<string> ViewModes { get; } = new List<string> { "所有任务", "按课程分类" };
        public ObservableCollection<string> AvailableCourses { get; } = new ();

        public TodoListViewModel(TodoService todoService, string specificCourseTag = null)
        {
            _todoService = todoService;
            IsCourseSpecified = !string.IsNullOrEmpty(specificCourseTag);
            SpecificCourseTag = specificCourseTag;

            if (IsCourseSpecified)
            {
                CourseTag = specificCourseTag;
            }

            LoadTodoItems();
            LoadAvailableCourses();
        }

        private void LoadAvailableCourses()
        {
            var courses = _todoService.GetTodoItems()
                .Select(t => t.CourseTag)
                .Where(tag => !string.IsNullOrEmpty(tag))
                .Distinct()
                .ToList();

            AvailableCourses.Clear();
            foreach (var course in courses)
            {
                AvailableCourses.Add(course);
            }
        }

        [RelayCommand]
        private void ToggleDetailedAddMode()
        {
            IsDetailedAddMode = !IsDetailedAddMode;
        }

        [RelayCommand]
        private void AddItem()
        {
            if (string.IsNullOrWhiteSpace(NewItemContent))
                return;

            string tag = IsCourseSpecified ? SpecificCourseTag : CourseTag;
            DateTime start = StartTime;
            DateTime? end = IsLongTerm ? null : EndTime;

            var todoItem = new TodoItem(NewItemContent, start, end, IsLongTerm, tag);
            _todoService.AddTodoItem(todoItem);

            NewItemContent = string.Empty;
            System.Diagnostics.Debug.WriteLine($"添加任务: {todoItem.Content}");
            
            LoadTodoItems();
        }

        [RelayCommand]
        private void DeleteItem(TodoItem item)
        {
            if (item == null) return;
            
            System.Diagnostics.Debug.WriteLine($"删除任务: {item.Content}");
            _todoService.RemoveTodoItem(item.Id);
            LoadTodoItems();
        }

        [RelayCommand]
        private void ToggleItemStatus(TodoItem item)
        {
            if (item == null) return;
    
            // 创建副本而不是直接修改原对象
            bool newStatus = !item.IsDone;
    
            System.Diagnostics.Debug.WriteLine($"切换任务状态: {item.Content} ({item.Id}) -> {newStatus}");
    
            // 更新数据库/服务
            var updatedItem = _todoService.ToggleTodoItemStatus(item.Id, newStatus);
    
            // 如果服务层返回更新后的对象则使用它，否则创建副本
            if (updatedItem == null)
            {
                updatedItem = new TodoItem(
                    item.Content,
                    item.StartTime,
                    item.EndTime,
                    item.IsLongTerm,
                    item.CourseTag
                )
                {
                    Id = item.Id,
                    IsDone = newStatus
                };
            }
    
            // 在集合中查找并替换
            int index = -1;
            for (int i = 0; i < TodoItems.Count; i++)
            {
                if (TodoItems[i].Id == item.Id)
                {
                    index = i;
                    break;
                }
            }
    
            if (index >= 0)
            {
                TodoItems[index] = updatedItem;
                System.Diagnostics.Debug.WriteLine($"已替换 TodoItems[{index}]");
            }
        }

        [RelayCommand]
        private void UpdateItemContent(TodoItem item)
        {
            if (item == null) return;
            
            System.Diagnostics.Debug.WriteLine($"更新任务内容: {item.Content}");
            _todoService.UpdateTodoItem(item);
            LoadTodoItems();
        }

        [RelayCommand]
        private void LoadTodoItems()
        {
            try
            {
                // 获取数据的副本而不是引用
                var items = _todoService.GetTodoItems().ToList();
        
                System.Diagnostics.Debug.WriteLine($"从服务获取了 {items.Count} 个任务项");
        
                // 筛选和排序前进行深拷贝，避免修改原始数据
                var filteredItems = items.Select(item => new TodoItem(
                    item.Content,
                    item.StartTime,
                    item.EndTime,
                    item.IsLongTerm,
                    item.CourseTag
                ) {
                    Id = item.Id,
                    IsDone = item.IsDone
                }).AsEnumerable();
        
                if (IsCourseSpecified)
                {
                    filteredItems = filteredItems.Where(item => item.CourseTag == SpecificCourseTag);
                }

                if (!ShowCompletedTasks)
                {
                    filteredItems = filteredItems.Where(item => !item.IsDone);
                }

                switch (SelectedSortOption)
                {
                    case "开始时间":
                        filteredItems = filteredItems.OrderBy(item => item.StartTime);
                        break;
                    case "结束时间":
                        filteredItems = filteredItems.OrderBy(item => item.IsLongTerm)
                            .ThenBy(item => item.EndTime ?? DateTime.MaxValue);
                        break;
                }

                // 使用临时列表避免多次触发集合变更通知
                var tempList = filteredItems.ToList();
                TodoItems.Clear();
        
                foreach (var item in tempList)
                {
                    TodoItems.Add(item);
                }
        
                System.Diagnostics.Debug.WriteLine($"加载了 {TodoItems.Count} 个任务到视图");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载任务出错: {ex}");
                TodoItems.Clear();
            }
        }

        partial void OnSelectedSortOptionChanged(string value) => LoadTodoItems();
        partial void OnSelectedViewModeChanged(string value) => LoadTodoItems();
        partial void OnShowCompletedTasksChanged(bool value) => LoadTodoItems();

        partial void OnIsLongTermChanged(bool value)
        {
            if (value)
            {
                EndTime = null;
            }
        }
    }
}