using UnityEditor;
using UnityEngine;

using Ts.Grayscale.Absyn;

namespace Ts.Unity.ComponentGUIs
{
    public class Log : Logging.StaticLogger<Log>
    {
        private Logging.Logger _log = Logging.GetLogger("component-guis");
        protected override Logging.Logger log
        {
            get {
                return _log;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Generator GUIs
    ////////////////////////////////////////////////////////////////////////////////////////////////

    [GeneratorGUI("circle")]
    public class CircleGUI
    {
        private Vector2 _center = new Vector2(0.5f, 0.5f);
        private float _radius = 0.5f;
        private float _feather = 0;

        public ArgList OnGUI()
        {
            GUILayout.Label("Circle");

            _center = EditorGUILayout.Vector2Field("Center", _center);
            _radius = EditorGUILayout.Slider("Radius", _radius, 0, 1);
            _feather = EditorGUILayout.Slider("Feather", _feather, 0, 1);

            return new ArgList {
                {"x", Ast.Float(_center.x)},
                {"y", Ast.Float(_center.y)},
                {"r", Ast.Float(_radius)},
                {"feather", Ast.Float(_feather)}
            };
        }
    }

    [GeneratorGUI("dome")]
    public class DomeGUI
    {
        private Vector2 _center = new Vector2(0.5f, 0.5f);
        private float _radius = 0.5f;

        public ArgList OnGUI()
        {
            GUILayout.Label("Dome");

            _center = EditorGUILayout.Vector2Field("Center", _center);
            _radius = EditorGUILayout.Slider("Radius", _radius, 0, 1);

            return new ArgList {
                { "x", Ast.Float(_center.x) },
                { "y", Ast.Float(_center.y) },
                { "r", Ast.Float(_radius) }
            };
        }
    }

    [GeneratorGUI("gradient")]
    public class GradientGUI
    {
        private Vector2 _low = new Vector2(0, 0);
        private Vector2 _high = new Vector2 (1, 1);

        public ArgList OnGUI()
        {
            GUILayout.Label("Gradient");

            _low = EditorGUILayout.Vector2Field("Low", _low);
            _high = EditorGUILayout.Vector2Field("High", _high);
            if (_low == _high) {
                EditorUtility.DisplayDialog(
                    "Invalid input.", "Cannot compute gradient between same two points.", "OK");
                _low = new Vector2(0, 0);
                _high = new Vector2(1, 1);
            }

            return new ArgList {
                { "xLow", Ast.Float(_low.x) },
                { "yLow", Ast.Float(_low.y) },
                { "xHigh", Ast.Float(_high.x) },
                { "yHigh", Ast.Float(_high.y) }
            };
        }
    }

    [GeneratorGUI("perlin")]
    public class PerlinGUI
    {
        private float _frequency = 50;
        private int _octaves = 1;
        private float _persistence = 0.5f;

        public ArgList OnGUI()
        {
            GUILayout.Label("Perlin");

            _frequency = EditorGUILayout.Slider("Frequency", _frequency, 0, 100);
            _octaves = EditorGUILayout.IntSlider("Octaves", _octaves, 1, 25);
            _persistence = EditorGUILayout.Slider("Persistence", _persistence, 0, 1);

            return new ArgList {
                { "frequency", Ast.Float(_frequency) },
                { "octaves", Ast.Int(_octaves) },
                { "persistence", Ast.Float(_persistence) }
            };
        }
    }

    [GeneratorGUI("plane")]
    public class PlaneGUI
    {
        public ArgList OnGUI()
        {
            GUILayout.Label("Plane");
            return new ArgList {};
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Filter GUIs
    ////////////////////////////////////////////////////////////////////////////////////////////////

    [FilterGUI("invert")]
    public class InvertGUI
    {
        public ArgList OnGUI()
        {
            GUILayout.Label("Invert");
            return new ArgList();
        }
    }

    [FilterGUI("scale")]
    public class ScaleGUI
    {
        private float _amplitude = 1;

        public ArgList OnGUI()
        {
            GUILayout.Label("Scale");
            _amplitude = EditorGUILayout.Slider("Amplitude", _amplitude, 0, 1);
            return new ArgList {
                {"factor", Ast.Float(_amplitude)}
            };
        }
    }
}
