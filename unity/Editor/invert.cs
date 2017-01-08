using UnityEditor;

namespace Ts.Unity
{
    public class InvertDebugger : FilterDebugger<InvertDebugger, InvertGUI>
    {
        [MenuItem("TerrainScript/Debug/Filters/Invert")]
        public static void ShowWindow()
        {
            Display();
        }
    }
}
