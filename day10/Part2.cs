using System.Numerics;

class Part2
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

    static Complex ReduceVector(Complex d)
    {
        var gcd = Math.Abs(GCD((int)d.Real, (int)d.Imaginary));
        return new(d.Real / gcd, d.Imaginary / gcd);
    }

    static double Angle(Complex z1, Complex z2)
    {
        return (z1.Real * z2.Real + z1.Imaginary * z2.Imaginary) / (z1.Magnitude * z2.Magnitude);
    }

    static double CrossProduct(Complex z1, Complex z2)
    {
        return z1.Real * z2.Imaginary - z1.Imaginary * z2.Real;
    }

    static double OrientedAngle(Complex z1, Complex z2)
    {
        return Math.Sign(CrossProduct(z1, z2)) * Angle(z1, z2);
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

    static Complex FindBestAsteroid(Dictionary<Complex, char> m)
    {
        return m.Where(kv => kv.Value == '#').Select(kv => kv.Key).MaxBy(z => CountAsteroid(m, z));
    }

    class Laser(Dictionary<Complex, char> m, Complex z0)
    {
        readonly HashSet<Complex> asteroids = [
            .. m.Where(kv => kv.Value == '#' && kv.Key != z0).Select(kv => kv.Key)
            ];

        readonly Complex z0 = z0;

        Complex d = new(-1, -1000);

        public Complex DestroyNext()
        {
            d = ReduceVector(asteroids.Where(z => Angle(z - z0, d) > 0).MinBy(z => OrientedAngle(z - z0, d)) - z0);
            var z = z0;
            while (!asteroids.Contains(z)) z += d;
            asteroids.Remove(z);
            return z;
        }
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
        var laser = new Laser(m, FindBestAsteroid(m));

        Complex z = new();
        for (var i = 0; i < 200; i++) z = laser.DestroyNext();
        var res = 100 * z.Real + z.Imaginary;
        Console.WriteLine(res);
    }
}
