class Day25
{
    static void Main(string[] args)
    {
        var raw = File.ReadAllText(args[0]).Replace("\r", "").Trim();
        Part1.Solve(raw);
    }
}
