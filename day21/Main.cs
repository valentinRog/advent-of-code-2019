class Day21
{
    static void Main()
    {
        var raw = Console.In.ReadToEnd().Replace("\r", "").Trim();
        Part1.Solve(raw);
        Part2.Solve(raw);
    }
}
