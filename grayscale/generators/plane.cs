using Ts.Grayscale.Absyn;

namespace Ts.Grayscale.Generators
{
    [Generator("plane")]
    public class Plane : PointwiseGenerator
    {
        public static Generator ParseArgs(ArgList _)
        {
            return new Plane();
        }

        protected override float Apply(float x, float y)
        {
            return 1;
        }
    }
}
