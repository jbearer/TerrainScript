using System.Collections.Generic;

namespace Ts.Grayscale.Filters
{
    public class Invert : UnaryFilter
    {
        protected override Generator Apply(Generator g)
        {
            return new LambdaGenerator((x, y) => 1 - g.GenerateValue(x, y));
        }
    }
}
