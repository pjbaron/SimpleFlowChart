using System.Collections.ObjectModel;
using System.Windows;


namespace SimpleFlowChart
{
    /// <summary>
    /// Interaction logic for NodeShape.xaml
    /// </summary>
    public partial class Node
    {
        public Point Position { get; set; }
        public Shape ParentShape { get; set; }
        public ObservableCollection<ShapeConnection> Connections { get; } = new ObservableCollection<ShapeConnection>();

        public Node(Shape parentShape, Point position)
        {
            ParentShape = parentShape;
            Position = position;
        }

        public void UpdatePosition(Point newPosition)
        {
            Position = newPosition;
            foreach (var connection in Connections)
            {
                connection.UpdatePosition();
            }
        }
    }
}
