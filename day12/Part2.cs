using System.Numerics;

class Part2
{
    static long GCD(long a, long b)
    {
        while (b != 0)
        {
            long c = b;
            b = a % b;
            a = c;
        }
        return a;
    }

    static long LCM(long a, long b) => a / GCD(a, b) * b;

    class MoonSystem(IEnumerable<Vector3> moonPositions)
    {
        class Moon(Vector3 p)
        {
            Vector3 p = p;
            public Vector3 P { get => p; }
            Vector3 v = Vector3.Zero;
            public Vector3 V { get => v; }

            public void ApplyGravity(Moon other)
            {
                v.X += Math.Sign(other.p.X - p.X);
                v.Y += Math.Sign(other.p.Y - p.Y);
                v.Z += Math.Sign(other.p.Z - p.Z);
            }

            public void ApplySpeed() => p += v;
        }

        readonly Moon[] moons = [.. moonPositions.Select(p => new Moon(p))];

        void Step()
        {
            foreach (var moon0 in moons)
            {
                foreach (var moon1 in moons) moon0.ApplyGravity(moon1);
            }
            foreach (var moon in moons) moon.ApplySpeed();
        }

        int ComputePeriodForAxe(Func<Vector3, float> selector)
        {
            MoonSystem system = new(moonPositions);
            for (int i = 1; ; i++)
            {
                system.Step();
                if (
                    system
                        .moons
                        .Zip(moons)
                        .All(e =>
                            selector(e.First.P) == selector(e.Second.P) &&
                            selector(e.First.V) == selector(e.Second.V))
                )
                {
                    return i;
                }
            }
        }

        public long ComputePeriod()
        {
            var periodX = ComputePeriodForAxe(v => v.X);
            var periodY = ComputePeriodForAxe(v => v.Y);
            var periodZ = ComputePeriodForAxe(v => v.Z);
            return LCM(LCM(periodX, periodY), periodZ);
        }
    }

    public static void Solve(string raw)
    {
        foreach (var c in "<>=xyz,") raw = raw.Replace(c, ' ');
        var moonPositions = raw
            .Split('\n')
            .Select(line => line
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray())
            .Select(a => new Vector3(a[0], a[1], a[2]));
        var system = new MoonSystem(moonPositions);
        var res = system.ComputePeriod();
        Console.WriteLine(res);
    }
}