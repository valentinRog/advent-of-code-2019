class Part1
{
    public static void Solve(string raw)
    {
        var res = raw.Split('\n').Select(w => int.Parse(w) / 3 - 2).Sum();
        Console.WriteLine(res);
    }
}
