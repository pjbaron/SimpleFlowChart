#pragma warning disable CS8618

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;



namespace SimpleFlowChart
{
    public class RectangleShape : Shape
    {
        private Polygon RectanglePolygon;

        public RectangleShape(double cx, double cy, double width = 100, double height = 60, string text = "") : base(cx, cy, width, height)
        {
            InitializeRectangle();
            InitializeNodes();
        }

        private void InitializeRectangle()
        {
            RectanglePolygon = new Polygon
            {
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            RectanglePolygon.Points.Clear();
            RectanglePolygon.Points.Add(new Point(0, 0));
            RectanglePolygon.Points.Add(new Point(Width, 0));
            RectanglePolygon.Points.Add(new Point(Width, Height));
            RectanglePolygon.Points.Add(new Point(0, Height));

            this.Content = RectanglePolygon;
        }

        private void InitializeNodes()
        {
            // Create nodes at the middle of each Rectangle side
            Nodes.Clear();
            Nodes.Add(new Node(this, new Point(Width / 2, 0)));
            Nodes.Add(new Node(this, new Point(Width, Height / 2)));
            Nodes.Add(new Node(this, new Point(Width / 2, Height)));
            Nodes.Add(new Node(this, new Point(0, Height / 2)));
            UpdateNodePositions();
        }

        public override void UpdateNodePositions()
        {
            double left = Canvas.GetLeft(this);
            double top = Canvas.GetTop(this);

            Debug.WriteLine($"Rect: UpdateNodePositions {left},{top}. Width: {Width}, Height: {Height}");

            Nodes[0].UpdatePosition(new Point(left + Width / 2, top));
            Nodes[1].UpdatePosition(new Point(left + Width, top + Height / 2));
            Nodes[2].UpdatePosition(new Point(left + Width / 2, top + Height));
            Nodes[3].UpdatePosition(new Point(left, top + Height / 2));
        }
    }
}
