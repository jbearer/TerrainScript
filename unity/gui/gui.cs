using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

using Ts.Grayscale;
using Ts.Grayscale.Absyn;
using Ts.Grayscale.Filters;

namespace Ts.Unity
{
    using GeneratorGUI = ComponentGUI<Generator>;
    using FilterGUI = ComponentGUI<Filter>;
    using GeneratorGUIFactory = Func<ComponentGUI<Generator>>;
    using FilterGUIFactory = Func<ComponentGUI<Filter>>;
    using AstComponent = Ts.Grayscale.Absyn.Component;

    public class Log : Logging.StaticLogger<Log>
    {
        private static Logging.Logger _log = Logging.GetLogger("gui");
        protected override Logging.Logger log
        {
            get {
                return _log;
            }
        }
    }

    public class GuiException : Exception
    {
        public GuiException(string msg)
            : base(msg)
        {
        }
    }

    public abstract class ComponentGUI<T>
        where T : AstComponent
    {
        public string Name;
        private ArgList _state;
        private T _component;

        protected abstract ArgList OnGUI();

        public ComponentGUI(string component)
        {
            Name = component;
        }

        public T GetComponent()
        {
            var newState = OnGUI();
            if (newState != _state) {
                _state = newState;
                _component = GrayscaleEngine.CreateComponent<T>(Name, newState);
            }
            return _component;
        }
    }

    public class ComponentGUIImpl<T> : ComponentGUI<T>
        where T : AstComponent
    {
        private Func<ArgList> _onGUI;

        public ComponentGUIImpl(string component, Func<ArgList> onGUI)
            : base(component)
        {
            _onGUI = onGUI;
        }

        protected override ArgList OnGUI()
        {
            return _onGUI();
        }
    }

    public interface ComponentGUIAttribute<T>
        where T : AstComponent
    {
        string Name { get; }
    }

    public static class ComponentGUIAttributeExtensions
    {
        public static Func<ComponentGUI<T>> GetFactory<T>(this ComponentGUIAttribute<T> a, Type implType)
            where T : AstComponent
        {
            Log.Info("using GUI '{0}' for '{1}'", implType.ToString(), a.Name);

            return () => {
                var onGUI = implType.GetMethod("OnGUI", new Type[]{});
                if (onGUI == null || onGUI.IsStatic || !typeof(ArgList).IsAssignableFrom(onGUI.ReturnType)) {
                    throw new GuiException("gui registration for " + Ast.ComponentType<T>() + " '" +
                        a.Name + "' does not have instance method 'public ArgList OnGUI()'");
                }

                object impl = Activator.CreateInstance(implType);

                Log.Info("created GUI for " + Ast.ComponentType<T>() + " '" + a.Name + "'");

                return new ComponentGUIImpl<T>(a.Name,
                    () => (ArgList)onGUI.Invoke(impl, new object[]{}));
            };
        }
    }

    // Attribute used to register a GUI that can be used to create a generator
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class GeneratorGUIAttribute : Attribute, ComponentGUIAttribute<Generator>
    {
        public string Name { get; set; }

        public GeneratorGUIAttribute(string name)
        {
            Name = name;
        }
    }

    // Attribute used to register a GUI that can be used to create a filter
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class FilterGUIAttribute : Attribute, ComponentGUIAttribute<Filter>
    {
        public string Name { get; set; }

        public FilterGUIAttribute(string name)
        {
            Name = name;
        }
    }

    public static class Info<T>
        where T : AstComponent
    {
        // It's impossible to make this code generic, because we can't make attributes generic.
        // Sadly, we have to specialize. And because generic specialization is bad, we use an ugly
        // hack.

        public static Dictionary<string, Func<ComponentGUI<T>>> Factories = Info<T>.getFactories();

        private static Dictionary<string, Func<ComponentGUI<T>>> getFactories()
        {
            var infos = new Dictionary<string, Func<ComponentGUI<T>>>();

            Log.Info("discovering registered {0} GUIs", Ast.ComponentType<T>());

            if (typeof(T) == typeof(Generator)) {
                foreach (var typePair in Introspection.GetAllWithAttribute<GeneratorGUIAttribute>(false)) {
                    var factory = typePair.Attribute.GetFactory(typePair.Type);
                    infos.Add(typePair.Attribute.Name, () => (ComponentGUI<T>)(object)factory());
                }
            } else if (typeof(T) == typeof(Filter)) {
                foreach (var typePair in Introspection.GetAllWithAttribute<FilterGUIAttribute>(false)) {
                    var factory = typePair.Attribute.GetFactory(typePair.Type);
                    infos.Add(typePair.Attribute.Name, () => (ComponentGUI<T>)(object)factory());
                }
            } else {
                // Should never happen
                throw new GrayscaleException("component type is not generator or filter");
            }

            Log.Info("discovered {0} {1}s: {2}",
                infos.Count, Ast.ComponentType<T>(), string.Join(", ", infos.Keys.ToArray()));

            return infos;
        }
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

    public class GeneratorDebugger<ImplType> : TerrainWindow<ImplType>
        where ImplType : GeneratorDebugger<ImplType>
    {
        private float _amplitude = 1;
        private GeneratorGUI _gui;

        public GeneratorDebugger(string generator)
        {
            if (Info<Generator>.Factories.ContainsKey(generator)) {
                _gui = Info<Generator>.Factories[generator]();
            } else {
                throw new GuiException("no GUI for generator " + generator);
            }

            Log.Info("created debugger for generator '" + generator + "'");
        }

        public override void OnTerrain(Terrain terrain)
        {
            _amplitude = EditorGUILayout.Slider("Amplitude", _amplitude, 0, 1);
            List<Generator> gs = new List<Generator>{ _gui.GetComponent() };
            Log.Debug("bar");
            Filter scale = new Scale(_amplitude);
            Log.Debug("wuzzle");
            terrain.DebugControls(scale.Apply(gs));
        }
    }

    public class FilterDebugger<ImplType> : TerrainWindow<ImplType>
        where ImplType : FilterDebugger<ImplType>
    {
        private List<int> _generators = new List<int>();
        private bool _filter = true;
        private FilterGUI _gui;

        private static string[] generatorNames = Info<Generator>.Factories.Keys.ToArray();
        private static GeneratorGUI[] generatorGUIs =
            Info<Generator>.Factories.Values.Select(factory => factory()).ToArray();

        public FilterDebugger(string filter)
        {
            if (Info<Filter>.Factories.ContainsKey(filter)) {
                _gui = Info<Filter>.Factories[filter]();
            } else {
                throw new GuiException("no GUI for filter " + filter);
            }

            Log.Info("created debugger for filter '" + filter + "'");
        }

        public override void OnTerrain(Terrain terrain)
        {
            Filter f = _gui.GetComponent();

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
                    "Generator " + i.ToString(), _generators[i], generatorNames);
                _generators[i] = choice;
                gs.Add(generatorGUIs[choice].GetComponent());
            }

            Generator g = _filter ? f.Apply(gs) : gs[0];
            terrain.DebugControls(g);
        }
    }
}
