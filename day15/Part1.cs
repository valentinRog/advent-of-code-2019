using System.Diagnostics;
using System.Numerics;

class Part1
{
    static void Run(Action a) => a();

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

    class Solver
    {
        record State(Complex Z, int N, Computer Computer);

        static readonly Dictionary<Complex, int> dirToCode = new()
        {
            { new (0, -1), 1 },
            { new (0, 1), 2 },
            { new (-1, 0), 3 },
            { new (1, 0), 4 },
        };

        public static int BFS(IEnumerable<long> prog)
        {
            var cache = new HashSet<Complex> { Complex.Zero };
            var q = new Queue<State>([new(Complex.Zero, 0, new(prog))]);
            while (true)
            {
                var state = q.Dequeue();
                var z = state.Z;
                var k = state.N;
                var computer = state.Computer;
                foreach (var d in dirToCode.Keys.Where(d => !cache.Contains(z + d)))
                {
                    Computer newComputer = new(computer);
                    newComputer.AddInput(dirToCode[d]);
                    var n = newComputer.ComputeUntilNextOuput();
                    if (n == 2) return k + 1;
                    cache.Add(z + d);
                    if (n == 1) q.Enqueue(new(z + d, k + 1, newComputer));
                }
            }
        }
    }

    public static void Solve(string raw)
    {
        var res = Solver.BFS(raw.Split(',').Select(long.Parse));
        Console.WriteLine(res);
    }
}
