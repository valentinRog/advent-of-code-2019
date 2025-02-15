using System.Diagnostics;

class Part1
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
            while (!res.HasValue)
            {
                if (ended) return null;
                res = Step();
            }
            return res;
        }
    }

    class Solver(IEnumerable<int> prog)
    {
        readonly IEnumerable<int> prog = prog;

        int? res = null;
        public int? Res { get => res; }

        void Solve(int amplifierSignal, HashSet<int> hs)
        {
            if (hs.Count == 0)
            {
                res = int.Max(res ?? amplifierSignal, amplifierSignal);
                return;
            }
            foreach (var n in hs)
            {
                Computer computer = new(prog);
                computer.AddInput(n);
                computer.AddInput(amplifierSignal);
                var newAmplifierSignal = (int)computer.ComputeUntilNextOuput()!;
                HashSet<int> newHs = [.. hs];
                newHs.Remove(n);
                Solve(newAmplifierSignal, newHs);
            }
        }

        public void Solve() => Solve(0, [0, 1, 2, 3, 4]);
    }

    public static void Solve(string raw)
    {
        Solver solver = new(raw.Split(',').Select(int.Parse));
        solver.Solve();
        Console.WriteLine(solver.Res);
    }
}
