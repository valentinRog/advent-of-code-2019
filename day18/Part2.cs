using System.Numerics;
using System.Text;

class Part2
{
    class Solver
    {
        static readonly Complex[] dirs = [new(0, -1), new(1, 0), new(0, 1), new(-1, 0)];
        readonly Dictionary<Complex, char> m;
        readonly Dictionary<char, Complex> z0s = [];
        readonly Dictionary<char, Complex> keys;
        readonly Dictionary<char, Dictionary<char, int>> distances = [];
        readonly Dictionary<char, Dictionary<char, HashSet<char>>> doors = [];

        public Solver(Dictionary<Complex, char> m)
        {
            this.m = m;
            foreach (var kv in m.Where(kv => char.IsAsciiDigit(kv.Value)))
            {
                z0s[kv.Value] = kv.Key;
            }
            keys = m
                .Where(kv => char.IsAsciiDigit(kv.Value) || char.IsAsciiLetterLower(kv.Value))
                .ToDictionary(kv => kv.Value, kv => kv.Key);
            foreach (var c in keys.Keys)
            {
                distances[c] = [];
                doors[c] = [];
                foreach (var cc in keys.Keys)
                {
                    var path = Path(keys[c], keys[cc]);
                    if (path is null) continue;
                    doors[c][cc] = [];
                    foreach (var z in path)
                    {
                        if (char.IsAsciiLetterUpper(m[z])) doors[c][cc].Add(char.ToLower(m[z]));
                    }
                    distances[c][cc] = path.Length - 1;
                }
            }
            DFS(z0s.ToDictionary(), [.. z0s.Keys], 0, []);
        }

        readonly Dictionary<Complex, Dictionary<Complex, Complex[]>> paths = [];

        Complex[]? Path(Complex z0, Complex z1)
        {
            if (!paths.ContainsKey(z0)) paths[z0] = [];
            if (paths[z0].TryGetValue(z1, out var value)) return value;
            Queue<Complex[]> q = [];
            q.Enqueue([z0]);
            while (q.Count > 0)
            {
                var path = q.Dequeue();
                var z = path[^1];
                if (char.IsAsciiLetterLower(m[z]) && !paths[z0].ContainsKey(z))
                {
                    paths[z0][z] = path;
                    if (!paths.ContainsKey(z)) paths[z] = [];
                    paths[z][z0] = path;
                }
                if (z == z1) return path;
                foreach (var d in dirs)
                {
                    if (m[z + d] == '#') continue;
                    if (path.Contains(z + d)) continue;
                    Complex[] newPath = [.. path, z + d];
                    q.Enqueue(newPath);
                }
            }
            return null;
        }

        int res = int.MaxValue;
        public int Res { get => res; }
        readonly Dictionary<string, int> cache = [];

        void DFS(
            Dictionary<char, Complex> state,
            char[] path,
            int cost,
            HashSet<char> keys
        )
        {
            var key = Serialize(state, path);
            if (cache.TryGetValue(key, out int value) && value <= cost) return;
            cache[key] = cost;
            if (cost >= res) return;
            if (path.Length == this.keys.Count)
            {
                res = cost;
                return;
            }
            foreach (var kv in state)
            {
                var c = m[kv.Value];
                foreach (var cc in distances[c].Keys.Where(cc => !path.Contains(cc)))
                {
                    if (doors[c][cc].Any(e => !keys.Contains(e))) continue;
                    var newState = state.ToDictionary();
                    newState[kv.Key] = m.First(kv => kv.Value == cc).Key;
                    DFS(newState, [.. path, cc], cost + distances[c][cc], [.. keys, cc]);
                }
            }
        }

        string Serialize(Dictionary<char, Complex> state, char[] path)
        {
            char[] a = [.. path];
            Array.Sort(a);
            var sb = new StringBuilder();
            char[] ks = [.. z0s.Keys];
            Array.Sort(ks);
            foreach (var k in ks) sb.Append($"{m[state[k]]},");
            return $"{sb}{string.Join("", a)}";
        }
    }

    static Dictionary<Complex, char> Parse(string s)
    {
        Dictionary<Complex, char> m = [];
        foreach (var (line, y) in s.ToString().Split('\n').Select((line, i) => (line, i)))
        {
            foreach (var (c, x) in line.Select((c, i) => (c, i))) m[new(x, y)] = c;
        }
        return m;
    }

    public static void Solve(string raw)
    {
        var m = Parse(raw);
        var subM = Parse("0#1\n###\n2#3");
        var z = m.First(kv => kv.Value == '@').Key;
        foreach (var k in subM.Keys) m[k + z - new Complex(1, 1)] = subM[k];
        Solver solver = new(m);
        Console.WriteLine(solver.Res);
    }
}
