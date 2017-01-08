using UnityEditor;
using UnityEngine;

using Ts.Grayscale;
using Ts.Grayscale.Filters;

namespace Ts.Unity
{
    public class ScaleGUI : FilterGUI
    {
        private float _amplitude = 1;

        Filter FilterGUI.OnGUI()
        {
            GUILayout.Label("Scale");
            _amplitude = EditorGUILayout.Slider("Amplitude", _amplitude, 0, 1);
            return new Scale(_amplitude);
        }
    }
}
