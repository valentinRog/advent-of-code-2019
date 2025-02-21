using System.Diagnostics;
using System.Numerics;

class Part2
{

    static void Run(Action a) => a();

    class Computer(IEnumerable<long> data)
    {
        readonly Dictionary<long, long> a = data
            .Select((e, i) => (e, i))
            .ToDictionary(e => (long)e.i, e => e.e);

        long i = 0;

        long relativeBase = 0;

        bool ended = false;
        public bool Ended { get => ended; }

        readonly Queue<long> inputs = new();

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
            while (!res.HasValue)
            {
                if (ended) return null;
                res = Step();
            }
            return res;
        }
    }

    public static void Dump(HashSet<Complex> white)
    {
        var x0 = white.Select(z => z.Real).Min();
        var x1 = white.Select(z => z.Real).Max();
        var y0 = white.Select(z => z.Imaginary).Min();
        var y1 = white.Select(z => z.Imaginary).Max();
        for (var y = y0; y <= y1; y++)
        {
            for (var x = x0; x <= x1; x++)
            {
                Console.Write(white.Contains(new(x, y)) ? '#' : ' ');
            }
            Console.WriteLine();
        }
    }

    public static void Solve(string raw)
    {
        Computer computer = new(raw.Split(',').Select(long.Parse));
        var white = new HashSet<Complex>();
        Complex d = new(0, -1);
        var z = Complex.Zero;
        white.Add(z);
        while (!computer.Ended)
        {
            computer.AddInput(white.Contains(z) ? 1 : 0);
            if (computer.ComputeUntilNextOuput() == 0)
            {
                white.Remove(z);
            }
            else
            {
                white.Add(z);
            }
            if (computer.Ended) break;
            d *= computer.ComputeUntilNextOuput() switch
            {
                0 => new Complex(0, -1),
                1 => new Complex(0, 1),
                _ => throw new UnreachableException(),
            };
            z += d;
        }
        Dump(white);
    }
}
