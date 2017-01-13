using UnityEditor;

namespace Ts.Unity.Debuggers
{
    public class Log : Logging.StaticLogger<Log>
    {
        private Logging.Logger _log = Logging.GetLogger("debugger");
        protected override Logging.Logger log
        {
            get {
                return _log;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Generator debuggers
    ////////////////////////////////////////////////////////////////////////////////////////////////

    public class CircleDebugger : GeneratorDebugger<CircleDebugger>
    {
        public CircleDebugger()
            : base("circle")
        {
        }

        [MenuItem("TerrainScript/Debug/Generators/Circle")]
        public static void ShowWindow()
        {
            Display();
        }
    }

    public class DomeDebugger : GeneratorDebugger<DomeDebugger>
    {
        public DomeDebugger()
            : base("dome")
        {
        }

        [MenuItem("TerrainScript/Debug/Generators/Dome")]
        public static void ShowWindow()
        {
            Display();
        }
    }

    public class GradientDebugger : GeneratorDebugger<GradientDebugger>
    {
        public GradientDebugger()
            : base("gradient")
        {
        }

        [MenuItem("TerrainScript/Debug/Generators/Gradient")]
        public static void ShowWindow()
        {
            Display();
        }
    }

    public class PerlinDebugger : GeneratorDebugger<PerlinDebugger>
    {
        public PerlinDebugger()
            : base("perlin")
        {
        }

        [MenuItem("TerrainScript/Debug/Generators/Perlin")]
        public static void ShowWindow()
        {
            Display();
        }
    }

    public class PlaneDebugger : GeneratorDebugger<PlaneDebugger>
    {
        public PlaneDebugger()
            : base("plane")
        {
        }

        [MenuItem("TerrainScript/Debug/Generators/Plane")]
        public static void ShowWindow()
        {
            Display();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Filter debuggers
    ////////////////////////////////////////////////////////////////////////////////////////////////

    public class InvertDebugger : FilterDebugger<InvertDebugger>
    {
        public InvertDebugger()
            : base("invert")
        {
        }

        [MenuItem("TerrainScript/Debug/Filters/Invert")]
        public static void ShowWindow()
        {
            Display();
        }
    }

    public class ScaleDebugger : FilterDebugger<ScaleDebugger>
    {
        public ScaleDebugger()
            : base("scale")
        {
        }

        [MenuItem("TerrainScript/Debug/Filters/Scale")]
        public static void ShowWindow()
        {
            Display();
        }
    }
}
