class Part2
{
    static int ComputeFuel(int n)
    {
        var nn = n / 3 - 2;
        return nn > 0 ? nn + ComputeFuel(nn) : 0;
    }

    public static void Solve(string raw)
    {
        var res = raw.Split('\n').Select(w => ComputeFuel(int.Parse(w))).Sum();
        Console.WriteLine(res);
    }
}
