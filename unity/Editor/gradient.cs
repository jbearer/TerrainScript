using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Ts.Grayscale;
using Ts.Grayscale.Filters;

namespace Ts.Unity
{
    public class GradientDebugger : EditorWindow
    {
        private Terrain _terrain;
        private float _amplitude = 1;
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

            _amplitude = EditorGUILayout.Slider("Amplitude", _amplitude, 0, 1);
            _low = EditorGUILayout.Vector2Field("Low", _low);
            _high = EditorGUILayout.Vector2Field("High", _high);
            if (_low == _high) {
                EditorUtility.DisplayDialog(
                    "Invalid input.", "Cannot compute gradient between same two points.", "OK");
                _low = new Vector2(0, 0);
                _high = new Vector2(1, 1);
            }

            List<Generator> gs = new List<Generator>();
            gs.Add(new Ts.Grayscale.Generators.Gradient(_low.x, _low.y, _high.x, _high.y));
            Filter scale = new Scale(_amplitude);
            _terrain.DebugControls(scale.Apply(gs));
        }
    }
}
