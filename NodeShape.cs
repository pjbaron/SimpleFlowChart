#pragma warning disable CS8618

using System.Diagnostics;
using System.Windows;



namespace SimpleFlowChart
{
    public class Node : Shape
    {
        private Shape? ParentShape;
        public Point Position { get; set; }
        public List<ShapeConnection> Connections { get; } = new List<ShapeConnection>();

        public Node(Shape? parent, Point position, string label = "") : base(position.X, position.Y, 10, 10)
        {
            Debug.WriteLine($"New Node: at ({position.X}, {position.Y})");
            ParentShape = parent;
            Position = position;
        }

        public void UpdatePosition(Point newPosition)
        {
            Debug.WriteLine($"Node: UpdatePositions {newPosition}. Connections {Connections.Count}");
            Position = newPosition;
            foreach (var connection in Connections)
            {
                connection.UpdateLinePosition();
            }
        }
    }
}
