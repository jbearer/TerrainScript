using Ts.Grayscale.Absyn;

namespace Ts.Grayscale.Filters
{
    [Filter("invert")]
    public class Invert : UnaryFilter
    {
        public static Filter ParseArgs(ArgList _)
        {
            return new Invert();
        }

        protected override Generator Apply(Generator g)
        {
            return new LambdaGenerator((x, y) => 1 - g.GenerateValue(x, y));
        }
    }
}
