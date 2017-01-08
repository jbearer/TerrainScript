namespace Ts.Grayscale.Generators
{
    public class Circle : PointwiseGenerator
    {
        private Math.Vector2 _center;
        private float        _radius;
        private float        _feather;

        public Circle(float x, float y, float r, float feather = 0)
        {
            _center = new Math.Vector2(x, y);
            _radius = r;
            _feather = feather;
        }

        protected override float Apply(float x, float y)
        {
            Math.Vector2 p = new Math.Vector2(x, y) - _center;
            float r2 = _radius * _radius;
            float f2 = (_radius + _feather)*(_radius + _feather);
            float d2 = Math.Dot(p , p);
            if (d2 <= r2) {
                return 1;
            } else if (_feather != 0 && d2 <= f2) {
                float r = (float)System.Math.Sqrt(r2);
                float f = (float)System.Math.Sqrt(f2);
                float d = (float)System.Math.Sqrt(d2);
                return Math.Lerp(1, 0, (d - r) / (f - r));
            } else {
                return 0;
            }
        }
    }
}
