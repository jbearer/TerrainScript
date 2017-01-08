namespace Ts.Grayscale.Generators
{
    public class Dome : PointwiseGenerator
    {
        private Math.Vector2 _center;
        private float        _radius;

        public Dome(float x, float y, float r)
        {
            _center = new Math.Vector2(x, y);
            _radius = r;
        }

        public override float Apply(float x, float y)
        {
            Math.Vector2 p = new Math.Vector2(x, y);
            float dome = System.Math.Max(_radius*_radius - Math.Dot(p - _center, p - _center), 0);

            // Scale to amplitude of 1
            return dome / (_radius * _radius);
        }
    }
}
