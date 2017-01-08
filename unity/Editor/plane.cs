using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Ts.Grayscale;
using Ts.Grayscale.Filters;
using Ts.Grayscale.Generators;

namespace Ts.Unity
{
    public class PlaneDebugger : EditorWindow
    {
        private Terrain _terrain;
        private float _height = 1;

        [MenuItem("TerrainScript/Debug/Plane")]
        public static void ShowWindow()
        {
            if (!Terrain.activeTerrain) {
                EditorUtility.DisplayDialog("No terrain detected.", "Shutting down TerainScript.", "OK");
                return;
            }

            EditorWindow.GetWindow(typeof(PlaneDebugger));
        }

        public void OnGUI()
        {
            _terrain = Terrain.activeTerrain;

            GUILayout.Label("Plane");

            _height = EditorGUILayout.Slider("Height", _height, 0, 1);

            List<Generator> gs = new List<Generator>();
            gs.Add(new Ts.Grayscale.Generators.Plane());
            Filter scale = new Scale(_height);
            _terrain.DebugControls(scale.Apply(gs));
        }
    }
}
