using System.Diagnostics;
using System.Numerics;

class Part2
{
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
            switch (ExtractOpcode(a.GetValueOrDefault(i)))
            {
                case 1:
                    a[ExtractAddress(3)] = ExtractValue(1) + ExtractValue(2);
                    i += 4;
                    break;
                case 2:
                    a[ExtractAddress(3)] = ExtractValue(1) * ExtractValue(2);
                    i += 4;
                    break;
                case 3:
                    a[ExtractAddress(1)] = inputs.Dequeue();
                    i += 2;
                    break;
                case 4:
                    var output = ExtractValue(1);
                    i += 2;
                    return output;
                case 5:
                    if (ExtractValue(1) != 0)
                    {
                        i = ExtractValue(2);
                    }
                    else
                    {
                        i += 3;
                    }
                    break;
                case 6:
                    if (ExtractValue(1) == 0)
                    {
                        i = ExtractValue(2);
                    }
                    else
                    {
                        i += 3;
                    }
                    break;
                case 7:
                    a[ExtractAddress(3)] = ExtractValue(1) < ExtractValue(2) ? 1 : 0;
                    i += 4;
                    break;
                case 8:
                    a[ExtractAddress(3)] = ExtractValue(1) == ExtractValue(2) ? 1 : 0;
                    i += 4;
                    break;
                case 9:
                    relativeBase += ExtractValue(1);
                    i += 2;
                    break;
            }
            if (a.GetValueOrDefault(i) == 99) ended = true;
            return null;
        }

        public long? ComputeUntilNextOuput()
        {
            long? res = null;
            while (!res.HasValue && !ended) res = Step();
            return res;
        }

        public bool NeedInput() => !ended && inputs.Count == 0 && ExtractOpcode(a.GetValueOrDefault(i)) == 3;
    }



    class Solver(IEnumerable<long> prog)
    {
        readonly Dictionary<Complex, bool> cache = [];

        public bool IsAffected(int x, int y)
        {
            if (cache.ContainsKey(new(x, y))) return cache[new(x, y)];
            Computer computer = new(prog);
            computer.AddInput(x);
            computer.AddInput(y);
            var res = computer.ComputeUntilNextOuput() == 1;
            cache[new(x, y)] = res;
            return res;
        }

        static readonly int w = 100;

        public bool Is100Square(int x, int y)
        {
            return IsAffected(x, y)
                && IsAffected(x + w - 1, y)
                && IsAffected(x + w - 1, y + w - 1)
                && IsAffected(x, y + w - 1);
        }
    }

    public static void Solve(string raw)
    {
        Solver solver = new(raw.Split(',').Select(long.Parse));
        int res = Run(() =>
        {
            var x0 = 0;
            for (var y = 100; ; y++)
            {
                for (int x = x0; ; x++)
                {
                    if (x > 0 && !solver.IsAffected(x - 1, y) && solver.IsAffected(x, y)) x0 = x - 1;
                    if (solver.Is100Square(x, y)) return 10_000 * x + y;
                    if (solver.IsAffected(x, y) && !solver.IsAffected(x + 1, y)) break;
                }
            }
        });
        Console.WriteLine(res);
    }
}
