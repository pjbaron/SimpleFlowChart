using System;
using System.Windows;

namespace SimpleFlowChart
{
    public partial class MainWindow : Window
    {
        FlowchartParser parser;
        string[] Markup = {
            // more complex test
            "RECT R1 (250, 50) (100, 50) \"Start\"",
            "DIAMOND D1 (250, 150) (100, 100) \"Decision 1\"",
            "NODE N1 (100, 150) \"Intermediate Node\"",
            "NODE N2 (100, 225) \"Intermediate Node\"",
            "RECT R2 (100, 300) (100, 50) \"Rectangle 1\"",
            "RECT R3 (400, 300) (100, 50) \"Rectangle 2\"",
            "DIAMOND D2 (100, 450) (100, 100) \"Decision 2\"",
            "DIAMOND D3 (400, 450) (100, 100) \"Decision 3\"",
            "RECT R4 (100, 600) (100, 50) \"Rectangle 3\"",
            "RECT R5 (400, 600) (100, 50) \"Rectangle 4\"",
            "CONNECT R1.BOTTOM -> D1.TOP",
            "CONNECT D1.LEFT -> N1",
            "CONNECT N1 -> R2.TOP",
            "CONNECT D1.RIGHT -> R3.TOP",
            "CONNECT R2.BOTTOM -> D2.TOP",
            "CONNECT R3.BOTTOM -> D3.TOP",
            "CONNECT D2.BOTTOM -> R4.TOP",
            "CONNECT D3.BOTTOM -> R5.TOP",
            "CONNECT D2.LEFT -> N2",
            "CONNECT D3.LEFT -> R4.TOP"
        };
            // original simple test
            //"RECT R1 (200, 50) (100, 50) \"Start\"",
            //"DIAMOND D1 (200, 150) (100, 50) \"Decision\"",
            //"RECT R2 (350, 150) (100, 50) \"Right Node\"",
            //"RECT R3 (200, 300) (100, 50) \"Bottom Node\"",
            //"CONNECT R1.BOTTOM -> D1.TOP",
            //"CONNECT D1.RIGHT -> R2.LEFT",
            //"CONNECT D1.BOTTOM -> R3.TOP"

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

            parser = new FlowchartParser();
            ServiceLocator.Register(flowchartCanvas);
            ServiceLocator.Register(parser);

            InitializeFlowchart(Markup);
        }

        private void InitializeFlowchart(string[] markup)
        {
            parser.ParseMarkup(markup);

            //var r1 = new RectangleShape(200.0, 50.0);
            //var d1 = new DiamondShape(200, 150);
            //var r2 = new RectangleShape(350, 150);
            //var r3 = new RectangleShape(200, 300);

            //// Connect the nodes with lines
            //var c1 = new ShapeConnection(r1.Nodes[(int)Shape.NodePoints.Bottom], d1.Nodes[(int)Shape.NodePoints.Top]);
            //var c2 = new ShapeConnection(d1.Nodes[(int)Shape.NodePoints.Right], r2.Nodes[(int)Shape.NodePoints.Left]);
            //var c3 = new ShapeConnection(d1.Nodes[(int)Shape.NodePoints.Bottom], r3.Nodes[(int)Shape.NodePoints.Top]);
        }
    }
}