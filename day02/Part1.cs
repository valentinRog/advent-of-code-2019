using System.Diagnostics;

class Part1
{
    static void Run(Action a) => a();

    static int Compute(IEnumerable<int> data)
    {
        var a = data.ToArray();
        a[1] = 12;
        a[2] = 2;
        for (var i = 0; a[i] != 99; i += 4)
        {
            Run(a[i] switch
            {
                1 => () => a[a[i + 3]] = a[a[i + 1]] + a[a[i + 2]],
                2 => () => a[a[i + 3]] = a[a[i + 1]] * a[a[i + 2]],
                _ => throw new UnreachableException(),
            });
        }
        return a[0];
    }

    public static void Solve(string raw)
    {
        var l = raw.Split(',').Select(int.Parse).ToList();
        Console.WriteLine(Compute(l));
    }
}
