﻿#pragma warning disable CS8618

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;



namespace SimpleFlowChart
{
    public class Shape : UserControl
    {
        public enum NodePoints {
            Top = 0,
            Right = 1,
            Bottom = 2,
            Left = 3
        }

        public List<Node> Nodes { get; } = new List<Node>();
        private bool IsDragging;
        private Point DragAnchorPoint;
        private Canvas Canvas;


        public Shape(double x, double y, double width, double height)
        {
            Width = width;
            Height = height;

            Canvas = MainWindow.ServiceLocator.GetService<Canvas>();
            Canvas.Children.Add(this);

            SetPosition(x, y);

            MouseLeftButtonDown += DragStart;
            MouseMove += Dragging;
            MouseLeftButtonUp += DragDrop;
        }

        private void DragStart(object sender, MouseButtonEventArgs e)
        {
            IsDragging = true;
            DragAnchorPoint = e.GetPosition(Canvas);
            ((UserControl)sender).CaptureMouse();
        }

        private void Dragging(object sender, MouseEventArgs e)
        {
            if (!IsDragging) return;

            Point dragPoint = e.GetPosition(Canvas);
            Point offset = (Point)(dragPoint - DragAnchorPoint);
            DragAnchorPoint = dragPoint;

            SetPosition(Canvas.GetLeft(this) + offset.X + Width / 2, Canvas.GetTop(this) + offset.Y + Height / 2);
        }

        private void DragDrop(object sender, MouseButtonEventArgs e)
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

        public void SetPosition(double x, double y)
        {
            Canvas.SetLeft(this, x - Width / 2);
            Canvas.SetTop(this, y - Height / 2);
            if (Nodes.Count > 0)
            {
                UpdateNodePositions();
            }
        }

        public virtual void UpdateNodePositions()
        {

        }

        private Point GetNearestGridPoint(Point position, double gridSize)
        {
            double x = Math.Round(position.X / gridSize) * gridSize;
            double y = Math.Round(position.Y / gridSize) * gridSize;
            return new Point(x, y);
        }
    }
}

