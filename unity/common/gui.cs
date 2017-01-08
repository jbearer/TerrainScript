using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Ts.Grayscale;
using Ts.Grayscale.Filters;

namespace Ts.Unity
{
    public interface GeneratorGUI
    {
        Generator OnGUI();
    }

    public abstract class GeneratorDebugger<ImplType, GUIType> : EditorWindow
        where ImplType : GeneratorDebugger<ImplType, GUIType>
        where GUIType : GeneratorGUI, new()
    {
        private float _amplitude = 1;
        private GUIType _gui = new GUIType();

        public static void Display()
        {
            if (!Terrain.activeTerrain) {
                EditorUtility.DisplayDialog("No terrain detected.", "Shutting down TerainScript.", "OK");
                return;
            }

            EditorWindow.GetWindow(typeof(ImplType));
        }

        public void OnGUI()
        {
            if (!Terrain.activeTerrain) {
                EditorUtility.DisplayDialog("No terrain detected.", "Shutting down TerainScript.", "OK");
                EditorWindow.Destroy(EditorWindow.GetWindow(typeof(ImplType)));
                return;
            }

            _amplitude = EditorGUILayout.Slider("Amplitude", _amplitude, 0, 1);

            List<Generator> gs = new List<Generator>();
            gs.Add(_gui.OnGUI());
            Filter scale = new Scale(_amplitude);
            Terrain.activeTerrain.DebugControls(scale.Apply(gs));
        }
    }
}
