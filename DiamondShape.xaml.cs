using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace SimpleFlowChart
{
    public partial class DiamondShape : UserControl
    {
        public Shape Shape { get; private set; }
        private Polygon DiamondPolygon;

        public DiamondShape(string text, double x, double y)
        {
            InitializeComponent();
            TextBlock.Text = text;
            Shape = new Shape(this, x, y);
            InitializeDiamond();
            InitializeNodes();
        }

        private void InitializeDiamond()
        {
            DiamondPolygon = new Polygon
            {
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            UpdateDiamondGeometry();
        }

        private void UpdateDiamondGeometry()
        {
            DiamondPolygon.Points.Clear();
            DiamondPolygon.Points.Add(new Point(ActualWidth / 2, 0));
            DiamondPolygon.Points.Add(new Point(ActualWidth, ActualHeight / 2));
            DiamondPolygon.Points.Add(new Point(ActualWidth / 2, ActualHeight));
            DiamondPolygon.Points.Add(new Point(0, ActualHeight / 2));
        }

        private void InitializeNodes()
        {
            // Create nodes at the midpoints of each side of the diamond
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

            UpdateDiamondGeometry();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateDiamondGeometry();
            UpdateNodePositions();
        }
    }
}
