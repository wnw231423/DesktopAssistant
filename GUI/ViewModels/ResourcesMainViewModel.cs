using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Table;
using Data.Models.Table;

namespace GUI.ViewModels
{
    public partial class ResourcesMainViewModel : ViewModelBase
    {
        private readonly TableService _tableService;

        [ObservableProperty]
        private ObservableCollection<Course> _courses;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ResourceViewModel))]
        private Course _selectedCourse;

        [ObservableProperty]
        private ResourcesViewModel _resourceViewModel;

        public ResourcesMainViewModel(TableService tableService)
        {
            _tableService = tableService;
            LoadCourses();
        }

        private void LoadCourses()
        {
            var courseList = _tableService.GetCourses();
            Courses = new ObservableCollection<Course>(courseList);
        }

        partial void OnSelectedCourseChanged(Course value)
        {
            if (value != null)
            {
                ResourceViewModel = new ResourcesViewModel(_tableService, value);
            }
        }
    }
}