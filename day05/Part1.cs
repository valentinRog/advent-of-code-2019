using System.Diagnostics;

class Part1
{
    static void Run(Action a) => a();

    class Computer(IEnumerable<int> data)
    {
        readonly int[] a = [.. data];

        static int ExtractOpcode(int n) => n % 100;

        static int ExtractMode(int n, int offset) => n / (int)Math.Pow(10, offset + 1) % 10;

        int ExtractValue(int i, int offset)
        {
            return ExtractMode(a[i], offset) switch
            {
                0 => a[a[i + offset]],
                1 => a[i + offset],
                _ => throw new UnreachableException(),
            };
        }

        public int Compute()
        {
            int? res = null;
            for (var i = 0; a[i] != 99;)
            {
                Run(ExtractOpcode(a[i]) switch
                {
                    1 => () =>
                    {
                        a[a[i + 3]] = ExtractValue(i, 1) + ExtractValue(i, 2);
                        i += 4;
                    }
                    ,
                    2 => () =>
                    {
                        a[a[i + 3]] = ExtractValue(i, 1) * ExtractValue(i, 2);
                        i += 4;
                    }
                    ,
                    3 => () =>
                    {
                        a[a[i + 1]] = 1;
                        i += 2;
                    }
                    ,
                    4 => () =>
                    {
                        res = ExtractValue(i, 1);
                        i += 2;
                    }
                    ,
                    _ => throw new UnreachableException(),
                });
            }
            if (res is null) throw new UnreachableException();
            return res.Value;
        }
    }

    public static void Solve(string raw)
    {
        var computer = new Computer(raw.Split(',').Select(int.Parse));
        var res = computer.Compute();
        Console.WriteLine(res);
    }
}
