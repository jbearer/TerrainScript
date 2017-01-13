using Ts.Grayscale.Absyn;

namespace Ts.Grayscale.Generators
{
    [Generator("gradient",
        RequiredArgs = new string[]{"xHigh", "yHigh", "xLow", "yLow"},
        RequiredArgTypes = new AstType[]{AstType.Float, AstType.Float, AstType.Float, AstType.Float})
    ]
    public class Gradient : PointwiseGenerator
    {
        private Math.Vector2 _high;
        private Math.Vector2 _low;

        public static Generator ParseArgs(ArgList args)
        {
            return new Gradient(
                args["xHigh"].AsFloat.Value,
                args["yHigh"].AsFloat.Value,
                args["xLow"].AsFloat.Value,
                args["yLow"].AsFloat.Value);
        }

        public Gradient(float xHigh, float yHigh, float xLow, float yLow)
        {
            _high = new Math.Vector2(xHigh, yHigh);
            _low = new Math.Vector2(xLow, yLow);
            if (_high == _low) {
                throw new GrayscaleException("cannot compute gradient between the same two points");
            }
        }

        protected override float Apply(float x, float y)
        {
            var p = new Math.Vector2(x, y);
            var proj = Math.Projection(p - _low, _high - _low);
            var ratio = Math.Norm(proj) / Math.Norm(_high - _low);
            return Math.Lerp(0, 1, ratio);
        }
    }
}
