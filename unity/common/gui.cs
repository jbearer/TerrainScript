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

    public interface FilterGUI
    {
        Filter OnGUI();
    }

    public abstract class TerrainWindow<ImplType> : EditorWindow
        where ImplType : TerrainWindow<ImplType>
    {
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
            } else {
                OnTerrain(Terrain.activeTerrain);
            }
        }

        public abstract void OnTerrain(Terrain terrain);
    }

    public class GeneratorDebugger<ImplType, GUIType> : TerrainWindow<ImplType>
        where ImplType : GeneratorDebugger<ImplType, GUIType>
        where GUIType : GeneratorGUI, new()
    {
        private float _amplitude = 1;
        private GUIType _gui = new GUIType();

        public override void OnTerrain(Terrain terrain)
        {
            _amplitude = EditorGUILayout.Slider("Amplitude", _amplitude, 0, 1);

            List<Generator> gs = new List<Generator>();
            gs.Add(_gui.OnGUI());
            Filter scale = new Scale(_amplitude);
            terrain.DebugControls(scale.Apply(gs));
        }
    }

    public class FilterDebugger<ImplType, GUIType> : TerrainWindow<ImplType>
        where ImplType : FilterDebugger<ImplType, GUIType>
        where GUIType : FilterGUI, new()
    {
        private List<int> _generators = new List<int>();
        private GUIType _gui = new GUIType();
        private bool _filter = true;

        public static string[] GeneratorNames = {
            "perlin",
            "gradient",
            "circle",
            "dome",
            "plane"
        };
        public static GeneratorGUI[] GeneratorGUIs = {
            new PerlinGUI(),
            new GradientGUI(),
            new CircleGUI(),
            new DomeGUI(),
            new PlaneGUI()
        };

        public override void OnTerrain(Terrain terrain)
        {
            Filter f = _gui.OnGUI();

            if (f.Arity() == 1) {
                // For unary filters, we can choose whether to apply the filter or just display the
                // results of the generator, unfiltered
                _filter = GUILayout.Toggle(_filter, "Filter?");
            } else {
                _filter = true;
            }

            List<Generator> gs = new List<Generator>();
            for (int i = 0; i < f.Arity(); i++) {
                if (i >= _generators.Count) {
                    _generators.Add(0);
                }
                int choice = EditorGUILayout.Popup(
                    "Generator " + i.ToString(), _generators[i], GeneratorNames);
                _generators[i] = choice;
                gs.Add(GeneratorGUIs[choice].OnGUI());
            }

            Generator g = _filter ? f.Apply(gs) : gs[0];
            terrain.DebugControls(g);
        }
    }
}
