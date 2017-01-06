namespace Ts.Grayscale
{
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
        Generator Apply(Generator g);
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

        public abstract float Apply(float x, float y);
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
