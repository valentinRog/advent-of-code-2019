class Part1
{
    static int Compute(IEnumerable<int> data)
    {
        var a = data.ToArray();
        a[1] = 12;
        a[2] = 2;
        for (var i = 0; a[i] != 99; i += 4)
        {
            switch (a[i])
            {
                case 1: a[a[i + 3]] = a[a[i + 1]] + a[a[i + 2]]; break;
                case 2: a[a[i + 3]] = a[a[i + 1]] * a[a[i + 2]]; break;
            }
        }
        return a[0];
    }

    public static void Solve(string raw)
    {
        var res = Compute(raw.Split(',').Select(int.Parse));
        Console.WriteLine(res);
    }
}
