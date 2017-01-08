namespace Ts
{
    public static class Math
    {
        public class MathException : System.Exception
        {
            public MathException(string message)
                : base(message)
            {
            }
        }

        public class Vector2
        {
            public float X;
            public float Y;

            public Vector2(float x, float y)
            {
                X = x;
                Y = y;
            }

            public override bool Equals(object other)
            {
                if (!(other is Vector2)) {
                    return false;
                }

                Vector2 v = (Vector2)other;
                return Equal(X, v.X) && Equal(Y, v.Y);
            }

            public override int GetHashCode()
            {
                return X.GetHashCode() ^ Y.GetHashCode();
            }

            public static Vector2 operator +(Vector2 a, Vector2 b)
            {
                return new Vector2(a.X + b.X, a.Y + b.Y);
            }

            public static Vector2 operator -(Vector2 a)
            {
                return new Vector2(-a.X, -a.Y);
            }

            public static Vector2 operator -(Vector2 a, Vector2 b)
            {
                return a + (-b);
            }

            public static Vector2 operator *(float c, Vector2 v)
            {
                return new Vector2(c * v.X, c * v.Y);
            }

            public static Vector2 operator *(Vector2 v, float c)
            {
                return c * v;
            }

            public override string ToString()
            {
                return string.Format("({0}, {1})", X, Y);
            }

            public static bool operator ==(Vector2 a, Vector2 b)
            {
                return a.Equals(b);
            }

            public static bool operator !=(Vector2 a, Vector2 b)
            {
                return !(a == b);
            }
        }

        // Floating point comparison
        public static bool Equal(float a, float b)
        {
            return System.Math.Abs(a - b) < 0.0000001;
        }

        // Linear interpolation from a to b with ratio r
        public static float Lerp(float a, float b, float r)
        {
            return a + r * (b - a);
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float r)
        {
            return a + r * (b - a);
        }

        // Inverse interpolation
        public static float ILerp(Vector2 a, Vector2 b, Vector2 lerp)
        {
            if (a == b) {
                throw new MathException("Cannot invert linear interpolation: input is not a line.");
            }

            Vector2 total = b - a;
            Vector2 distance = lerp - a;

            if (total.X == 0) return distance.Y / total.Y;
            if (total.Y == 0) return distance.X / total.X;

            if (!Equal(distance.X / total.X, distance.Y / total.Y)) {
                throw new MathException(System.String.Format(
                    "{0} does not lie on the line {1} -> {2}", lerp, a, b));
            }
            return distance.X / total.X;
        }

        public static float Dot(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static Vector2 Projection(Vector2 a, Vector2 b)
        {
            return (Dot(a, b) / Dot(b, b)) * b;
        }
    }
}
