using System.Diagnostics;

class Part2
{
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
    }

    public static void Solve(string raw)
    {
        Computer computer = new(raw.Split(',').Select(long.Parse));
        computer.AddInput(2);
        var res = computer.ComputeUntilNextOuput();
        Console.WriteLine(res);
    }
}
