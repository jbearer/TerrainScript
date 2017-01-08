using UnityEditor;

namespace Ts.Unity
{
    public class ScaleDebugger : FilterDebugger<ScaleDebugger, ScaleGUI>
    {
        [MenuItem("TerrainScript/Debug/Filters/Scale")]
        public static void ShowWindow()
        {
            Display();
        }
    }
}
