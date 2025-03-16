using System.Diagnostics;
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

        readonly Dictionary<Complex, char> m0;
        Dictionary<int, Dictionary<Complex, char>> grids;
        readonly int xyMax;
        readonly Complex zCenter;

        public Solver(Dictionary<Complex, char> m)
        {
            xyMax = (int)m.Keys.MaxBy(z => z.Magnitude).Real;
            zCenter = new(xyMax / 2, xyMax / 2);
            m.Remove(zCenter);
            m0 = m;
            grids = [];
            grids[0] = m;
            grids[-1] = MakeEmptyGrid();
            grids[1] = MakeEmptyGrid();
            foreach (var _ in Enumerable.Range(0, 200)) NextGrid();
        }

        public int BugCount() => grids.Values.Select(m => m.Values.Count(c => c == '#')).Sum();

        Dictionary<Complex, char> MakeEmptyGrid() => m0.ToDictionary(kv => kv.Key, _ => '.');

        int NeighborCount(int i, Complex z, Complex d)
        {
            if (grids[i].ContainsKey(z + d)) return grids[i][z + d] == '#' ? 1 : 0;
            if (z + d != zCenter)
            {
                if (!grids.ContainsKey(i + 1)) return 0;
                return grids[i + 1][zCenter + d] == '#' ? 1 : 0;
            }
            if (!grids.ContainsKey(i - 1)) return 0;
            Func<KeyValuePair<Complex, char>, bool> f = d switch
            {
                var dd when dd == new Complex(0, -1) => kv => kv.Key.Imaginary == xyMax,
                var dd when dd == new Complex(1, 0) => kv => kv.Key.Real == 0,
                var dd when dd == new Complex(0, 1) => kv => kv.Key.Imaginary == 0,
                var dd when dd == new Complex(-1, 0) => kv => kv.Key.Real == xyMax,
                _ => throw new UnreachableException(),
            };
            return grids[i - 1].Where(f).Count(kv => kv.Value == '#');
        }

        void NextGrid()
        {
            Dictionary<int, Dictionary<Complex, char>> grids = this
                .grids
                .ToDictionary(kv => kv.Key, kv => Next(kv.Key));
            grids[grids.Keys.Min() - 1] = MakeEmptyGrid();
            grids[grids.Keys.Max() + 1] = MakeEmptyGrid();
            this.grids = grids;
        }

        Dictionary<Complex, char> Next(int i)
        {
            Dictionary<Complex, char> m = [];
            foreach (var z in m0.Keys)
            {
                var nbCount = dirs.Select(d => NeighborCount(i, z, d)).Sum();
                switch ((grids[i][z], nbCount))
                {
                    case ('#', 1):
                        m[z] = '#';
                        break;
                    case ('#', _):
                        m[z] = '.';
                        break;
                    case ('.', var n) when n == 1 || n == 2:
                        m[z] = '#';
                        break;
                    case ('.', _):
                        m[z] = '.';
                        break;
                }
            }
            return m;
        }
    }

    public static void Solve(string raw)
    {
        Solver solver = new(Parse(raw));
        Console.WriteLine(solver.BugCount());
    }
}
