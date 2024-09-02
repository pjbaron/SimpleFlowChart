using System.Windows;
using System.Windows.Controls;

namespace SimpleFlowChart
{
    public partial class RectangleShape : UserControl
    {
        public Shape Shape { get; private set; }

        public RectangleShape(string text, double x, double y)
        {
            InitializeComponent();
            TextBlock.Text = text;
            Shape = new Shape(this, x, y);
            InitializeNodes();
        }

        private void InitializeNodes()
        {
            // Create nodes at the midpoints of each side
            Shape.Nodes.Add(new Node(Shape, new Point(ActualWidth / 2, 0))); // Top
            Shape.Nodes.Add(new Node(Shape, new Point(ActualWidth, ActualHeight / 2))); // Right
            Shape.Nodes.Add(new Node(Shape, new Point(ActualWidth / 2, ActualHeight))); // Bottom
            Shape.Nodes.Add(new Node(Shape, new Point(0, ActualHeight / 2))); // Left
        }

        public void UpdateNodePositions()
        {
            double left = Canvas.GetLeft(this);
            double top = Canvas.GetTop(this);

            Shape.Nodes[0].UpdatePosition(new Point(left + ActualWidth / 2, top)); // Top
            Shape.Nodes[1].UpdatePosition(new Point(left + ActualWidth, top + ActualHeight / 2)); // Right
            Shape.Nodes[2].UpdatePosition(new Point(left + ActualWidth / 2, top + ActualHeight)); // Bottom
            Shape.Nodes[3].UpdatePosition(new Point(left, top + ActualHeight / 2)); // Left
        }
    }
}
