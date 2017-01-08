using UnityEditor;
using UnityEngine;

using Ts.Grayscale;
using Ts.Grayscale.Generators;

namespace Ts.Unity
{
    public class PerlinGUI : GeneratorGUI
    {
        private float _frequency = 50;
        private int _octaves = 1;
        private float _persistence = 0.5f;

        Generator GeneratorGUI.OnGUI()
        {
            GUILayout.Label("Perlin");

            _frequency = EditorGUILayout.Slider("Frequency", _frequency, 0, 100);
            _octaves = EditorGUILayout.IntSlider("Octaves", _octaves, 1, 25);
            _persistence = EditorGUILayout.Slider("Persistence", _persistence, 0, 1);

            return new Perlin(_frequency, _octaves, _persistence);
        }
    }
}
