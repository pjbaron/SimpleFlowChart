using System.Windows;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace SimpleFlowChart
{
    class VisualParser
    {
        private Dictionary<string, VisualShape> shapes = new Dictionary<string, VisualShape>();
        private static Regex shapeRegex = new Regex(@"(\w+)\s+(\w+)\s+\(([^)]+)\)\s+(\(([^)]+)\)\s*)?(\"".*\"")?");
        private int LineNumber;
        private string[]? Markup = null;


        public void ParseMarkup(string[] lines)
        {
            Markup = lines;

            // 2-passes, always collect shapes before connections
            for (LineNumber = 0; LineNumber < Markup.Length; LineNumber++)
            {
                string line = Markup[LineNumber];
                if (line.StartsWith("RECT") || line.StartsWith("DIAMOND") || line.StartsWith("NODE"))
                {
                    CreateShape(line);
                }
            }
                
            for (LineNumber = 0; LineNumber < Markup.Length; LineNumber++)
            {
                string line = Markup[LineNumber];
                if (line.StartsWith("CONNECT"))
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

            VisualShape? shape = null;
            switch (shapeType)
            {
                case "RECT":
                {
                    var sizeParts = size.Split(',');
                    double width = double.Parse(sizeParts[0]);
                    double height = double.Parse(sizeParts[1]);
                    shape = new RectangleShape(x, y, width, height, label);
                    shape.LineNumber = LineNumber;
                }
                break;

                case "DIAMOND":
                {
                    var sizeParts = size.Split(',');
                    double width = double.Parse(sizeParts[0]);
                    double height = double.Parse(sizeParts[1]);
                    shape = new DiamondShape(x, y, width, height, label);
                    shape.LineNumber = LineNumber;
                }
                break;

                case "NODE":
                {
                    shape = new NodeShape(null, new Point(x, y), label);
                    shape.LineNumber = LineNumber;
                }
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

            if (shapes.TryGetValue(sourceShapeId, out VisualShape? valueSrc) && shapes.TryGetValue(targetShapeId, out VisualShape? valueTgt))
            {
                Debug.WriteLine($"CreateConnection (Visual) {sourceShapeId} to {targetShapeId}");
                VisualShape sourceShape = valueSrc;
                VisualShape targetShape = valueTgt;
                // adds itself to the Nodes
                _ = new ShapeConnection(GetNode(sourceShape, sourceAnchor), GetNode(targetShape, targetAnchor));
            }
            else
            {
                Debug.WriteLine($"Cannot CreateConnection (Visual) {sourceShapeId} to {targetShapeId}");
            }
        }

        private NodeShape GetNode(VisualShape shape, string anchor)
        {
            if (shape is NodeShape node)
            {
                return node;
            }

            return anchor.ToUpper() switch
            {
                "TOP" => shape.Nodes[(int)VisualShape.NodePoints.Top],
                "BOTTOM" => shape.Nodes[(int)VisualShape.NodePoints.Bottom],
                "LEFT" => shape.Nodes[(int)VisualShape.NodePoints.Left],
                "RIGHT" => shape.Nodes[(int)VisualShape.NodePoints.Right],
                _ => throw new ArgumentException("Invalid node anchor"),
            };
        }

        public void FindAndHighlight(int lineNumber)
        {
            foreach(var kvPair in shapes)
            {
                VisualShape shape = kvPair.Value;
                shape.Highlight(shape.LineNumber == lineNumber);
            }
        }
    }
}
