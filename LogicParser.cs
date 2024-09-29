using System;
using System.Diagnostics;
using System.Windows.Shapes;

namespace SimpleFlowChart
{
    // parse the Markup into a ProgramFlow
    public class LogicParser
    {
        public Dictionary<string, LogicShape> shapes = new Dictionary<string, LogicShape>();
        private Dictionary<string, LogicNode> nodes = new Dictionary<string, LogicNode>();
        private Dictionary<string, LogicConnection> connections = new Dictionary<string, LogicConnection>();

        private int LineNumber;
        private string[]? Markup = null;

        public class BranchDirections
        {
            public const string TrueBottom = "BOTTOM";
            public const string TrueLeft = "LEFT";
            public const string TrueRight = "RIGHT";
        }

        private string AnotherDirection(string direction)
        {
            if (direction == "BOTTOM") return "LEFT";
            if (direction == "LEFT") return "RIGHT";
            return "BOTTOM";
        }
        private string LastDirection(string direction)
        {
            if (direction == "BOTTOM") return "RIGHT";
            if (direction == "LEFT") return "BOTTOM";
            return "LEFT";
        }

        public void ParseProgram(string[] lines)
        {
            Markup = lines;

            // 2 passes: shapes first, followed by nodes and connections
            for (LineNumber = 0; LineNumber < Markup.Length; LineNumber++)
            {
                string line = Markup[LineNumber];
                var parts = line.Split(' ');
                switch (parts[0])
                {
                    case "RECT":
                        CreateRectangle(parts, line);
                        break;
                    case "DIAMOND":
                        CreateDiamond(parts, line);
                        break;
                }
            }

            for (LineNumber = 0; LineNumber < Markup.Length; LineNumber++)
            {
                string line = Markup[LineNumber];
                var parts = line.Split(' ');
                switch (parts[0])
                {
                    case "NODE":
                        CreateNode(parts, line);
                        break;
                    case "CONNECT":
                        CreateConnection(parts, line);
                        break;
                }
            }

            // connect LogicShapes together directly, without connections or nodes
            foreach (var shape in shapes)
            {
                TraceConnections(shape);
            }

            // we're done with these
            nodes.Clear();
            connections.Clear();
        }

        private void TraceConnections(KeyValuePair<string, LogicShape> shape)
        {
            var logicShape = shape.Value;
            LogicRectangle? rectShape = logicShape as LogicRectangle;
            if (rectShape != null)
            {
                if (rectShape.Id != "END")
                {
                    rectShape.Destination = FindDestination(rectShape);
                    if (rectShape.Destination == null)
                        throw new Exception($"RECT {rectShape.Id} has no onward CONNECT");
                }
            }
            else
            {
                LogicDiamond? diamondShape = logicShape as LogicDiamond;
                if (diamondShape != null)
                {
                    diamondShape.NextIfTrue = FindDestination(diamondShape, true, diamondShape.TrueBranch);
                    diamondShape.NextIfFalse = FindDestination(diamondShape, false, diamondShape.TrueBranch);
                }
            }
        }

        private LogicShape? FindDestination(LogicRectangle rect)
        {
            if (rect.Id == null)
                throw new Exception($"RECT has null Id");

            // trace the connections and nodes until we reach a LogicShape
            var connection = FindConnection(rect.Id);
            while (connection != null)
            {
                if (connection.FromId == null)
                    throw new Exception($"CONNECT does not have a source {connection}");
                if (connection.ToId == null)
                    throw new Exception($"CONNECT does not have a destination {connection}.");

                LogicShape? shape;
                if (shapes.TryGetValue(connection.ToId, out shape))
                {
                        // we found a LogicShape, all done!
                        return shape;
                }

                LogicNode? node = null;
                if (nodes.TryGetValue(connection.ToId, out node))
                {
                    // the connection ends in a node, find the next connection onwards
                    if (node == null)
                        throw new Exception($"NODE is null");
                    if (node.Id == null)
                        throw new Exception($"NODE does not have an ID");

                    connection = FindConnection(node.Id);
                    if (connection == null)
                        throw new Exception($"NODE {node.Id} has no onward CONNECT");

                    continue;
                }

                if (FindConnection(connection.FromId) != null)
                {
                    throw new Exception($"CONNECT is connected directly to CONNECT {connection.FromId}");
                }

                throw new Exception($"Cannot find destination for RECT {rect.Id} output");
            }

            return null;
        }

        private LogicShape? FindDestination(LogicDiamond diamond, bool trueBranch, string trueDirection)
        {
            if (diamond.Id == null)
                throw new Exception($"DIAMOND has null Id");

            // trace the connections and nodes until we reach a LogicShape
            string searchForId = diamond.Id;
            LogicConnection? connection = null;
            if (trueBranch)
            {
                searchForId = diamond.Id + "." + trueDirection;
                connection = FindConnection(searchForId);
            }
            else
            {
                searchForId = diamond.Id + "." + AnotherDirection(trueDirection);
                connection = FindConnection(searchForId);
                if (connection == null)
                {
                    searchForId = diamond.Id + "." + LastDirection(trueDirection);
                    connection = FindConnection(searchForId);
                }
            }
            while (connection != null)
            {
                if (connection.FromId == null)
                    throw new Exception($"CONNECT does not have a source {connection}");
                if (connection.ToId == null)
                    throw new Exception($"CONNECT does not have a destination {connection}.");

                LogicShape? shape;
                if (shapes.TryGetValue(connection.ToId, out shape))
                {
                    // we found a LogicShape, all done!
                    return shape;
                }

                LogicNode? node = null;
                if (nodes.TryGetValue(connection.ToId, out node))
                {
                    // the connection ends in a node, find the next connection onwards
                    if (node == null)
                        throw new Exception($"NODE is null");
                    if (node.Id == null)
                        throw new Exception($"NODE does not have an ID");

                    connection = FindConnection(node.Id);
                    if (connection == null)
                        throw new Exception($"NODE {node.Id} has no onward CONNECT");

                    continue;
                }

                if (FindConnection(connection.FromId) != null)
                {
                    throw new Exception($"CONNECT is connected directly to CONNECT {connection.FromId}");
                }

                throw new Exception($"Cannot find destination for RECT {diamond.Id} output");
            }

            return null;
        }

        private LogicConnection? FindConnection(string Id)
        {
            foreach(var key in connections.Keys)
            {
                if (key == Id || (key.StartsWith(Id) && key.Contains('.')))
                {
                    Debug.WriteLine($"FindConnection {Id} to {connections[key].ToId}");
                    return connections[key];
                }
            }
            Debug.WriteLine($"FindConnection {Id} FAILED");
            return null;
        }

        private void CreateRectangle(string[] parts, string line)
        {
            var id = parts[1];
            var rect = new LogicRectangle
            {
                Id = id,
                DataString = ExtractDataString(line),
                Destination = null,
                LineNumber = LineNumber,
            };
            ProgramFlow.AddCommand(rect);
            shapes[id] = rect;
        }

        private void CreateDiamond(string[] parts, string line)
        {
            var id = parts[1];
            var diam = new LogicDiamond
            {
                Id = id,
                DataString = ExtractDataString(line),
                TrueBranch = line switch
                {
                    string s when s.Contains("TRUEBOTTOM") => BranchDirections.TrueBottom,
                    string s when s.Contains("TRUELEFT") => BranchDirections.TrueLeft,
                    string s when s.Contains("TRUERIGHT") => BranchDirections.TrueRight,
                    _ => throw new Exception($"Invalid Branch Direction for Diamond {id}")
                },
                NextIfTrue = null,
                NextIfFalse = null,
                //Condition = /* set to Func with bool return */
                LineNumber = LineNumber,
            };
            ProgramFlow.AddCommand(diam);
            shapes[id] = diam;
        }

        private void CreateNode(string[] parts, string line)
        {
            if (parts.Length < 2)
                throw new Exception($"Node is not fully specified {line}");

            // these nodes are intermediate positions on connections
            var node = new LogicNode
            {
                Id = parts[1],
                LineNumber = LineNumber,
            };

            nodes.Add(parts[1], node);
        }

        private void CreateConnection(string[] parts, string line)
        {
            if (parts.Length < 4)
                throw new Exception($"Connection is not fully specified {line}");

            if (parts[2] != "->")
                throw new Exception($"Connection Format Error: cannot find ->\n{line}");

            // "CONNECT N1 -> D1.TOP"
            // "CONNECT D1.LEFT -> N1"
            string FromId = parts[1].Split('.')[0];
            string ToId = parts[3].Split('.')[0];

            //Debug.WriteLine($"CreateConnection {FromId} {ToId}");

            var connection = new LogicConnection
            {
                FromId = FromId,
                ToId = ToId,
                LineNumber = LineNumber,
            };

            // the Key is the whole 'from' value e.g. D1.LEFT
            connections.Add(parts[1], connection);
        }

        private string ExtractDataString(string line)
        {
            // Find and extract the text string from the line
            var quoteIndexStart = line.IndexOf('"');
            var quoteIndexEnd = line.LastIndexOf('"');
            return line.Substring(quoteIndexStart + 1, quoteIndexEnd - quoteIndexStart - 1);
        }

        private bool EvaluateCondition(string id)
        {
            // TODO: evaluate a decision
            Debug.WriteLine($"Evaluating condition for {id}");
            return true;
        }
    }



    // support classes for ProgramFlow which hold the executable and connection data for each node in the original graph
    public abstract class LogicShape
    {
        public string? Id { get; set; }
        public string? DataString { get; set; }
        public int LineNumber { get; set; }
        public string? Response { get; set; }
        protected static string RectanglePreface = "Answer this question completely, but as concisely as possible. Do not include additional text except for the answer to the question (e.g. 'Certainly ...')";
        protected static string DiamondPreface = "Answer this question with 'yes', 'no' or 'unknown' only. Do not include additional text except for the one word answer to the question.";

        public abstract LogicShape? Execute(LlmClient llmClient, string? PreviousResponse);
    }

    public class LogicRectangle : LogicShape
    {
        public LogicShape? Destination { get; set; }

        public override LogicShape? Execute(LlmClient llmClient, string? previousResponse)
        {
            if (DataString == null) return null;

            Response = "";
            if (DataString.ToUpper() == "START") return Destination;

            // Perform some action here (e.g., log, update state)
            string query = $"{RectanglePreface}\n{DataString}";
            Debug.WriteLine($"Executing Action: {query}");
            Response = llmClient.PerformQuery(query).Result;

            if (Id != "END" && Destination == null)
            {
                throw new Exception($"Rectangle {Id} has no link to the next command.");
            }

            // Return the next shape in the flow
            return Destination;
        }
    }

    public class LogicDiamond : LogicShape
    {
        public LogicShape NextIfTrue { get; set; } = null!;
        public LogicShape NextIfFalse { get; set; } = null!;
        public Func<bool> Condition { get; set; } = null!;
        public string TrueBranch { get; set; } = string.Empty;

        public override LogicShape? Execute(LlmClient llmClient, string? previousResponse)
        {
            if (DataString == null) return null;

            string query = $"{DiamondPreface}\n{DataString}";
            Debug.WriteLine($"Evaluating Decision: {query}");
            Response = llmClient.PerformQuery(query).Result;

            //if (Condition == null)
            //{
            //    throw new Exception($"There's no Condition Function for Diamond {Id}");
            //}
            //bool result = Condition.Invoke();
            bool result = Response.ToLower() == "yes";
            Debug.WriteLine($"Decision {Id} result: {result}");

            return result ? NextIfTrue : NextIfFalse;
        }
    }

    public class LogicConnection
    {
        public string FromId { get; set; } = null!;
        public string ToId { get; set; } = null!;
        public int LineNumber;
    }

    public class LogicNode
    {
        public string? Id { get; set; }
        public int LineNumber;
    }

}