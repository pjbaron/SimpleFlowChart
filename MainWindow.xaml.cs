using System.Windows;

namespace SimpleFlowChart
{
    public partial class MainWindow : Window
    {
        public static class Constants
        {
            public const double SnapThreshold = 10.0;
        }


        public static class ServiceLocator
        {
            private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();

            public static void Register<T>(T service)
            {
                if (service != null)
                {
                    Services[typeof(T)] = service;
                }
            }

            public static T GetService<T>()
            {
                return (T)Services[typeof(T)];
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            ServiceLocator.Register(flowchartCanvas);

            InitializeFlowchart();
        }

        private void InitializeFlowchart()
        {
            var r1 = new RectangleShape("Rectangle 1", 100, 50);
            var d1 = new DiamondShape("Diamond 1", 200, 150);
            var r2 = new RectangleShape("Rectangle 2", 350, 300);

            // Connect them using nodes
            var c1 = new ShapeConnection(r1.Shape.Nodes[(int)Shape.NodePoints.Bottom], d1.Shape.Nodes[(int)Shape.NodePoints.Top]);
            var c2 = new ShapeConnection(d1.Shape.Nodes[(int)Shape.NodePoints.Right], r2.Shape.Nodes[(int)Shape.NodePoints.Left]);
        }
    }
}