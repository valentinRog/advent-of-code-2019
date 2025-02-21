using System.Numerics;

class Part1
{
    static int GCD(int a, int b)
    {
        while (b != 0)
        {
            int c = b;
            b = a % b;
            a = c;
        }
        return a;
    }

    static int CountAsteroid(Dictionary<Complex, char> m, Complex z)
    {
        var hs = new HashSet<Complex>(m.Where(kv => kv.Value == '#').Select(kv => kv.Key));
        hs.Remove(z);
        var res = 0;
        for (; hs.Count > 0; res++)
        {
            var z1 = hs.MinBy(e => (e - z).Magnitude);
            var d = z1 - z;
            var gcd = Math.Abs(GCD((int)d.Real, (int)d.Imaginary));
            d = new(d.Real / gcd, d.Imaginary / gcd);
            for (var zz = z1; m.ContainsKey(zz); zz += d) hs.Remove(zz);
        }
        return res;
    }

    public static void Solve(string raw)
    {
        var m = new Dictionary<Complex, char>();
        foreach (var (line, y) in raw.Split("\n").Select((e, i) => (e, i)))
        {
            foreach (var (c, x) in line.Select((c, i) => (c, i)))
            {
                m[new(x, y)] = c;
            }
        }
        var res = m.Where(kv => kv.Value == '#').Select(kv => CountAsteroid(m, kv.Key)).Max();
        Console.WriteLine(res);
    }
}
