using Ts.Grayscale.Absyn;

namespace Ts.Grayscale.Filters
{
    [Filter("scale",
        RequiredArgs = new string[]{ "factor" },
        RequiredArgTypes = new AstType[]{ AstType.Float })
    ]
    public class Scale : UnaryFilter
    {
        private float _factor;

        public static Filter ParseArgs(ArgList args)
        {
            return new Scale(args["factor"].AsFloat.Value);
        }

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
