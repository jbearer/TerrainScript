namespace Ts.Grayscale.Generators
{
    public class Gradient : PointwiseGenerator
    {
        private Math.Vector2 _high;
        private Math.Vector2 _low;

        public Gradient(float xHigh, float yHigh, float xLow, float yLow)
        {
            _high = new Math.Vector2(xHigh, yHigh);
            _low = new Math.Vector2(xLow, yLow);
            if (_high == _low) {
                throw new GrayscaleException("cannot compute gradient between the same two points");
            }
        }

        public override float Apply(float x, float y)
        {
            var p = new Math.Vector2(x, y);
            var proj = Math.Projection(p - _low, _high - _low);
            var ratio = Math.Norm(proj) / Math.Norm(_high - _low);
            return Math.Lerp(0, 1, ratio);
        }
    }
}
