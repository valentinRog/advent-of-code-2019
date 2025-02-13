class Part1
{
    static bool Valid(int n)
    {
        var s = n.ToString();
        var a = s.ToArray();
        Array.Sort(a);
        return s.ToHashSet().Count < s.Length && a.SequenceEqual(s);
    }
    public static void Solve(string raw)
    {
        var a = raw.Split('-').Select(int.Parse).ToArray();
        var res = Enumerable.Range(a[0], a[1] - a[0]).Count(Valid);
        Console.WriteLine(res);
    }
}