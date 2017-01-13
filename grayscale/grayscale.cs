using System;
using System.Collections.Generic;
using System.Linq;

using Ts.Grayscale.Absyn;

namespace Ts.Grayscale
{
    public class Log : Logging.StaticLogger<Log>
    {
        private static Logging.Logger _log = Logging.GetLogger("grayscale");
        protected override Logging.Logger log
        {
            get {
                return _log;
            }
        }
    }

    public static class GrayscaleEngine
    {
        public static T CreateComponent<T>(string name, ArgList args)
            where T : Component
        {
            if (Info<T>.Factories.ContainsKey(name)) {
                return Info<T>.Factories[name](args);
            } else {
                throw new GrayscaleException("no " + Ast.ComponentType<T>() + " '" + name + "'");
            }
        }
    }

    // Exception used to indicate that a generator cannot create an image for any reason
    public class GrayscaleException : System.Exception
    {
        public GrayscaleException(string message)
            : base(message)
        {
        }
    }

    public interface GrayscaleImage
    {
        int Height();
        int Width();
        float GetValue(int x, int y);
        float[,] ToArray();
    }

    public interface ComponentAttribute<T>
        where T : Component
    {
        string Name { get; }

        // Required named arguments. If specified, the system will check that these arguments exist
        // and have the right types before calling FactoryType.ParseArgs.
        string[] RequiredArgs { get; }
        AstType[] RequiredArgTypes { get; }

        // Optional named arguments. Before calling FactoryType.ParseArgs, the system will check
        // that any of the given arguments are present in either RequiredArgs or OptionalArgs.
        // In addition, if any optional args are not specified, they will be added to the specified
        // args with a default value, so that when the underlying generator attempts to parse the
        // arguments, all arguments are guaranteed to be present.
        // Note: because of the limitations regarding what types of parameters can be passed to
        // attributes, the default arguments are not typesafe. So be careful.
        string[] OptionalArgs { get; }
        AstType[] OptionalArgTypes { get; }
        object[] OptionalArgDefaults { get; }
    }

    public static class ComponentAttributeExtensions
    {
        private static Exception FailIt<T>(this ComponentAttribute<T> a, string msg)
            where T : Component
        {
            return new GrayscaleException(
                Ast.ComponentType<T>() + " registration for " + a.Name + " " + msg);
        }

        public static Func<ArgList, T> GetFactory<T>(this ComponentAttribute<T> a, Type implType)
            where T : Component
        {
            Log.Info("using {0} implementation '{1}' for '{2}'",
                Ast.ComponentType<T>(), implType.ToString(), a.Name);

            var factory = implType.GetMethod("ParseArgs", new Type[]{typeof(ArgList)});
            if (factory == null
                || !typeof(T).IsAssignableFrom(factory.ReturnType)
                || !factory.IsStatic)
            {
                throw a.FailIt("does not have static method '" +
                    typeof(T).ToString() + " ParseArgs(ArgList)'");
            }

            // Turn those cumbersome arrays into dictionaries, making sure that all the argument
            // names are distinct.
            var requiredArgs = new Dictionary<string, AstType>();
            var optionalArgs = new ArgList();
            for (int i = 0; i < a.RequiredArgs.Length; i++) {
                if (requiredArgs.ContainsKey(a.RequiredArgs[i])) {
                    throw a.FailIt("has two arguments named " + a.RequiredArgs[i]);
                } else {
                    requiredArgs.Add(a.RequiredArgs[i], a.RequiredArgTypes[i]);
                }
            }
            for (int i = 0; i < a.OptionalArgs.Length; i++) {
                if (requiredArgs.ContainsKey(a.OptionalArgs[i])
                    || optionalArgs.ContainsKey(a.OptionalArgs[i])) {
                    throw a.FailIt("has two arguments named " + a.OptionalArgs[i]);
                } else {
                    AstValue defaultValue = null;
                    switch (a.OptionalArgTypes[i]) {
                    case AstType.Int:
                        defaultValue = new AstInt((int)a.OptionalArgDefaults[i]);
                        break;
                    case AstType.Float:
                        defaultValue = new AstFloat((float)a.OptionalArgDefaults[i]);
                        break;
                    default:
                        throw a.FailIt("has unsupported default argument type '" +
                            a.OptionalArgTypes[i].ToString() + "'");
                    }
                    optionalArgs.Add(a.OptionalArgs[i], defaultValue);
                }
            }

            return (args) => {
                // Make sure we have all of the required args.
                foreach (var requiredArg in requiredArgs) {
                    if (args.ContainsKey(requiredArg.Key)) {
                        if (args[requiredArg.Key].AstType != requiredArg.Value) {
                            throw new GrayscaleException("parameter '" + requiredArg.Key + "' " +
                                "of " + Ast.ComponentType<T>() + " '" + a.Name + "' is the wrong type.\n" +
                                "\tRequired: " + requiredArg.Value.ToString() + "\n" +
                                "\tFound: " + args[requiredArg.Key].AstType.ToString());
                        }
                    } else {
                        throw new GrayscaleException("instantiation of " + Ast.ComponentType<T>() + " '" +
                            a.Name + "' " + "requires argument '" + requiredArg.Key + "'");
                    }
                }

                // Make sure every argument specified is either required or optional, and has the
                // correct type.
                foreach (var arg in args) {
                    if (requiredArgs.ContainsKey(arg.Key)) {
                        if (arg.Value.AstType != requiredArgs[arg.Key]) {
                            throw new GrayscaleException("parameter '" + arg.Key + "' " +
                                "of " + Ast.ComponentType<T>() + " '" + a.Name + "' is the wrong type.\n" +
                                "\tRequired: " + requiredArgs[arg.Key].ToString() + "\n" +
                                "\tFound: " + arg.Value.AstType.ToString());
                        }
                    } else if (optionalArgs.ContainsKey(arg.Key)) {
                        if (arg.Value.AstType != optionalArgs[arg.Key].AstType) {
                            throw new GrayscaleException("parameter '" + arg.Key + "' " +
                                "of " + Ast.ComponentType<T>() + " '" + a.Name + "' is the wrong type.\n" +
                                "\tRequired: " + optionalArgs[arg.Key].ToString() + "\n" +
                                "\tFound: " + arg.Value.AstType.ToString());
                        }
                    } else {
                        throw new GrayscaleException("unrecognized parameter '" + arg.Key + "' " +
                            "in instantiation of " + Ast.ComponentType<T>() + " '" + a.Name + "'");
                    }
                }

                // Go through optional arguments, adding default values where no value is specified
                foreach (var optArg in optionalArgs) {
                    if (!args.ContainsKey(optArg.Key)) {
                        args.Add(optArg.Key, optArg.Value);
                    }
                }

                Log.Info("instantiating '{0}' with args [{1}]", a.Name,
                    string.Join(", ", args.Keys.Select(k => k + " = " + args[k].ToString()).ToArray()));

                // Delegate to the underlying constructor
                return (T)(factory.Invoke(null, new object[]{args}));
            };
        }
    }

    public abstract class ComponentAttributeBase : Attribute
    {
        public string Name { get; set; }
        public string[] RequiredArgs { get; set; }
        public AstType[] RequiredArgTypes { get; set; }
        public string[] OptionalArgs { get; set; }
        public AstType[] OptionalArgTypes { get; set; }
        public object[] OptionalArgDefaults { get; set; }

        public ComponentAttributeBase(string name)
        {
            Name = name;

            // All the argument arrays default to empty
            if (RequiredArgs == null) {
                RequiredArgs = new string[]{};
            }
            if (RequiredArgTypes == null) {
                RequiredArgTypes = new AstType[]{};
            }
            if (OptionalArgs == null) {
                OptionalArgs = new string[]{};
            }
            if (OptionalArgTypes == null) {
                OptionalArgTypes = new AstType[]{};
            }
            if (OptionalArgDefaults == null) {
                OptionalArgDefaults = new object[]{};
            }

            // Number of required args must be the same as number of specified types
            if (RequiredArgs.Length != RequiredArgTypes.Length) {
                throw new GrayscaleException("component registration for " + name +
                    " specifies required argument names and types of different lengths");
            }

            // Same for OptionalArgs
            if (OptionalArgs.Length != OptionalArgTypes.Length) {
                throw new GrayscaleException("component registration for " + name +
                    " specifies optional argument names and types of different lengths");
            }
            if (OptionalArgs.Length != OptionalArgDefaults.Length) {
                throw new GrayscaleException("component registration for " + name +
                    " specifies optional argument names and default values of different lengths");
            }
        }
    }

    // Attribute used to register a generator with the grayscale engine.
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class GeneratorAttribute : ComponentAttributeBase, ComponentAttribute<Generator>
    {
        public GeneratorAttribute(string name)
            : base(name)
        {
        }
    }

     // Attribute used to register a filter with the grayscale engine.
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class FilterAttribute : ComponentAttributeBase, ComponentAttribute<Filter>
    {
        public FilterAttribute(string name)
            : base(name)
        {
        }
    }

    public static class Info<T>
        where T : Component
    {
        // It's impossible to make this code generic, because we can't make attributes generic.
        // Sadly, we have to specialize. Unfortunately, generic specialization is bad and so we use
        // an ugly introspection hack (what else is new).

        public static Dictionary<string, Func<ArgList, T>> Factories = Info<T>.getFactories();

        private static Dictionary<string, Func<ArgList, T>> getFactories()
        {
            Log.Info("discovering registered {0}s", Ast.ComponentType<T>());

            var infos = new Dictionary<string, Func<ArgList, T>>();

            if (typeof(T) == typeof(Generator)) {
                foreach (var typePair in Introspection.GetAllWithAttribute<GeneratorAttribute>(false)) {
                    var factory = typePair.Attribute.GetFactory(typePair.Type);
                    infos.Add(typePair.Attribute.Name, (args) => (T)(object)factory(args));
                }
            } else if (typeof(T) == typeof(Filter)) {
                foreach (var typePair in Introspection.GetAllWithAttribute<FilterAttribute>(false)) {
                    var factory = typePair.Attribute.GetFactory(typePair.Type);
                    infos.Add(typePair.Attribute.Name, (args) => (T)(object)factory(args));
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

    public class LazyGrayscale : GrayscaleImage
    {
        public delegate float ValueGenerator(float x, float y);

        private ValueGenerator _getValue;
        private float?[,] _data;

        public LazyGrayscale(int height, int width, ValueGenerator getValue)
        {
            _getValue = getValue;
            _data = new float?[height, width];
        }

        int GrayscaleImage.Height()
        {
            return _data.GetLength(0);
        }

        int GrayscaleImage.Width()
        {
            return _data.GetLength(1);
        }

        float GrayscaleImage.GetValue(int x, int y)
        {
            if (!_data[y, x].HasValue) {
                _data[y, x] = _getValue((float)x / _data.GetLength(1), (float)y / _data.GetLength(0));
            }
            return _data[y, x].Value;
        }

        float[,] GrayscaleImage.ToArray()
        {
            float[,] result = new float[_data.GetLength(0), _data.GetLength(1)];
            Arrays.ForEach2D(result, (int row, int col, ref float val) => {
                val = ((GrayscaleImage)this).GetValue(col, row);
            });
            return result;
        }
    }
}
