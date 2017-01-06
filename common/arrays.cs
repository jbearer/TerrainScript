namespace Ts
{
    public static class Arrays
    {
        public delegate void ElementModifier<T>(int row, int col, ref T val);

        public static void ForEach2D<T>(T[,] values, ElementModifier<T> f)
        {
            for (int row = 0; row < values.GetLength(0); row++) {
                for (int col = 0; col < values.GetLength(1); col++) {
                    f(row, col, ref values[row, col]);
                }
            }
        }
    }
}
