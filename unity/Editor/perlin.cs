using UnityEditor;
using UnityEngine;

using Ts.Grayscale;
using Ts.Grayscale.Generators;

namespace Ts.Unity
{
    public class PerlinDebugger : EditorWindow
    {
        private Terrain _terrain;
        private float _amplitude = 0;
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

            _amplitude = EditorGUILayout.Slider(_amplitude, 0, _terrain.terrainData.size.y);
            _frequency = EditorGUILayout.Slider(_frequency, 0, 100);
            _octaves = EditorGUILayout.IntSlider(_octaves, 1, 25);
            _persistence = EditorGUILayout.Slider(_persistence, 0, 1);

            _terrain.DebugControls(new Perlin(
                _amplitude / _terrain.terrainData.size.y, _frequency, _octaves, _persistence));
        }
    }
}
