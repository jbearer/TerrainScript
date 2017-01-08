using UnityEditor;

namespace Ts.Unity
{
    public class DomeDebugger : GeneratorDebugger<DomeDebugger, DomeGUI>
    {
        [MenuItem("TerrainScript/Debug/Generators/Dome")]
        public static void ShowWindow()
        {
            Display();
        }
    }
}
