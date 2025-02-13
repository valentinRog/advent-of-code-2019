using System.Diagnostics;
using System.Numerics;

class Part2
{
    class Wire
    {
        Complex z = Complex.Zero;
        int k = 0;
        readonly Dictionary<Complex, int> path = new() { { Complex.Zero, 0 } };

        public Complex ClosestIntersection(Wire other)
        {
            return path
                .Keys
                .Intersect(other.path.Keys)
                .Where(z => z != Complex.Zero)
                .Select(z => path[z] + other.path[z])
                .Min();
        }

        public void Step(string s)
        {
            var d = s[0] switch
            {
                'U' => new Complex(0, -1),
                'R' => new Complex(1, 0),
                'D' => new Complex(0, 1),
                'L' => new Complex(-1, 0),
                _ => throw new UnreachableException(),
            };
            var n = int.Parse(s[1..]);
            foreach (var _ in Enumerable.Range(0, n))
            {
                z += d;
                k++;
                if (!path.ContainsKey(z)) path[z] = k;
            }
        }
    }

    public static void Solve(string raw)
    {
        Wire[] a = [new(), new()];
        foreach (var (wire, i) in a.Select((w, i) => (w, i)))
        {
            foreach (var s in raw.Split('\n')[i].Split(',')) wire.Step(s);
        }
        var z = a[0].ClosestIntersection(a[1]);
        Console.WriteLine(z.Real + z.Imaginary);
    }
}
