using System.Numerics;

class Part1
{
    class MoonSystem(IEnumerable<Vector3> moonPositions)
    {
        class Moon(Vector3 p)
        {
            Vector3 p = p;
            Vector3 v = Vector3.Zero;

            public void ApplyGravity(Moon other)
            {
                v.X += Math.Sign(other.p.X - p.X);
                v.Y += Math.Sign(other.p.Y - p.Y);
                v.Z += Math.Sign(other.p.Z - p.Z);
            }

            public void ApplySpeed() => p += v;

            public int Energy()
            {
                return (int)(
                    (Math.Abs(p.X) + Math.Abs(p.Y) + Math.Abs(p.Z)) *
                    (Math.Abs(v.X) + Math.Abs(v.Y) + Math.Abs(v.Z))
                );
            }
        }

        readonly Moon[] moons = [.. moonPositions.Select(p => new Moon(p))];

        public void Step()
        {
            foreach (var moon0 in moons)
            {
                foreach (var moon1 in moons) moon0.ApplyGravity(moon1);
            }
            foreach (var moon in moons) moon.ApplySpeed();
        }

        public int Energy() => moons.Select(e => e.Energy()).Sum();
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
        for (int i = 0; i < 1000; i++) system.Step();
        Console.WriteLine(system.Energy());
    }
}
