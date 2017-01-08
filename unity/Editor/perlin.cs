using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Ts.Grayscale;
using Ts.Grayscale.Filters;
using Ts.Grayscale.Generators;

namespace Ts.Unity
{
    public class PerlinDebugger : EditorWindow
    {
        private Terrain _terrain;
        private float _amplitude = 1;
        private float _frequency = 50;
        private int _octaves = 1;
        private float _persistence = 0.5f;

        [MenuItem("TerrainScript/Debug/Perlin")]
        public static void ShowWindow()
        {
            if (!Terrain.activeTerrain) {
                EditorUtility.DisplayDialog("No terrain detected.", "Shutting down TerainScript.", "OK");
                return;
            }

            EditorWindow.GetWindow(typeof(PerlinDebugger));
        }

        public void OnGUI()
        {
            _terrain = Terrain.activeTerrain;

            GUILayout.Label("Perlin");

            _amplitude = EditorGUILayout.Slider("Amplitude", _amplitude, 0, 1);
            _frequency = EditorGUILayout.Slider("Frequency", _frequency, 0, 100);
            _octaves = EditorGUILayout.IntSlider("Octaves", _octaves, 1, 25);
            _persistence = EditorGUILayout.Slider("Persistence", _persistence, 0, 1);

            List<Generator> gs = new List<Generator>();
            gs.Add(new Perlin(_frequency, _octaves, _persistence));
            Filter scale = new Scale(_amplitude);
            _terrain.DebugControls(scale.Apply(gs));
        }
    }
}
