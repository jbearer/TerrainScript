using UnityEditor;
using UnityEngine;

using Ts.Grayscale;
using Ts.Grayscale.Generators;

namespace Ts.Unity
{
    public class CircleGUI : GeneratorGUI
    {
        private Vector2 _center = new Vector2(0.5f, 0.5f);
        private float _radius = 0.5f;
        private float _feather = 0;

        Generator GeneratorGUI.OnGUI()
        {
            GUILayout.Label("Circle");

            _center = EditorGUILayout.Vector2Field("Center", _center);
            _radius = EditorGUILayout.Slider("Radius", _radius, 0, 1);
            _feather = EditorGUILayout.Slider("Feather", _feather, 0, 1);

            return new Circle(_center.x, _center.y, _radius, _feather);
        }
    }

    public class CircleDebugger : GeneratorDebugger<CircleDebugger, CircleGUI>
    {
        [MenuItem("TerrainScript/Debug/Generators/Circle")]
        public static void ShowWindow()
        {
            Display();
        }
    }
}
