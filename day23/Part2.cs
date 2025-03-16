using System.Diagnostics;

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
        public bool HasInputs { get => inputs.Count > 0; }

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


    static bool WillProduceOutput(Computer computer)
    {
        while (!computer.NeedInput())
        {
            var n = computer.Step();
            if (n is not null) return true;
        }
        return false;
    }

    public static void Solve(string raw)
    {
        var prog = raw.Split(',').Select(long.Parse);
        var network = Enumerable.Range(0, 50).Select(i =>
        {
            Computer computer = new(prog);
            computer.AddInput(i);
            return computer;
        }).ToArray();
        bool[] idle = [.. network.Select(_ => false)];
        long natX = 0;
        long natY = 0;
        var res = Run(() =>
        {
            long? prevNatY = null;
            while (true)
            {
                if (network.All(c => !c.HasInputs) && network.All(c => !WillProduceOutput(new(c))))
                {
                    network[0].AddInput(natX);
                    network[0].AddInput(natY);
                    if (natY == prevNatY) return natY;
                    prevNatY = natY;
                }
                for (var i = 0; i < network.Length; i++)
                {
                    var computer = network[i];
                    if (computer.NeedInput())
                    {
                        computer.AddInput(-1);
                        idle[i] = true;
                    }
                    var n = computer.Step();
                    if (n is null) continue;
                    idle[i] = false;
                    var x = (long)computer.ComputeUntilNextOuput()!;
                    var y = (long)computer.ComputeUntilNextOuput()!;
                    if (n == 255)
                    {
                        natX = x;
                        natY = y;
                    }
                    else
                    {
                        network[(int)n].AddInput(x);
                        network[(int)n].AddInput(y);
                    }
                }
            }
        });
        Console.WriteLine(res);
    }
}
