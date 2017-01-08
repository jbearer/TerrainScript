using UnityEditor;
using UnityEngine;

using Ts.Grayscale;

namespace Ts.Unity
{
    public class GradientGUI : GeneratorGUI
    {
        private Vector2 _low = new Vector2(0, 0);
        private Vector2 _high = new Vector2 (1, 1);

        Generator GeneratorGUI.OnGUI()
        {
            GUILayout.Label("Gradient");

            _low = EditorGUILayout.Vector2Field("Low", _low);
            _high = EditorGUILayout.Vector2Field("High", _high);
            if (_low == _high) {
                EditorUtility.DisplayDialog(
                    "Invalid input.", "Cannot compute gradient between same two points.", "OK");
                _low = new Vector2(0, 0);
                _high = new Vector2(1, 1);
            }

            return new Ts.Grayscale.Generators.Gradient(_low.x, _low.y, _high.x, _high.y);
        }
    }

    public class GradientDebugger : GeneratorDebugger<GradientDebugger, GradientGUI>
    {
        [MenuItem("TerrainScript/Debug/Generators/Gradient")]
        public static void ShowWindow()
        {
            Display();
        }
    }
}
