using System;
using System.Collections.Generic;

namespace Ts.Grayscale.Absyn
{
    class Log : Logging.StaticLogger<Log>
    {
        private static Logging.Logger _log = Logging.GetLogger("ast");
        protected override Logging.Logger log
        {
            get {
                return Log._log;
            }
        }
    }

    public class ArgList : Dictionary<string, AstValue>
    {
        public override bool Equals(object other)
        {
            if (!(other is Dictionary<string, AstValue>)) {
                return false;
            }

            var d = (Dictionary<string, AstValue>)other;

            if (Count != d.Count) {
                return false;
            }

            foreach (var kv in this) {
                if (!d.ContainsKey(kv.Key) || !d[kv.Key].Equals(kv.Value)) {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            throw new GrayscaleException("ArgList is not hashable");
        }

        public static bool operator ==(ArgList a, Dictionary<string, AstValue> other) {
            return a.Equals(other);
        }

        public static bool operator !=(ArgList a, Dictionary<string, AstValue> other) {
            return !(a == other);
        }
    }

    // Sugar and convenience functions
    public static class Ast
    {
        public static AstFloat Float(float val)
        {
            return new AstFloat(val);
        }

        public static AstInt Int(int val)
        {
            return new AstInt(val);
        }

        // Useful for diagnostics in generic code
        public static string ComponentType<T>()
            where T : Component
        {
            return (typeof(T) == typeof(Generator)) ? "generator" : "filter";
        }
    }

    public enum AstType
    {
        Int,
        Float,
        Generator,
        Filter
    }

    /**
     * We use a bit of a strange idiom to take advantage of the only case of pattern matching
     * allowed by C#: pattern matching on exception types. If all AST elements inherit from
     * Exception, we can "pattern match" like:
     *
     * AstValue v = ...;
     * try {throw v;};
     * catch (AstInt i) { ... do something with int ... }
     * catch (AstFloat f) { ... do something with float ... }
     * ...
     */
    public abstract class AstValue : Exception
    {
        public readonly AstType AstType;

        public AstValue(AstType astType)
        {
            AstType = astType;
        }

        public T AsType<T>(string expected) where T : AstValue
        {
            try { throw this; }
            catch (T v) { return v; }
            catch (AstValue v) {
                throw new GrayscaleException("type mismatch:\n" +
                    "\tRequired: " + expected + "\n" +
                    "\tFound: " + v.AstType.ToString());
            }
        }

        public AstInt AsInt {
            get {
                return AsType<AstInt>("int");
            }
        }

        public AstFloat AsFloat {
            get {
                return AsType<AstFloat>("float");
            }
        }

        public Generator AsGenerator {
            get {
                return AsType<Generator>("generator");
            }
        }

        public Filter AsFilter {
            get {
                return AsType<Filter>("filter");
            }
        }
    }

    public class AstInt : AstValue
    {
        public int Value;

        public AstInt(int val)
            : base(AstType.Int)
        {
            Value = val;
        }

        public override bool Equals(object other)
        {
            return other is AstInt ? Value == ((AstInt)other).Value : false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class AstFloat : AstValue
    {
        public float Value;

        public AstFloat(float val)
            : base(AstType.Float)
        {
            Value = val;
        }

        public override bool Equals(object other)
        {
           return other is AstFloat ? Value == ((AstFloat)other).Value : false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public abstract class Component : AstValue
    {
        public Component(AstType type)
            : base(type)
        {
        }
    };

    public abstract class Generator : Component
    {
        public Generator()
            : base(AstType.Generator)
        {
        }

        public abstract float GenerateValue(float x, float y);
        public abstract GrayscaleImage GenerateImage(int height, int width);
    }

    public abstract class Filter : Component
    {
        public Filter()
            : base(AstType.Filter)
        {
        }

        public abstract int Arity();
        public abstract Generator Apply(List<Generator> gs);
    }

    public abstract class PointwiseGenerator : Generator
    {
        public override float GenerateValue(float x, float y)
        {
            return Apply(x, y);
        }

        public override GrayscaleImage GenerateImage(int height, int width)
        {
            return new LazyGrayscale(height, width, Apply);
        }

        protected abstract float Apply(float x, float y);
    }

    public class LambdaGenerator : PointwiseGenerator
    {
        public delegate float Applier(float x, float y);

        private Applier _apply;

        public LambdaGenerator(Applier apply)
        {
            _apply = apply;
        }

        protected override float Apply(float x, float y)
        {
            return _apply(x, y);
        }
    }

    public abstract class FixedArityFilter : Filter
    {
        private int _arity;

        public FixedArityFilter(int arity)
        {
            _arity = arity;
        }

        public override int Arity()
        {
            return _arity;
        }

        public override Generator Apply(List<Generator> gs)
        {
            if (gs.Count != _arity) {
                throw new GrayscaleException(
                    string.Format("{0}-ary filter applied to {1} generators", _arity, gs.Count));
            }
            return ApplySafe(gs);
        }

        protected abstract Generator ApplySafe(List<Generator> gs);
    }

    public abstract class UnaryFilter : FixedArityFilter
    {
        public UnaryFilter()
            : base(1)
        {
        }

        protected override Generator ApplySafe(List<Generator> gs)
        {
            return Apply(gs[0]);
        }

        protected abstract Generator Apply(Generator g);
    }

    public abstract class BinaryFilter : FixedArityFilter
    {
        public BinaryFilter()
            : base(2)
        {
        }

        protected override Generator ApplySafe(List<Generator> gs)
        {
            return Apply(gs[0], gs[1]);
        }

        protected abstract Generator Apply(Generator l, Generator r);
    }
}
