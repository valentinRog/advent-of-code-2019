class Part1
{
    class Layers
    {
        readonly List<char[]> layers = [];

        public Layers(int width, int height, string raw)
        {
            int windowSize = width * height;
            for (int i = 0; i < raw.Length; i += windowSize)
            {
                var a = new char[height * width];
                for (int ii = 0; ii < windowSize; ii++)
                {
                    a[ii] = raw[i + ii];
                    layers.Add(a);
                }
            }
        }

        public int Compute()
        {
            var m = layers
                .Select(e => e.GroupBy(e => e).ToDictionary(e => e.Key, e => e.Count()))
                .MinBy(e => e['0'])!;
            return m['1'] * m['2'];
        }
    }

    public static void Solve(string raw)
    {
        Layers layers = new(25, 6, raw);
        var res = layers.Compute();
        Console.WriteLine(res);
    }
}
