class Day20
{
    static void Main()
    {
        var raw = Console.In.ReadToEnd().Replace("\r", "").TrimEnd();
        Part1.Solve(raw);
        Part2.Solve(raw);
    }
}
