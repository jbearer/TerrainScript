using UnityEditor;
using UnityEngine;

using Ts.Grayscale;
using Ts.Grayscale.Generators;

namespace Ts.Unity
{
    public class DomeGUI : GeneratorGUI
    {
        private Vector2 _center = new Vector2(0.5f, 0.5f);
        private float _radius = 0.5f;

        Generator GeneratorGUI.OnGUI()
        {
            GUILayout.Label("Dome");

            _center = EditorGUILayout.Vector2Field("Center", _center);
            _radius = EditorGUILayout.Slider("Radius", _radius, 0, 1);

            return new Dome(_center.x, _center.y, _radius);
        }
    }
}
