using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleFlowChart
{
    public class Shape
    {
        public enum NodePoints {
            Top = 0,
            Right = 1,
            Bottom = 2,
            Left = 3
        }

        public UserControl UserControl { get; }
        public ObservableCollection<Node> Nodes { get; } = new ObservableCollection<Node>();
        private bool IsDragging;
        private Point ClickPosition;
        private Canvas Canvas;

        public Shape(UserControl uc, double x, double y)
        {
            UserControl = uc;
            Canvas = MainWindow.ServiceLocator.GetService<Canvas>();
            Canvas.Children.Add(UserControl);

            SetPosition(x, y);
            InitializeNodes();

            UserControl.MouseLeftButtonDown += MouseLeftButtonDown;
            UserControl.MouseMove += MouseMove;
            UserControl.MouseLeftButtonUp += MouseLeftButtonUp;
        }

        private void InitializeNodes()
        {
            // Initialize nodes based on the shape's geometry
            // This method should be overridden in derived classes
        }

        public void SetPosition(double x, double y)
        {
            Canvas.SetLeft(UserControl, x - UserControl.ActualWidth / 2);
            Canvas.SetTop(UserControl, y - UserControl.ActualHeight / 2);
            UpdateNodePositions();
        }

        private void UpdateNodePositions()
        {
            // Update positions of all nodes based on the new shape position
            // This method should be implemented based on the shape's geometry
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsDragging = true;
            ClickPosition = e.GetPosition(Canvas);
            ((UserControl)sender).CaptureMouse();
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDragging) return;

            Point currentPosition = e.GetPosition(Canvas);
            double offsetX = currentPosition.X - ClickPosition.X;
            double offsetY = currentPosition.Y - ClickPosition.Y;
            ClickPosition = currentPosition;

            double newLeft = Canvas.GetLeft(UserControl) + offsetX;
            double newTop = Canvas.GetTop(UserControl) + offsetY;

            Canvas.SetLeft(UserControl, newLeft);
            Canvas.SetTop(UserControl, newTop);
            UpdateNodePositions();
        }

        private void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsDragging = false;
            ((UserControl)sender).ReleaseMouseCapture();

            UserControl draggedShape = (UserControl)sender;
            double currentLeft = Canvas.GetLeft(draggedShape);
            double currentTop = Canvas.GetTop(draggedShape);

            Point shapeCenter = new Point(currentLeft + draggedShape.Width / 2, currentTop + draggedShape.Height / 2);
            Point nearestGridPoint = GetNearestGridPoint(shapeCenter, MainWindow.Constants.SnapThreshold);

            SetPosition(nearestGridPoint.X, nearestGridPoint.Y);
        }

        private Point GetNearestGridPoint(Point position, double gridSize)
        {
            double x = Math.Round(position.X / gridSize) * gridSize;
            double y = Math.Round(position.Y / gridSize) * gridSize;
            return new Point(x, y);
        }
    }
}

