using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Ts.Grayscale;
using Ts.Grayscale.Filters;
using Ts.Grayscale.Generators;

namespace Ts.Unity
{
    public class CircleDebugger : EditorWindow
    {
        private Terrain _terrain;
        private float _amplitude = 1;
        private Vector2 _center = new Vector2(0.5f, 0.5f);
        private float _radius = 0.5f;
        private float _feather = 0;

        [MenuItem("TerrainScript/Debug/Circle")]
        public static void ShowWindow()
        {
            if (!Terrain.activeTerrain) {
                EditorUtility.DisplayDialog("No terrain detected.", "Shutting down TerainScript.", "OK");
                return;
            }

            EditorWindow.GetWindow(typeof(CircleDebugger));
        }

        public void OnGUI()
        {
            _terrain = Terrain.activeTerrain;

            GUILayout.Label("Circle");

            _amplitude = EditorGUILayout.Slider("Amplitude", _amplitude, 0, 1);
            _center = EditorGUILayout.Vector2Field("Center", _center);
            _radius = EditorGUILayout.Slider("Radius", _radius, 0, 1);
            _feather = EditorGUILayout.Slider("Feather", _feather, 0, 1);

            List<Generator> gs = new List<Generator>();
            gs.Add(new Circle(_center.x, _center.y, _radius, _feather));
            Filter scale = new Scale(_amplitude);
            _terrain.DebugControls(scale.Apply(gs));
        }
    }
}
