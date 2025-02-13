class Part1
{
    class Orbits(Dictionary<string, string> m)
    {
        readonly Dictionary<string, string> m = m;

        int CountOrbits(string s)
        {
            if (!m.TryGetValue(s, out string? v)) return 0;
            return 1 + CountOrbits(v);
        }

        public int CountOrbits() => m.Keys.Select(e => CountOrbits(e)).Sum();
    };

    public static void Solve(string raw)
    {
        var m = raw
            .Split('\n')
            .Select(e => e.Split(')'))
            .ToDictionary(e => e[1], e => e[0]);
        Orbits orbits = new(m);
        Console.WriteLine(orbits.CountOrbits());
    }
}
