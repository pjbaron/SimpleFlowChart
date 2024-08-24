using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;


namespace SimpleFlowChart
{
    public class Shape
    {
        public UserControl UserControl;
        public ShapeConnection? ConnectionIn;
        public List<ShapeConnection> ConnectionsOut;
        private bool IsDragging;
        private Point ClickPosition;
        private Canvas Canvas;


        public Shape(UserControl uc, double x, double y)
        {
            UserControl = uc;
            SetPosition(x, y);

            Canvas = ServiceLocator.GetService<Canvas>();
            Canvas.Children.Add(UserControl);

            ConnectionIn = null;
            ConnectionsOut = [];
            IsDragging = false;

            UserControl.MouseLeftButtonDown += MouseLeftButtonDown;
            UserControl.MouseMove += MouseMove;
            UserControl.MouseLeftButtonUp += MouseLeftButtonUp;
        }

        public void AddConnectionOut(Shape shapeConnected)
        {
            // connection exits this Shape
            ShapeConnection connection = new ShapeConnection(this, shapeConnected);
            ConnectionsOut.Add(connection);
            // connection enters the connected Shape
            shapeConnected.AddConnectionIn(connection);
        }

        public void AddConnectionIn(ShapeConnection connectionIn)
        {
            ConnectionIn = connectionIn;
        }

        public void SetPosition(double x, double y)
        {
            Canvas.SetLeft(UserControl, x - UserControl.Width / 2);
            Canvas.SetTop(UserControl, y + UserControl.Height / 2);
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

            double initialLeft = Canvas.GetLeft(UserControl);
            double initialTop = Canvas.GetTop(UserControl);
            double newLeft = initialLeft + offsetX;
            double newTop = initialTop + offsetY;

            bool moveAllowed = true;
            if (ConnectionIn != null)
            {
                ConnectionIn.UpdateFrom();
            }
            foreach (var connection in ConnectionsOut)
            {
                moveAllowed |= connection.UpdateTo(newLeft, newTop);
            }

            if (moveAllowed)
            {
                Canvas.SetLeft(UserControl, newLeft);
                Canvas.SetTop(UserControl, newTop);
            }
        }

        private void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsDragging = false;
            ((UserControl)sender).ReleaseMouseCapture();

            UserControl draggedShape = (UserControl)sender;
            double currentLeft = Canvas.GetLeft(draggedShape);
            double currentTop = Canvas.GetTop(draggedShape);

            Point shapeCenter = new Point(currentLeft + draggedShape.Width / 2, currentTop + draggedShape.Height / 2);

            Point nearestGridPoint = GetNearestGridPoint(shapeCenter, Constants.SnapThreshold);

            double newLeft = nearestGridPoint.X - draggedShape.Width / 2;
            double newTop = nearestGridPoint.Y - draggedShape.Height / 2;
            Canvas.SetLeft(draggedShape, newLeft);
            Canvas.SetTop(draggedShape, newTop);

            if (ConnectionIn != null)
            {
                ConnectionIn.UpdateFrom();
            }
            foreach (var connection in ConnectionsOut)
            {
                connection.UpdateTo(newLeft, newTop);
            }
        }

        private Point GetNearestGridPoint(Point position, double gridSize)
        {
            double x = Math.Round(position.X / gridSize) * gridSize;
            double y = Math.Round(position.Y / gridSize) * gridSize;
            return new Point(x, y);
        }
    }
}
