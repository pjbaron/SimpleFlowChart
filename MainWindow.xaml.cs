using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;


namespace SimpleFlowChart
{
    public partial class MainWindow : Window
    {
        string[] Markup =
        {
            "RECT START (250, 50) (100, 50) \"Start\"",
            "CONNECT START.BOTTOM -> D1.TOP",
            "DIAMOND D1 (250, 250) (100, 100) TRUERIGHT \"Is it raining in Auckland right now?\"",
            "CONNECT D1.RIGHT -> R1.TOP",
            "CONNECT D1.BOTTOM -> R2.TOP",
            "RECT R1 (400, 450) (100, 50) \"Suggest wet weather activities.\"",
            "RECT R2 (250, 450) (100, 50) \"Suggest dry weather activities.\"",
            "CONNECT R1.BOTTOM -> END.TOP",
            "CONNECT R2.BOTTOM -> END.TOP",
            "RECT END (250, 655) (100, 50) \"End\"",

            //"RECT START (250, 50) (100, 50) \"Start\"",
            //"RECT R1 (250, 150) (100, 50) \"Rectangle 0\"",
            //"DIAMOND D1 (250, 250) (100, 100) TRUERIGHT \"Decision 1\"",
            //"NODE N1 (100, 250) \"Intermediate Node\"",     // left of d1
            //"NODE N2 (100, 325) \"Intermediate Node\"",     // halfway between n1 and top of r2
            //"NODE N3 (50, 325) \"Intermediate Node\"",      // bend in loop back connection
            //"RECT R2 (100, 400) (100, 50) \"Rectangle 1\"",
            //"RECT R3 (400, 400) (100, 50) \"Rectangle 2 has a lot of text! It's more than a little and it should overflow, crop, or shrink...\"",
            //"DIAMOND D2 (100, 550) (100, 100) TRUEBOTTOM \"Decision 2\"",
            //"DIAMOND D3 (400, 550) (100, 100) TRUEBOTTOM \"Decision 3\"",
            //"RECT R4 (100, 700) (100, 50) \"Rectangle 3\"",
            //"RECT R5 (400, 700) (100, 50) \"Rectangle 4\"",
            //"CONNECT START.BOTTOM -> D1.TOP",
            //"CONNECT D1.LEFT -> N1",
            //"CONNECT N1 -> N2",
            //"CONNECT N2 -> R2.TOP",
            //"CONNECT D1.RIGHT -> R3.TOP",
            //"CONNECT R2.BOTTOM -> D2.TOP",
            //"CONNECT R3.BOTTOM -> D3.TOP",
            //"CONNECT D2.BOTTOM -> R4.TOP",
            //"CONNECT D3.BOTTOM -> R5.TOP",
            //"CONNECT D2.LEFT -> N3",        // loop back up to N2 node via N3 bend
            //"CONNECT N3 -> N2",
            //"CONNECT D3.LEFT -> R4.TOP",
            //"NODE N4 (250, 780) \"Intermediate Node\"",
            //"CONNECT R4.BOTTOM -> N4",
            //"CONNECT R5.BOTTOM -> N4",
            //"CONNECT N4 -> END.TOP",
            //"RECT END (250, 800) (100, 50) \"End\"",
        };

        private LlmClient llmClient;
        private LogicParser Logic;
        private VisualParser Visual;
        private const double TickInterval = 1.0;
        private DispatcherTimer ProgramTimer;
        private string LastResponse = "";


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
            ServiceLocator.Register(flowchartCanvas);

            Initialize();
        }

        private void Initialize()
        {
            Debug.WriteLine("MainWindow InitializeAsync");

            LlmClient llmClient = new LlmClient();
            ServiceLocator.Register(llmClient);

            Visual = new VisualParser();
            ServiceLocator.Register(Visual);

            Logic = new LogicParser();
            ServiceLocator.Register(Logic);

            Visual.ParseMarkup(Markup);
            Logic.ParseProgram(Markup);
        }


        private void RunMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Run menu item clicked");

            ProgramFlow.ResetProgram();
            ProgramTimer = new DispatcherTimer();
            ProgramTimer.Interval = TimeSpan.FromSeconds(TickInterval);
            ProgramTimer.Tick += ProgramTick;

            ProgramTimer.Start();
        }

        private void ProgramTick(object? sender, EventArgs e)
        {
            Debug.WriteLine("Tick");
            ProgramFlow.StepProgram();
            if (ProgramFlow.Finished())
            {
                ProgramTimer.Stop();
                Debug.WriteLine($"This Program has reached the END");
            }
        }
    }
}
