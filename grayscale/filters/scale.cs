using System.Collections.Generic;

namespace Ts.Grayscale.Filters
{
    public class Scale : FixedArityFilter
    {
        private float _factor;

        public Scale(float factor)
            : base(1)
        {
            _factor = factor;
        }

        protected override Generator Apply(List<Generator> gs)
        {
            return new LambdaGenerator((x, y) => _factor * gs[0].GenerateValue(x, y));
        }
    }
}
