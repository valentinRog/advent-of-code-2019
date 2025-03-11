using System.Numerics;

class Part1
{
    class Solver
    {
        static readonly Complex[] dirs = [new(0, -1), new(1, 0), new(0, 1), new(-1, 0)];
        readonly Dictionary<Complex, char> m;
        readonly Dictionary<char, Complex> keys;
        readonly Dictionary<char, Dictionary<char, int>> distances = [];
        readonly Dictionary<char, Dictionary<char, HashSet<char>>> doors = [];

        public Solver(Dictionary<Complex, char> m)
        {
            this.m = m;
            keys = m
                .Where(kv => kv.Value == '@' || char.IsAsciiLetterLower(kv.Value))
                .ToDictionary(kv => kv.Value, kv => kv.Key);
            foreach (var c in keys.Keys)
            {
                distances[c] = [];
                doors[c] = [];
                foreach (var cc in keys.Keys)
                {
                    doors[c][cc] = [];
                    var path = Path(keys[c], keys[cc]);
                    foreach (var z in path)
                    {
                        if (char.IsAsciiLetterUpper(m[z])) doors[c][cc].Add(char.ToLower(m[z]));
                    }
                    distances[c][cc] = path.Length - 1;
                }
            }
            DFS(['@'], 0, []);
        }

        readonly Dictionary<Complex, Dictionary<Complex, Complex[]>> paths = [];

        Complex[] Path(Complex z0, Complex z1)
        {
            if (!paths.ContainsKey(z0)) paths[z0] = [];
            if (paths[z0].TryGetValue(z1, out var value)) return value;
            Queue<Complex[]> q = [];
            q.Enqueue([z0]);
            while (true)
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
        }

        int res = int.MaxValue;
        public int Res { get => res; }
        readonly Dictionary<string, int> cache = [];

        void DFS(char[] path, int cost, HashSet<char> keys)
        {
            var key = Serialize(path);
            if (cache.TryGetValue(key, out int value) && value <= cost) return;
            cache[key] = cost;
            if (cost >= res) return;
            if (path.Length == this.keys.Count)
            {
                res = cost;
                return;
            }
            var c = path[^1];
            foreach (var cc in distances[c].Keys.Where(cc => !path.Contains(cc)))
            {
                if (doors[c][cc].Any(e => !keys.Contains(e))) continue;
                DFS([.. path, cc], cost + distances[c][cc], [.. keys, cc]);
            }
        }

        static string Serialize(char[] path)
        {
            char[] a = [.. path];
            Array.Sort(a);
            return $"{path[^1]},{string.Join("", a)}";
        }
    }

    public static void Solve(string raw)
    {
        Dictionary<Complex, char> m = [];
        foreach (var (line, y) in raw.ToString().Split('\n').Select((line, i) => (line, i)))
        {
            foreach (var (c, x) in line.Select((c, i) => (c, i))) m[new(x, y)] = c;
        }
        Solver solver = new(m);
        Console.WriteLine(solver.Res);
    }
}
