using UnityEditor;

namespace Ts.Unity
{
    public class CircleDebugger : GeneratorDebugger<CircleDebugger, CircleGUI>
    {
        [MenuItem("TerrainScript/Debug/Generators/Circle")]
        public static void ShowWindow()
        {
            Display();
        }
    }
}
