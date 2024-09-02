using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SimpleFlowChart
{
    public class ShapeConnection
    {
        public Node StartNode { get; }
        public Node EndNode { get; }
        public Line Line { get; private set; }
        private readonly Canvas Canvas;

        public ShapeConnection(Node startNode, Node endNode)
        {
            StartNode = startNode;
            EndNode = endNode;
            Canvas = MainWindow.ServiceLocator.GetService<Canvas>();

            CreateLine();
            StartNode.Connections.Add(this);
            EndNode.Connections.Add(this);
        }

        private void CreateLine()
        {
            Line = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            UpdatePosition();
            Canvas.Children.Add(Line);
        }

        public void UpdatePosition()
        {
            Line.X1 = StartNode.Position.X;
            Line.Y1 = StartNode.Position.Y;
            Line.X2 = EndNode.Position.X;
            Line.Y2 = EndNode.Position.Y;
        }
    }
}
