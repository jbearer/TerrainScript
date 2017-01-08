using UnityEditor;

namespace Ts.Unity
{
    public class PlaneDebugger : GeneratorDebugger<PlaneDebugger, PlaneGUI>
    {
        [MenuItem("TerrainScript/Debug/Generators/Plane")]
        public static void ShowWindow()
        {
            Display();
        }
    }
}
