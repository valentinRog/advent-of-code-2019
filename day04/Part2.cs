class Part2
{
    static bool Valid(int n)
    {
        var s = n.ToString();
        if (s.ToHashSet().Count == s.Length) return false;
        var a = s.ToArray();
        Array.Sort(a);
        if (!a.SequenceEqual(s)) return false;
        return s.GroupBy(e => e).Select(e => e.Count()).Any(e => e == 2);
    }

    public static void Solve(string raw)
    {
        var a = raw.Split('-').Select(int.Parse).ToArray();
        var res = Enumerable.Range(a[0], a[1] - a[0]).Count(Valid);
        Console.WriteLine(res);
    }
}
