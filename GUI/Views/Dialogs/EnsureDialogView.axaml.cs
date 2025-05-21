using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using GUI.ViewModels;

namespace GUI.Views
{
    public partial class EnsureDialogView : Window
    {
        public EnsureDialogView()
        {
            InitializeComponent();
            
            // 创建问号图标
            var drawingGroup = new DrawingGroup();

            // 添加圆形背景
            drawingGroup.Children.Add(new GeometryDrawing
            {
                Brush = new SolidColorBrush(Color.Parse("#FF333333")),
                // 使用正确的 EllipseGeometry 构造方式
                Geometry = new EllipseGeometry(new Rect(0, 0, 32, 32))
            });

            // 添加问号的竖线
            var pathGeometry = new PathGeometry();
            var pathFigure = new PathFigure { StartPoint = new Point(16, 7), IsClosed = false };
            pathFigure.Segments.Add(new LineSegment { Point = new Point(16, 20) });
            pathGeometry.Figures.Add(pathFigure);

            drawingGroup.Children.Add(new GeometryDrawing
            {
                Brush = Brushes.White,
                Geometry = pathGeometry
            });

            // 添加问号的点
            drawingGroup.Children.Add(new GeometryDrawing
            {
                Brush = Brushes.White,
                // 使用正确的 EllipseGeometry 构造方式
                Geometry = new EllipseGeometry(new Rect(15, 23, 2, 2))
            });

            // 将 DrawingImage 转换为 Bitmap
            var drawingImage = new DrawingImage(drawingGroup);
            using var memoryStream = new MemoryStream();
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