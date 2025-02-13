class Part2
{
    static int Compute(Dictionary<string, string> m)
    {
        var path = new HashSet<string>();
        for (var s = m["YOU"]; s != "COM"; s = m[s]) path.Add(s);
        var intersection = m["SAN"];
        for (; !path.Contains(intersection); intersection = m[intersection]) ;
        var res = 0;
        foreach (var s0 in new[] { "YOU", "SAN" })
        {
            for (var s = m[s0]; s != intersection; s = m[s]) res++;
        }
        return res;
    }

    public static void Solve(string raw)
    {
        var m = raw
            .Split('\n')
            .Select(e => e.Split(')'))
            .ToDictionary(e => e[1], e => e[0]);
        var res = Compute(m);
        Console.WriteLine(res);
    }
}
