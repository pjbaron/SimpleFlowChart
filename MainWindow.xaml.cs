﻿using System.Windows;

namespace SimpleFlowChart
{
    public partial class MainWindow : Window
    {
        FlowchartParser parser;
        string[] Markup = {
            // Complex test
            "RECT R1 (250, 50) (100, 50) \"Start\"",
            "DIAMOND D1 (250, 150) (100, 100) \"Decision 1\"",
            "NODE N1 (100, 150) \"Intermediate Node\"",     // left of d1
            "NODE N2 (100, 225) \"Intermediate Node\"",     // halfway between n1 and top of r2
            "RECT R2 (100, 300) (100, 50) \"Rectangle 1\"",
            "RECT R3 (400, 300) (100, 50) \"Rectangle 2 has a lot of text! It's more than a little and it should overflow, crop, or shrink...\"",
            "DIAMOND D2 (100, 450) (100, 100) \"Decision 2\"",
            "DIAMOND D3 (400, 450) (100, 100) \"Decision 3\"",
            "RECT R4 (100, 600) (100, 50) \"Rectangle 3\"",
            "RECT R5 (400, 600) (100, 50) \"Rectangle 4\"",
            "CONNECT R1.BOTTOM -> D1.TOP",
            "CONNECT D1.LEFT -> N1",
            "CONNECT N1 -> N2",
            "CONNECT N2 -> R2.TOP",
            "CONNECT D1.RIGHT -> R3.TOP",
            "CONNECT R2.BOTTOM -> D2.TOP",
            "CONNECT R3.BOTTOM -> D3.TOP",
            "CONNECT D2.BOTTOM -> R4.TOP",
            "CONNECT D3.BOTTOM -> R5.TOP",
            "CONNECT D2.LEFT -> N2",        // loop back up to N2 node
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
            public const double SnapThreshold = 16.0;
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
        }
    }
}