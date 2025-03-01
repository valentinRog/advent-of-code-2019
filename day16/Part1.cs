class Part1
{
    class Compute
    {
        class Pattern(int repeat)
        {
            static readonly int[] pattern = [0, 1, 0, -1];

            int i = 0;
            int k = 0;

            public int Next()
            {
                if (k == repeat)
                {
                    k = 0;
                    i++;
                    i %= pattern.Length;
                }
                k++;
                return pattern[i];
            }
        }

        public static int[] Next(int[] a)
        {
            List<int> l = [];
            foreach (var i in Enumerable.Range(0, a.Length))
            {
                Pattern pattern = new(i + 1);
                pattern.Next();
                var n = 0;
                foreach (var ii in Enumerable.Range(0, a.Length))
                {
                    n += a[ii] * pattern.Next();
                }
                l.Add(Math.Abs(n) % 10);
            }
            return [.. l];
        }
    }

    public static void Solve(string raw)
    {
        var a = raw.Select(c => c - '0').ToArray();
        foreach (var _ in Enumerable.Range(0, 100)) a = Compute.Next(a);
        Console.WriteLine(string.Join("", a[..8]));
    }
}
