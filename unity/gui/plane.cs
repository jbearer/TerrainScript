using UnityEditor;
using UnityEngine;

using Ts.Grayscale;
using Ts.Grayscale.Generators;

namespace Ts.Unity
{
    public class PlaneGUI : GeneratorGUI
    {
        Generator GeneratorGUI.OnGUI()
        {
            GUILayout.Label("Plane");
            return new Ts.Grayscale.Generators.Plane();
        }
    }
}