namespace SimpleFlowChart
{
    // parse the Markup into a ProgramFlow
    public class MarkupToProgramFlow
    {
        public Dictionary<string, LogicShape> shapes = new Dictionary<string, LogicShape>();

        public void InitializeProgram(string[] lines)
        {
            foreach (var line in lines)
            {
                var parts = line.Split(' ');

                if (parts[0] == "START")
                {
                    var id = parts[1];
                    var label = ExtractLabel(line);
                    var rect = new LogicRectangle { Id = id, Label = label };
                    ProgramFlow.AddShape(rect);
                    shapes["START"] = rect;
                }
                else if (parts[0] == "RECT")
                {
                    var id = parts[1];
                    var label = ExtractLabel(line);
                    var rect = new LogicRectangle { Id = id, Label = label };
                    ProgramFlow.AddShape(rect);
                    shapes[id] = rect;
                }
                else if (parts[0] == "DIAMOND")
                {
                    var id = parts[1];
                    var label = ExtractLabel(line);
                    var diam = new LogicDiamond
                    {
                        Id = id,
                        Label = label,
                        Condition = () => EvaluateCondition(id)
                    };
                    ProgramFlow.AddShape(diam);
                    shapes[id] = diam;
                }
                else if (parts[0] == "CONNECT")
                {
                    // TODO: don't encode connectors, trace them to their destination and link that directly from the LogicShape

                    //var fromId = parts[1].Split('.')[0];
                    //var toId = parts[3].Split('.')[0];
                    //var condition = parts[1].Contains("LEFT") || parts[1].Contains("RIGHT") ? false : (bool?)null;
                    //if (parts[1].Contains("BOTTOM")) condition = true;

                    //var conn = new LogicConnection { FromId = fromId, ToId = toId, Condition = condition }
                    //ProgramFlow.AddConnection(conn);
                    //shapes.Append(conn);
                }
            }
        }

        private string ExtractLabel(string line)
        {
            // Find and extract the label from the line
            var quoteIndexStart = line.IndexOf('"');
            var quoteIndexEnd = line.LastIndexOf('"');
            return line.Substring(quoteIndexStart + 1, quoteIndexEnd - quoteIndexStart - 1);
        }

        private bool EvaluateCondition(string id)
        {
            // Implement the logic for evaluating a decision, possibly user input or data-driven
            Console.WriteLine($"Evaluating condition for {id}. Please enter true/false:");
            return bool.Parse(Console.ReadLine());
        }
    }

    // hold the parsed markup in a data structure and run it
    public static class ProgramFlow
    {
        private static Dictionary<string, LogicShape> shapes = new Dictionary<string, LogicShape>();
        private static List<LogicConnection> connections = new List<LogicConnection>();

        public static void AddShape(LogicShape shape)
        {
            shapes[shape.Id] = shape;
        }

        public static void AddConnection(LogicConnection connection)
        {
            connections.Add(connection);
        }

        // Get the next shape in the flow
        public static LogicShape GetNextShape(string currentShapeId, bool? condition = null)
        {
            // Find the connection from the current shape
            var nextConnection = connections.FirstOrDefault(c =>
                c.FromId == currentShapeId && (condition == null || c.Condition == condition));

            if (nextConnection == null)
            {
                Console.WriteLine("No further connections. Program ends.");
                return null; // End of the program
            }

            return shapes[nextConnection.ToId];
        }

        public static void Run(string startShapeId)
        {
            // Set the program pointer to the start shape
            LogicShape currentShape = shapes[startShapeId];

            // Iterate through shapes until there is no next shape (program ends)
            while (currentShape != null)
            {
                currentShape = currentShape.Execute();
            }
        }
    }



    // support classes for ProgramFlow which hold the executable and connection data for each node in the original graph
    public abstract class LogicShape
    {
        public string Id { get; set; }
        public string Label { get; set; }

        public abstract LogicShape Execute(); // Execute action or decision
    }

    public class LogicRectangle : LogicShape
    {
        public override LogicShape Execute()
        {
            // Perform some action here (e.g., log, update state)
            Console.WriteLine($"Executing Action: {Label}");

            // Return the next shape in the flow
            return ProgramFlow.GetNextShape(Id);
        }
    }

    public class LogicDiamond : LogicShape
    {
        public Func<bool> Condition { get; set; }  // A function that evaluates true or false

        public override LogicShape Execute()
        {
            Console.WriteLine($"Evaluating Decision: {Label}");

            // Evaluate the condition
            bool result = Condition.Invoke();

            Console.WriteLine($"Decision result: {result}");

            // Return the appropriate next shape based on true/false
            return ProgramFlow.GetNextShape(Id, result);
        }
    }

    public class LogicConnection
    {
        public string FromId { get; set; }
        public string ToId { get; set; }
        public bool? Condition { get; set; } // True for 'true' connection, False for 'false', Null for normal connections
    }


}