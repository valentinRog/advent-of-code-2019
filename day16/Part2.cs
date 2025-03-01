class Part2
{
    class Compute
    {
        public static int[] Next(int[] a)
        {
            Dictionary<int, int> m = [];
            {
                var i = a.Length - 1;
                var acc = 0;
                foreach (var n in a.Reverse())
                {
                    acc += n;
                    acc %= 10;
                    m[i] = acc;
                    i--;
                }
            }
            List<int> l = [];
            foreach (var i in Enumerable.Range(0, a.Length)) l.Add(m[i]);
            return [.. l];
        }
    }

    public static void Solve(string raw)
    {
        int offset = int.Parse(raw[..7]);
        var a0 = raw.Select(c => c - '0').ToArray();
        List<int> l = [];
        foreach (var _ in Enumerable.Range(0, 10_000)) l.AddRange(a0);
        int[] a = [.. l[offset..]];
        foreach (var i in Enumerable.Range(0, 100))
        {
            a = Compute.Next(a);
        }
        Console.WriteLine(string.Join("", a[..8]));
    }
}
