/**
 * The Perlin noise algorithm was created by Ken Perlin. The implementation here is adapted from
 * http://flafla2.github.io/2014/08/09/perlinnoise.html.
 */

namespace Ts.Grayscale.Generators
{
    public class Perlin : PointwiseGenerator
    {
        // Hash lookup table as defined by Ken Perlin. This is a randomly arranged array of all
        // numbers from 0-255 inclusive.
        private static readonly byte[] permutation = {
            151,160,137,91,90,15,131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,
            10,23,190,6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,88,237,149,
            56,87,174,20,125,136,171,168,68,175,74,165,71,134,139,48,27,166,77,146,158,231,83,111,229,
            122,60,211,133,230,220,105,92,41,55,46,245,40,244,102,143,54,65,25,63,161,1,216,80,73,209,
            76,132,187,208,89,18,169,200,196,135,130,116,188,159,86,164,100,109,198,173,186,3,64,52,217,
            226,250,124,123,5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152,2,44,154,163,70,221,153,101,155,167,43,172,9,129,22,39,253,19,
            98,108,110,79,113,224,232,178,185,112,104,218,246,97,228,251,34,242,193,238,210,144,12,191,
            179,162,241,81,51,145,235,249,14,239,107,49,192,214,31,181,199,106,157,184,84,204,176,115,
            121,50,45,127, 4,150,254,138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,
            156,180
        };

        // Doubled permutation to avoid overflow
        private static readonly byte[] p;

        static Perlin()
        {
            p = new byte[512];
            for(int x = 0; x < 512; x++) {
                p[x] = permutation[x % 256];
            }
        }

        private float _amplitude;
        private float _frequency;
        private int _octaves;
        private float _persistence;

        public Perlin(float amplitude, float frequency = 1, int octaves = 1, float persistence = 0.5f)
        {
            _amplitude = amplitude;
            _frequency = frequency;
            _octaves = octaves;
            _persistence =persistence;
        }

        public override float Apply(float x, float y)
        {
            float total = 0;
            float A = 1;                   // Amplitude starts at 1, we'll normalize at the end
            float w = _frequency / 255;
            float maxValue = 0;            // Used for normalizing result to 0.0 - 1.0

            for(int i = 0; i < _octaves; i++) {
                total += A * getPoint(w * x, w * y);

                maxValue += A;

                A *= _persistence;
                w *= 2;
            }

            return total * _amplitude / maxValue;
        }

        private float getPoint(float x, float y)
        {
            // Truncate to fit within the range [0, 1]
            x = x - (int)x;
            y = y - (int)y;

            /**
             * Calculate the unit square in which the point will be located. We convert to an
             * integral 0 - 255 coordinate system so that we can do look-ups in our 255-entry table.
             * The point (xi, yi) is the lower left corner of the square. The other points are
             * (xi + 1, yi), (xi, yi + 1), and (xi + 1, yi + 1).
             */
            int xi = (int)(x * 255);
            int yi = (int)(y * 255);

            // Calculate the position of the point (x, y) relative to the unit square
            float xf = (x * 255) - xi;
            float yf = (y * 255) - yi;

            // Apply a filter to the location to smooth the result
            float u = fade(xf);
            float v = fade(yf);

            // Hash the corners of the square
            byte bottomLeft  = hash(xi, yi);
            byte topLeft     = hash(xi, yi + 1);
            byte bottomRight = hash(xi + 1, yi);
            byte topRight    = hash(xi + 1, yi + 1);

            // Find the influence of each gradient vector, and then take a weighted average based on
            // the coordinates of the point within the unit square.
            float bottom = Math.Lerp(grad(bottomLeft, xf, yf),
                                 grad(bottomRight, xf - 1, yf),
                                 u);
            float top = Math.Lerp(grad(topLeft, xf, yf-1),
                              grad(topRight, xf-1, yf-1),
                              u);
            float result = Math.Lerp(bottom, top, v);

            // Bound it to 0 - 1 (theoretical min/max before is -1 - 1)
            return (result + 1) / 2;
        }

        private static byte hash(int x, int y)
        {
            return p[ p[x] + y ];
        }

        /**
         * Calculate the dot product between a pseudorandom gradient vector and the vector from the
         * input coordinate to each of the 4 corners of its containing unit square. The gradient
         * vector is chosen to be one of the vectors from a point in the center of a square to one
         * of the edges of the square, ie (1, 0), (0, 1), (-1, 0), and (0, -1).
         *
         * Since we dot one of these vectors with the point (x, y), the actual value returned will
         * be one of x, y, -x, or -y.
         */
        private static float grad(byte hash, float x, float y) {
            // The first bit chooses between x and y
            float result = ((hash & 2) == 0) ? x : y;

            // The second bit chooses the sign
            return ((hash & 1) == 0) ? result : -result;
        }

        /**
         * Fade function as defined by Ken Perlin. This eases coordinate valuesso that they will
         * ease towards integral values. This ends up smoothing the final output.
         */
        private static float fade(float t) {
            // 6t^5 - 15t^4 + 10t^3
            return t * t * t * (t * (t * 6 - 15) + 10);
        }
    }
}
