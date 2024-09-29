#pragma warning disable CS8618

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SimpleFlowChart
{
    public class RectangleShape : VisualShape
    {
        private Polygon RectanglePolygon;

        private const double DefaultStrokeThickness = 1;
        private const double HighlightedStrokeThickness = 3;
        private Brush DefaultFill = Brushes.White;
        private Brush HighlightFill = Brushes.LightYellow;

        public RectangleShape(double cx, double cy, double width = 100, double height = 60, string text = "") : base(cx, cy, width, height, text)
        {
            InitializeShape();
            InitializeNodes();
        }

        private void InitializeShape()
        {
            RectanglePolygon = new Polygon
            {
                Fill = DefaultFill,
                Stroke = Brushes.Black,
                StrokeThickness = DefaultStrokeThickness,
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
            Nodes.Add(new NodeShape(this, new Point(Width / 2, 0)));
            Nodes.Add(new NodeShape(this, new Point(Width, Height / 2)));
            Nodes.Add(new NodeShape(this, new Point(Width / 2, Height)));
            Nodes.Add(new NodeShape(this, new Point(0, Height / 2)));
            UpdateNodePositions();
        }

        public override void UpdateNodePositions()
        {
            double left = Canvas.GetLeft(this);
            double top = Canvas.GetTop(this);

            Nodes[0].UpdatePosition(new Point(left + Width / 2, top));
            Nodes[1].UpdatePosition(new Point(left + Width, top + Height / 2));
            Nodes[2].UpdatePosition(new Point(left + Width / 2, top + Height));
            Nodes[3].UpdatePosition(new Point(left, top + Height / 2));
        }


        public override void Highlight(bool isHighlighted)
        {
            if (isHighlighted)
            {
                RectanglePolygon.Fill = HighlightFill;
                RectanglePolygon.StrokeThickness = HighlightedStrokeThickness;
            }
            else
            {
                RectanglePolygon.Fill = DefaultFill;
                RectanglePolygon.StrokeThickness = DefaultStrokeThickness;
            }
        }
    }
}
