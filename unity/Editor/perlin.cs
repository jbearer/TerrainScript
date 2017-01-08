using UnityEditor;

namespace Ts.Unity
{
    public class PerlinDebugger : GeneratorDebugger<PerlinDebugger, PerlinGUI>
    {
        [MenuItem("TerrainScript/Debug/Generators/Perlin")]
        public static void ShowWindow()
        {
            Display();
        }
    }
}
