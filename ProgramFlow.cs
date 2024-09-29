using System.Diagnostics;


namespace SimpleFlowChart
{

    // hold the parsed markup in a simple data structure and execute it
    public static class ProgramFlow
    {
        private static Dictionary<string, LogicShape> shapes = new Dictionary<string, LogicShape>();
        private static LogicShape? StartCommand = null;
        private static LogicShape? CurrentCommand = null;
        private static VisualParser? Parser = null;
        private static LlmClient LlmClient;
        private static string? PreviousResponse = null;


        public static void AddCommand(LogicShape commandShape)
        {
            if (commandShape == null) throw new ArgumentNullException(nameof(commandShape));
            if (string.IsNullOrEmpty(commandShape.Id)) throw new ArgumentException("commandShape.Id cannot be null or empty", nameof(commandShape));

            shapes[commandShape.Id] = commandShape;
            if (commandShape.Id == "START")
                StartCommand = commandShape;
        }

        public static void ResetProgram()
        {
            Parser = MainWindow.ServiceLocator.GetService<VisualParser>()
                ?? throw new InvalidOperationException("VisualParser could not be retrieved from ServiceLocator.");

            LlmClient = MainWindow.ServiceLocator.GetService<LlmClient>()
                ?? throw new InvalidOperationException("LlmClient could not be retrieved from ServiceLocator.");

            if (StartCommand != null)
            {
                CurrentCommand = StartCommand;
                return;
            }

            if (shapes.Count == 0)
                throw new InvalidOperationException("Program is empty or not parsed yet.");
            else
                throw new InvalidOperationException("Program START node was not found.");
        }

        public static void StepProgram()
        {
            if (CurrentCommand == null)
                throw new InvalidOperationException("CurrentCommand is null. Ensure ResetProgram() is called before stepping.");

            Debug.WriteLine($"Program: {CurrentCommand.Id} {CurrentCommand.GetType().Name}");
            Parser!.FindAndHighlight(CurrentCommand.LineNumber);
            LogicShape? next = CurrentCommand.Execute(LlmClient, PreviousResponse);
            PreviousResponse = CurrentCommand.Response;
            CurrentCommand = next;
        }

        public static bool Finished() => CurrentCommand == null;
    }

}
