using UnityEditor;
using UnityEngine;

namespace Ts.Unity
{
    public class GradientDebugger : EditorWindow
    {
        private Terrain _terrain;
        private Vector2 _low = new Vector2(0, 0);
        private Vector2 _high = new Vector2 (1, 1);

        [MenuItem("TerrainScript/Debug/Gradient")]
        public static void ShowWindow()
        {
            if (!Terrain.activeTerrain) {
                EditorUtility.DisplayDialog("No terrain detected.", "Shutting down TerainScript.", "OK");
                return;
            }

            EditorWindow.GetWindow(typeof(GradientDebugger));
        }

        public void OnGUI()
        {
            _terrain = Terrain.activeTerrain;

            GUILayout.Label("Gradient");

            _low = EditorGUILayout.Vector2Field("Low", _low);
            _high = EditorGUILayout.Vector2Field("High", _high);
            if (_low == _high) {
                EditorUtility.DisplayDialog(
                    "Invalid input.", "Cannot compute gradient between same two points.", "OK");
                _low = new Vector2(0, 0);
                _high = new Vector2(1, 1);
            }

            _terrain.DebugControls(new Ts.Grayscale.Generators.Gradient(_low.x, _low.y, _high.x, _high.y));
        }
    }
}
