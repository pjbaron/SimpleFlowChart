﻿#pragma warning disable CS8618

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SimpleFlowChart
{
    public class DiamondShape : VisualShape
    {
        private Polygon DiamondPolygon;
        private const double DefaultStrokeThickness = 1;
        private const double HighlightedStrokeThickness = 3;
        private Brush DefaultFill = Brushes.White;
        private Brush HighlightFill = Brushes.LightYellow;

        public DiamondShape(double x, double y, double width = 100, double height = 60, string text = "") : base(x, y, width, height, text)
        {
            InitializeShape();
            InitializeNodes();
        }

        private void InitializeShape()
        {
            DiamondPolygon = new Polygon
            {
                Fill = DefaultFill,
                Stroke = Brushes.Black,
                StrokeThickness = DefaultStrokeThickness,
            };

            DiamondPolygon.Points.Clear();
            DiamondPolygon.Points.Add(new Point(Width / 2, 0));
            DiamondPolygon.Points.Add(new Point(Width, Height / 2));
            DiamondPolygon.Points.Add(new Point(Width / 2, Height));
            DiamondPolygon.Points.Add(new Point(0, Height / 2));

            this.Content = DiamondPolygon;
        }

        private void InitializeNodes()
        {
            // Create nodes on each vertex of the Diamond
            Nodes.Add(new NodeShape(this, new Point(Width / 2, 0))); // Top
            Nodes.Add(new NodeShape(this, new Point(Width, Height / 2))); // Right
            Nodes.Add(new NodeShape(this, new Point(Width / 2, Height))); // Bottom
            Nodes.Add(new NodeShape(this, new Point(0, Height / 2))); // Left
            UpdateNodePositions();
        }

        public override void UpdateNodePositions()
        {
            double left = Canvas.GetLeft(this);
            double top = Canvas.GetTop(this);

            Nodes[0].UpdatePosition(new Point(left + Width / 2, top)); // Top
            Nodes[1].UpdatePosition(new Point(left + Width, top + Height / 2)); // Right
            Nodes[2].UpdatePosition(new Point(left + Width / 2, top + Height)); // Bottom
            Nodes[3].UpdatePosition(new Point(left, top + Height / 2)); // Left
        }

        public override void Highlight(bool isHighlighted)
        {
            if (isHighlighted)
            {
                DiamondPolygon.Fill = HighlightFill;
                DiamondPolygon.StrokeThickness = HighlightedStrokeThickness;
            }
            else
            {
                DiamondPolygon.Fill = DefaultFill;
                DiamondPolygon.StrokeThickness = DefaultStrokeThickness;
            }
        }
    }

}
