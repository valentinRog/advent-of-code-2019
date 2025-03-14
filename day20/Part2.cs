using System.Numerics;

class Part2
{
    static Dictionary<Complex, char> Parse(string s)
    {
        Dictionary<Complex, char> m = [];
        foreach (var (line, y) in s.ToString().Split('\n').Select((line, i) => (line, i)))
        {
            foreach (var (c, x) in line.Select((c, i) => (c, i))) m[new(x, y)] = c;
        }
        return m;
    }

    class Solver
    {
        static readonly Complex[] dirs = [new(0, -1), new(1, 0), new(0, 1), new(-1, 0)];
        readonly Dictionary<Complex, char> m;
        readonly Complex zMin;
        readonly Complex zMax;
        readonly Dictionary<Complex, string> portalNames;
        readonly Complex z0;
        readonly Complex z1;
        readonly Dictionary<Complex, Complex> portals = [];
        readonly int res;
        public int Res { get => res; }

        public Solver(Dictionary<Complex, char> m)
        {
            this.m = m;
            {
                var seq = m.Where(kv => kv.Value == '#').Select(kv => kv.Key);
                zMin = seq.MinBy(z => z.Magnitude);
                zMax = seq.MaxBy(z => z.Magnitude);
            }
            portalNames = MakePortalNames();
            foreach (var kv in portalNames)
            {
                var z = kv.Key;
                var s = kv.Value;
                if (s == "AA")
                {
                    z0 = z;
                }
                else if (s == "ZZ")
                {
                    z1 = z;
                }
                else
                {
                    portals[z] = portalNames.First(kv => kv.Value == s && kv.Key != z).Key;
                }
            }
            res = BFS();
        }

        Dictionary<Complex, string> MakePortalNames()
        {
            Dictionary<Complex, string> portals = [];
            foreach (var z in m.Where(kv => kv.Value == '.').Select(kv => kv.Key))
            {
                foreach (var d in dirs.Where(d => char.IsAsciiLetter(m[z + d])))
                {
                    if (d.Imaginary == -1 || d.Real == -1)
                    {
                        portals[z] = $"{m[z + 2 * d]}{m[z + d]}";
                    }
                    else
                    {
                        portals[z] = $"{m[z + d]}{m[z + 2 * d]}";
                    }
                }
            }
            return portals;
        }

        bool IsOuter(Complex z)
        {
            return z.Real == zMin.Real
                || z.Real == zMax.Real
                || z.Imaginary == zMin.Imaginary
                || z.Imaginary == zMax.Imaginary;
        }

        record Node(Complex Z, int Cost, int Level);
        record CacheKey(Complex Z, int Level);
        int BFS()
        {
            Queue<Node> q = new();
            q.Enqueue(new(z0, 0, 0));
            HashSet<CacheKey> cache = [];
            cache.Add(new(z0, 0));
            while (true)
            {
                var node = q.Dequeue();
                var z = node.Z;
                if (z == z1 && node.Level == 0) return node.Cost;
                foreach (var d in dirs.Where(d => m[z + d] == '.' && !cache.Contains(new(z + d, node.Level))))
                {
                    q.Enqueue(new(z + d, node.Cost + 1, node.Level));
                    cache.Add(new(z + d, node.Level));
                }
                if (portals.TryGetValue(z, out var zz))
                {
                    var level = node.Level + (IsOuter(z) ? -1 : 1);
                    if (level == -1) continue;
                    if (cache.Contains(new(zz, level))) continue;
                    q.Enqueue(new(zz, node.Cost + 1, level));
                    cache.Add(new(zz, level));
                }
            }
        }
    }

    public static void Solve(string raw)
    {
        Solver solver = new(Parse(raw));
        Console.WriteLine(solver.Res);
    }
}
