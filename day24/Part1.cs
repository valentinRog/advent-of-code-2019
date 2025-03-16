using System.Numerics;
using System.Text;

class Part1
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
        Dictionary<Complex, char> m;
        readonly Complex zMax;

        public Solver(Dictionary<Complex, char> m)
        {
            this.m = m;
            zMax = m.Keys.MaxBy(z => z.Magnitude);
            ComputeUntilRepetition();
        }

        string Serialize()
        {
            var sb = new StringBuilder();
            for (var y = 0; y <= zMax.Imaginary; y++)
            {
                for (var x = 0; x <= zMax.Real; x++) sb.Append(m[new(x, y)]);
                sb.Append('\n');
            }
            return sb.ToString();
        }

        void Next()
        {
            Dictionary<Complex, char> m = [];
            foreach (var kv in this.m)
            {
                var nbCount = dirs.Count(d => this.m.GetValueOrDefault(kv.Key + d) == '#');
                switch ((kv.Value, nbCount))
                {
                    case ('#', 1):
                        m[kv.Key] = '#';
                        break;
                    case ('#', _):
                        m[kv.Key] = '.';
                        break;
                    case ('.', var n) when n == 1 || n == 2:
                        m[kv.Key] = '#';
                        break;
                    case ('.', _):
                        m[kv.Key] = '.';
                        break;
                }
            }
            this.m = m;
        }

        void ComputeUntilRepetition()
        {
            HashSet<string> cache = [Serialize()];
            while (true)
            {
                Next();
                var s = Serialize();
                if (cache.Contains(s)) break;
                cache.Add(s);
            }
        }

        public int BiodiversityRating()
        {
            return m.Where(kv => kv.Value == '#')
                .Select(kv => kv.Key)
                .Select(z => (int)Math.Pow(2, z.Real + z.Imaginary * (zMax.Real + 1)))
                .Sum();
        }
    }

    public static void Solve(string raw)
    {
        Solver solver = new(Parse(raw));
        Console.WriteLine(solver.BiodiversityRating());
    }
}
