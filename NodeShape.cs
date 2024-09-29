#pragma warning disable CS8618

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SimpleFlowChart
{
    public class NodeShape : VisualShape
    {
        private System.Windows.Shapes.Ellipse Circle;
        private VisualShape? ParentShape;
        public Point Position { get; set; }
        public List<ShapeConnection> Connections { get; } = new List<ShapeConnection>();

        public NodeShape(VisualShape? parent, Point position, string label = "") : base(position.X, position.Y, 10, 10)
        {
            Debug.WriteLine($"New NodeShape for {parent.Name}: at ({position.X}, {position.Y})");
            ParentShape = parent;
            Position = position;
            if (ParentShape == null)
            {
                InitializeShape();
                InitializeNodes();
            }
        }

        private void InitializeShape()
        {
            Circle = new Ellipse
            {
                Fill = Brushes.Gray,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            this.Content = Circle;
            // stay above Connection lines to avoid them blocking input to us
            Canvas.SetZIndex(this, 1);
        }

        private void InitializeNodes()
        {
            // Add this Node to the Shape list for update calls when dragged
            Nodes.Clear();
            Nodes.Add(this);
            UpdateNodePositions();
        }

        public override void UpdateNodePositions()
        {
            if (ParentShape == null)
            {
                double left = Canvas.GetLeft(this);
                double top = Canvas.GetTop(this);

                Nodes[0].UpdatePosition(new Point(left + Width / 2, top + Height / 2));
            }
        }

        public void UpdatePosition(Point newPosition)
        {
            Position = newPosition;
            foreach (var connection in Connections)
            {
                connection.UpdateLinePosition();
            }
        }
    }
}
