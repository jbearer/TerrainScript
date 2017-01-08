using UnityEngine;

using Ts.Grayscale;
using Ts.Grayscale.Filters;

namespace Ts.Unity
{
    public class InvertGUI : FilterGUI
    {
        Filter FilterGUI.OnGUI()
        {
            GUILayout.Label("Invert");
            return new Invert();
        }
    }
}
