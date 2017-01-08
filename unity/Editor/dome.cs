using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Ts.Grayscale;
using Ts.Grayscale.Filters;
using Ts.Grayscale.Generators;

namespace Ts.Unity
{
    public class DomeDebugger : EditorWindow
    {
        private Terrain _terrain;
        private float _amplitude = 1;
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

            _amplitude = EditorGUILayout.Slider("Amplitude", _amplitude, 0, 1);
            _center = EditorGUILayout.Vector2Field("Center", _center);
            _radius = EditorGUILayout.Slider("Radius", _radius, 0, 1);

            List<Generator> gs = new List<Generator>();
            gs.Add(new Dome(_center.x, _center.y, _radius));
            Filter scale = new Scale(_amplitude);
            _terrain.DebugControls(scale.Apply(gs));
        }
    }
}
