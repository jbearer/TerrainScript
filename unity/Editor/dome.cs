using UnityEditor;
using UnityEngine;

using Ts.Grayscale.Generators;

namespace Ts.Unity
{
    public class DomeDebugger : EditorWindow
    {
        private Terrain _terrain;
        private Vector2 _center = new Vector2(0.5f, 0.5f);
        private float _radius = 0.5f;

        [MenuItem("TerrainScript/Debug/Dome")]
        public static void ShowWindow()
        {
            if (!Terrain.activeTerrain) {
                EditorUtility.DisplayDialog("No terrain detected.", "Shutting down TerainScript.", "OK");
                return;
            }

            EditorWindow.GetWindow(typeof(DomeDebugger));
        }

        public void OnGUI()
        {
            _terrain = Terrain.activeTerrain;

            GUILayout.Label("Dome");

            _center = EditorGUILayout.Vector2Field("Center", _center);
            _radius = EditorGUILayout.Slider("Radius", _radius, 0, 1);

            _terrain.DebugControls(new Dome(_center.x, _center.y, _radius));
        }
    }
}
