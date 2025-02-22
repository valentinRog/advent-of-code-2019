using System.Diagnostics;

class Part2
{
    static void Run(Action a) => a();

    class Computer(IEnumerable<int> data)
    {
        readonly int[] a = [.. data];

        int i = 0;

        bool ended = false;
        public bool Ended { get => ended; }

        readonly Queue<int> inputs = new();

        public void AddInput(int n) => inputs.Enqueue(n);

        static int ExtractOpcode(int n) => n % 100;

        static int ExtractMode(int n, int offset) => n / (int)Math.Pow(10, offset + 1) % 10;

        int ExtractValue(int offset)
        {
            return ExtractMode(a[i], offset) switch
            {
                0 => a[a[i + offset]],
                1 => a[i + offset],
                _ => throw new UnreachableException(),
            };
        }

        public int? Step()
        {
            int? output = null;
            Run(ExtractOpcode(a[i]) switch
            {
                1 => () =>
                {
                    a[a[i + 3]] = ExtractValue(1) + ExtractValue(2);
                    i += 4;
                }
                ,
                2 => () =>
                {
                    a[a[i + 3]] = ExtractValue(1) * ExtractValue(2);
                    i += 4;
                }
                ,
                3 => () =>
                {
                    a[a[i + 1]] = inputs.Dequeue();
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
                    a[a[i + 3]] = ExtractValue(1) < ExtractValue(2) ? 1 : 0;
                    i += 4;
                }
                ,
                8 => () =>
                {
                    a[a[i + 3]] = ExtractValue(1) == ExtractValue(2) ? 1 : 0;
                    i += 4;
                }
                ,
                _ => throw new UnreachableException(),
            });
            if (a[i] == 99) ended = true;
            return output;
        }

        public int? ComputeUntilNextOuput()
        {
            int? res = null;
            while (!res.HasValue && !ended) res = Step();
            return res;
        }
    }

    class Amplifiers
    {
        readonly Computer[] computers;

        public Amplifiers(IEnumerable<int> prog, IEnumerable<int> a)
        {
            computers = [.. Enumerable.Range(0, 5).Select(_ => new Computer(prog))];
            foreach (var (computer, n) in computers.Zip(a)) computer.AddInput(n);
        }

        public int ComputeSignal()
        {
            var amplifierSignal = 0;
            for (int i = 0; ; i = (i + 1) % computers.Length)
            {
                computers[i].AddInput(amplifierSignal);
                var n = computers[i].ComputeUntilNextOuput();
                if (n is null) return amplifierSignal;
                amplifierSignal = (int)n;
            }
        }
    }

    class Solver(IEnumerable<int> prog)
    {
        readonly IEnumerable<int> prog = prog;

        int? res = null;
        public int? Res { get => res; }

        void Solve(HashSet<int> hs, List<int> a)
        {
            if (hs.Count == 0)
            {
                var amplifierSignal = new Amplifiers(prog, a).ComputeSignal();
                res = int.Max(res ?? amplifierSignal, amplifierSignal);
                return;
            }
            foreach (var n in hs)
            {
                HashSet<int> newHs = [.. hs];
                newHs.Remove(n);
                a.Add(n);
                Solve(newHs, a);
                a.RemoveAt(a.Count - 1);
            }
        }

        public void Solve() => Solve([5, 6, 7, 8, 9], []);
    }

    public static void Solve(string raw)
    {
        Solver solver = new(raw.Split(',').Select(int.Parse));
        solver.Solve();
        Console.WriteLine(solver.Res);
    }
}
