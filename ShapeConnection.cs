using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SimpleFlowChart
{
    public class ShapeConnection
    {
        public Shape Shape1 { get; }
        public Shape Shape2 { get; }
        public Polyline Line { get; private set; }
        private readonly Canvas Canvas;


        public ShapeConnection(Shape shape1, Shape shape2)
        {
            Shape1 = shape1;
            Shape2 = shape2;
            Canvas = ServiceLocator.GetService<Canvas>();

            CreateLine();
        }

        private void CreateLine()
        {
            // Create a polyline to connect the shapes with axis-aligned lines
            Line = new Polyline
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Points = []
            };

            UpdateLinePosition(Canvas.GetLeft(Shape1.UserControl), Canvas.GetTop(Shape1.UserControl));

            // Add the polyline to the canvas
            Canvas.Children.Add(Line);
        }

        private void UpdateLinePosition(double x, double y)
        {
            double x1 = x + Shape1.UserControl.Width / 2;
            double y1 = y + Shape1.UserControl.Height;
            double x2 = Canvas.GetLeft(Shape2.UserControl) + Shape2.UserControl.Width / 2;
            double y2 = Canvas.GetTop(Shape2.UserControl);

            Line.Points.Clear();
            Line.Points.Add(new Point(x1, y1));

            if (y1 != y2)
            {
                Line.Points.Add(new Point(x1, (y1 + y2) / 2));
                Line.Points.Add(new Point(x2, (y1 + y2) / 2));
            }
            else
            {
                Line.Points.Add(new Point(x2, y1));
            }

            Line.Points.Add(new Point(x2, y2));
        }

        public bool UpdateFrom()
        {
            double x = Canvas.GetLeft(Shape1.UserControl);
            double y = Canvas.GetTop(Shape1.UserControl);
            UpdateLinePosition(x, y);
            return true;
        }

        public bool UpdateTo(double x, double y)
        {
            UpdateLinePosition(x, y);
            return true;
        }
    }
}
