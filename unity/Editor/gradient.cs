using UnityEditor;

namespace Ts.Unity
{
    public class GradientDebugger : GeneratorDebugger<GradientDebugger, GradientGUI>
    {
        [MenuItem("TerrainScript/Debug/Generators/Gradient")]
        public static void ShowWindow()
        {
            Display();
        }
    }
}
