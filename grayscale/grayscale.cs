using System.Collections.Generic;

namespace Ts.Grayscale
{
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

    public interface Generator
    {
        float GenerateValue(float x, float y);
        GrayscaleImage GenerateImage(int height, int width);
    }

    public interface Filter
    {
        int Arity();
        Generator Apply(List<Generator> gs);
    }

    public abstract class PointwiseGenerator : Generator
    {
        float Generator.GenerateValue(float x, float y)
        {
            return Apply(x, y);
        }

        GrayscaleImage Generator.GenerateImage(int height, int width)
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

        int Filter.Arity()
        {
            return _arity;
        }

        Generator Filter.Apply(List<Generator> gs)
        {
            if (gs.Count != _arity) {
                throw new GrayscaleException(
                    string.Format("{0}-ary filter applied to {1} generators", _arity, gs.Count));
            }
            return Apply(gs);
        }

        protected abstract Generator Apply(List<Generator> gs);
    }

    public abstract class UnaryFilter : FixedArityFilter
    {
        public UnaryFilter()
            : base(1)
        {
        }

        protected override Generator Apply(List<Generator> gs)
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

        protected override Generator Apply(List<Generator> gs)
        {
            return Apply(gs[0], gs[1]);
        }

        protected abstract Generator Apply(Generator l, Generator r);
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
