using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Text.RegularExpressions;

namespace SimpleFlowChart
{
    class FlowchartParser
    {
        private Dictionary<string, Shape> shapes = new Dictionary<string, Shape>();
        private static Regex shapeRegex = new Regex(@"(\w+)\s+(\w+)\s+\(([^)]+)\)\s+(\(([^)]+)\)\s*)?(\"".*\"")?");

        public void ParseMarkup(string[] lines)
        {
            foreach (string line in lines)
            {
                if (line.StartsWith("RECT") || line.StartsWith("DIAMOND") || line.StartsWith("NODE"))
                {
                    CreateShape(line);
                }
                else if (line.StartsWith("CONNECT"))
                {
                    CreateConnection(line);
                }
            }
        }

        private void CreateShape(string line)
        {
            var match = shapeRegex.Match(line);
            if (!match.Success) return;

            string shapeType = match.Groups[1].Value;
            string id = match.Groups[2].Value;
            string position = match.Groups[3].Value;
            string size = match.Groups[5].Value;
            string label = match.Groups[6].Success ? match.Groups[6].Value.Trim('"') : "";

            var posParts = position.Split(',');
            double x = double.Parse(posParts[0]);
            double y = double.Parse(posParts[1]);

            Shape shape = null;
            switch (shapeType)
            {
                case "RECT":
                case "DIAMOND":
                    var sizeParts = size.Split(',');
                    double width = double.Parse(sizeParts[0]);
                    double height = double.Parse(sizeParts[1]);
                    shape = shapeType == "RECT"
                        ? new RectangleShape(x, y, width, height, label)
                        : new DiamondShape(x, y, width, height, label) as Shape;
                    break;
                case "NODE":
                    shape = new Node(null, new Point(x, y), label);
                    break;
            }

            if (shape != null)
            {
                shapes[id] = shape;
            }
        }

        private void CreateConnection(string line)
        {
            string[] parts = line.Replace("->", "").Split(' ');
            string sourceNode = parts[1];
            string targetNode = parts[3];
            string[] sourceParts = sourceNode.Split('.');
            string[] targetParts = targetNode.Split('.');
            string sourceShapeId = sourceParts[0];
            string sourceAnchor = sourceParts.Length > 1 ? sourceParts[1] : "CENTER";
            string targetShapeId = targetParts[0];
            string targetAnchor = targetParts.Length > 1 ? targetParts[1] : "CENTER";

            if (shapes.ContainsKey(sourceShapeId) && shapes.ContainsKey(targetShapeId))
            {
                Shape sourceShape = shapes[sourceShapeId];
                Shape targetShape = shapes[targetShapeId];
                var connection = new ShapeConnection(GetNode(sourceShape, sourceAnchor), GetNode(targetShape, targetAnchor));
            }
        }

        private Node GetNode(Shape shape, string anchor)
        {
            if (shape is Node node)
            {
                return node;
            }

            return anchor.ToUpper() switch
            {
                "TOP" => shape.Nodes[(int)Shape.NodePoints.Top],
                "BOTTOM" => shape.Nodes[(int)Shape.NodePoints.Bottom],
                "LEFT" => shape.Nodes[(int)Shape.NodePoints.Left],
                "RIGHT" => shape.Nodes[(int)Shape.NodePoints.Right],
                _ => throw new ArgumentException("Invalid node anchor"),
            };
        }
    }
}

/*
using SimpleFlowChart;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SimpleFlowChart
{


    class FlowchartParser
    {
        // Dictionary to store shapes with their IDs
        private Dictionary<string, Shape> shapes = new Dictionary<string, Shape>();

        // Regex to match the shape line structure
        private static Regex shapeRegex = new Regex(@"(\w+)\s+(\w+)\s+\(([^)]+)\)\s+\(([^)]+)\)\s*(\"".*\"")?");

        // Parse the markup and generate shapes and connections
        public void ParseMarkup(string[] lines)
        {
            foreach (string line in lines)
            {
                if (line.StartsWith("RECT") || line.StartsWith("DIAMOND"))
                {
                    CreateShape(line);
                }
                else if (line.StartsWith("CONNECT"))
                {
                    CreateConnection(line);
                }
            }
        }

        // Create a shape based on the markup line
        private void CreateShape(string line)
        {
            // Use regular expressions to parse the line
            var match = shapeRegex.Match(line);
            if (!match.Success) return;

            string shapeType = match.Groups[1].Value;
            string id = match.Groups[2].Value;
            string position = match.Groups[3].Value;
            string size = match.Groups[4].Value;
            string label = match.Groups[5].Success ? match.Groups[5].Value.Trim('"') : "";

            var posParts = position.Split(',');
            var sizeParts = size.Split(',');

            double x = double.Parse(posParts[0]);
            double y = double.Parse(posParts[1]);
            double width = double.Parse(sizeParts[0]);
            double height = double.Parse(sizeParts[1]);

            Shape shape = null;

            switch(shapeType)
            {
                case "RECT":
                    shape = new RectangleShape(x, y, width, height, label);
                    break;
                case "DIAMOND":
                    shape = new DiamondShape(x, y, width, height, label);
                    break;
            }

            if (shape != null)
            {
                shapes[id] = shape;
            }
        }

        // Create a connection based on the markup line
        private void CreateConnection(string line)
        {
            string[] parts = line.Replace("->", "").Split(' ');

            string sourceNode = parts[1];
            string targetNode = parts[3];

            string[] sourceParts = sourceNode.Split('.');
            string[] targetParts = targetNode.Split('.');

            string sourceShapeId = sourceParts[0];
            string sourceAnchor = sourceParts[1];

            string targetShapeId = targetParts[0];
            string targetAnchor = targetParts[1];

            if (shapes.ContainsKey(sourceShapeId) && shapes.ContainsKey(targetShapeId))
            {
                Shape sourceShape = shapes[sourceShapeId];
                Shape targetShape = shapes[targetShapeId];

                // Connect nodes of the shapes
                var connection = new ShapeConnection(GetNode(sourceShape, sourceAnchor), GetNode(targetShape, targetAnchor));
            }
        }

        // Get the appropriate node from a shape based on the anchor (e.g., TOP, BOTTOM, LEFT, RIGHT)
        private Node GetNode(Shape shape, string anchor)
        {
            switch (anchor.ToUpper())
            {
                case "TOP": return shape.Nodes[(int)Shape.NodePoints.Top];
                case "BOTTOM": return shape.Nodes[(int)Shape.NodePoints.Bottom];
                case "LEFT": return shape.Nodes[(int)Shape.NodePoints.Left];
                case "RIGHT": return shape.Nodes[(int)Shape.NodePoints.Right];
                default: throw new ArgumentException("Invalid node anchor");
            }
        }
    }

}
*/