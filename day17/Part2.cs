using System.Data;
using System.Diagnostics;
using System.Numerics;
using System.Text;

class Part2
{
    static void Run(Action a) => a();
    static T Run<T>(Func<T> f) => f();

    class Computer
    {
        readonly Dictionary<long, long> a;

        long i = 0;

        long relativeBase = 0;

        bool ended = false;
        public bool Ended { get => ended; }

        readonly Queue<long> inputs = new();

        public Computer(IEnumerable<long> data)
        {
            a = data.Select((e, i) => (e, i)).ToDictionary(e => (long)e.i, e => e.e);
        }

        public Computer(Computer other)
        {
            a = new Dictionary<long, long>(other.a);
            i = other.i;
            relativeBase = other.relativeBase;
            ended = other.ended;
            inputs = new Queue<long>(other.inputs);
        }

        public void SetRegister(long i, long n) => a[i] = n;

        public void AddInput(long n) => inputs.Enqueue(n);

        static long ExtractOpcode(long n) => n % 100;

        static long ExtractMode(long n, int offset) => n / (long)Math.Pow(10, offset + 1) % 10;

        long ExtractValue(int offset)
        {
            return ExtractMode(a[i], offset) switch
            {
                0 => a.GetValueOrDefault(a.GetValueOrDefault(i + offset)),
                1 => a.GetValueOrDefault(i + offset),
                2 => a.GetValueOrDefault(relativeBase + a.GetValueOrDefault(i + offset)),
                _ => throw new UnreachableException(),
            };
        }

        long ExtractAddress(int offset)
        {
            return ExtractMode(a[i], offset) switch
            {
                0 => a.GetValueOrDefault(i + offset),
                2 => relativeBase + a.GetValueOrDefault(i + offset),
                _ => throw new UnreachableException(),
            };
        }

        public long? Step()
        {
            long? output = null;
            Run(ExtractOpcode(a.GetValueOrDefault(i)) switch
            {
                1 => () =>
                {
                    a[ExtractAddress(3)] = ExtractValue(1) + ExtractValue(2);
                    i += 4;
                }
                ,
                2 => () =>
                {
                    a[ExtractAddress(3)] = ExtractValue(1) * ExtractValue(2);
                    i += 4;
                }
                ,
                3 => () =>
                {
                    a[ExtractAddress(1)] = inputs.Dequeue();
                    i += 2;
                }
                ,
                4 => () =>
                {
                    output = ExtractValue(1);
                    i += 2;
                }
                ,
                5 => () =>
                {
                    if (ExtractValue(1) != 0)
                    {
                        i = ExtractValue(2);
                    }
                    else
                    {
                        i += 3;
                    }
                }
                ,
                6 => () =>
                {
                    if (ExtractValue(1) == 0)
                    {
                        i = ExtractValue(2);
                    }
                    else
                    {
                        i += 3;
                    }
                }
                ,
                7 => () =>
                {
                    a[ExtractAddress(3)] = ExtractValue(1) < ExtractValue(2) ? 1 : 0;
                    i += 4;
                }
                ,
                8 => () =>
                {
                    a[ExtractAddress(3)] = ExtractValue(1) == ExtractValue(2) ? 1 : 0;
                    i += 4;
                }
                ,
                9 => () =>
                {
                    relativeBase += ExtractValue(1);
                    i += 2;
                }
                ,
                _ => throw new UnreachableException(),
            });
            if (a.GetValueOrDefault(i) == 99) ended = true;
            return output;
        }

        public long? ComputeUntilNextOuput()
        {
            long? res = null;
            while (!res.HasValue && !ended) res = Step();
            return res;
        }

        public bool NeedInput() => !ended && inputs.Count == 0 && ExtractOpcode(a.GetValueOrDefault(i)) == 3;
    }

    class RoutineBuilder
    {
        readonly Dictionary<Complex, char> m;
        readonly string[] path;
        readonly string rawInstructions;
        public string RawInstructions { get => rawInstructions; }

        public RoutineBuilder(Dictionary<Complex, char> m)
        {
            this.m = m;
            path = MakePath();
            rawInstructions = Backtracking(0, [], [])!;
        }

        string[] MakePath()
        {
            List<string> l = [];
            var z = m.First(kv => kv.Value == '^').Key;
            Complex d = new(0, -1);
            int acc = 0;
            while (true)
            {
                if (m.GetValueOrDefault(z + d) == '#')
                {
                    z += d;
                    acc++;
                    continue;
                }
                var res = new (Complex, string)[] {
                    (d * new Complex(0, -1), "L"),
                    (d * new Complex(0, 1), "R")
                }.Where(e => m.GetValueOrDefault(z + e.Item1) == '#');
                if (!res.Any()) break;
                if (acc > 0) l.Add(acc.ToString());
                acc = 0;
                var (dd, w) = res.First();
                d = dd;
                l.Add(w);
            }
            if (acc > 0) l.Add(acc.ToString());
            return [.. l];
        }

        string? Backtracking(int i, string[][] fs, int[] arr)
        {
            if (arr.Length * 2 - 1 > 20) return null;
            if (i == path.Length)
            {
                var sb = new StringBuilder();
                sb.Append(string.Join(',', arr.Select(i => (char)('A' + i))) + "\n");
                foreach (var f in fs) sb.Append(string.Join(',', f) + "\n");
                return sb.ToString();
            }
            foreach (var (f, index) in fs.Select((e, i) => (e, i)))
            {
                if (f.Length * 2 - 1 > 20) return null;
                if (i + f.Length > path.Length) continue;
                if (path[i..(i + f.Length)].SequenceEqual(f))
                {
                    var res = Backtracking(i + f.Length, fs, [.. arr, index]);
                    if (res is not null) return res;
                }
            }
            if (fs.Length == 3) return null;
            for (int j = i + 1; j <= path.Length; j++)
            {
                var res = Backtracking(i, [.. fs, path[i..j]], arr);
                if (res is not null) return res;
            }
            return null;
        }
    }

    public static void Solve(string raw)
    {
        Computer computer = new(raw.Split(',').Select(long.Parse));
        var sb = new StringBuilder();
        while (true)
        {
            var n = computer.ComputeUntilNextOuput();
            if (n is null) break;
            sb.Append((char)n);
        }
        Dictionary<Complex, char> m = [];
        foreach (var (line, y) in sb.ToString().Split('\n').Select((line, i) => (line, i)))
        {
            foreach (var (c, x) in line.Select((c, i) => (c, i))) m[new(x, y)] = c;
        }
        RoutineBuilder routineBuilder = new(m);
        computer = new(raw.Split(',').Select(long.Parse));
        computer.SetRegister(0, 2);
        foreach (var c in routineBuilder.RawInstructions + "n\n") computer.AddInput(c);
        long res = Run(() =>
        {
            while (true)
            {
                var c = (long)computer.ComputeUntilNextOuput()!;
                if (c > 127) return c;
            }
        });
        Console.WriteLine(res);
    }
}
