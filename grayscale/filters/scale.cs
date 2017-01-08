using System.Collections.Generic;

namespace Ts.Grayscale.Filters
{
    public class Scale : UnaryFilter
    {
        private float _factor;

        public Scale(float factor)
        {
            _factor = factor;
        }

        protected override Generator Apply(Generator g)
        {
            return new LambdaGenerator((x, y) => _factor * g.GenerateValue(x, y));
        }
    }
}
