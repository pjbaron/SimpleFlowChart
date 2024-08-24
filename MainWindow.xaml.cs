using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace SimpleFlowChart
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


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ServiceLocator.Register(flowchartCanvas);

            InitializeFlowchart();
        }

        private void InitializeFlowchart()
        {
            // Add rectangles and diamonds to the canvas
            var r1 = new Shape(new RectangleShape("Rect 1"), 100, 50);
            var d1 = new Shape(new DiamondShape("Diamond 1"), 200, 150);
            var r2 = new Shape(new RectangleShape("Rectangle 2"), 350, 300);

            // Connect them together
            r1.AddConnectionOut(d1);
            d1.AddConnectionOut(r2);
        }
    }
}