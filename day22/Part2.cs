using System.Numerics;

class Part2
{
    public static void Solve(string raw)
    {
        BigInteger a = 1;
        BigInteger b = 0;
        var m = 119315717514047;
        var k = 101741582076661;
        BigInteger ModPow(BigInteger value, BigInteger exponent) => BigInteger.ModPow(value, exponent, m);
        foreach (var ins in raw.Split("\n").Reverse())
        {
            string[] arr = [.. ins.Split(" ")];
            if (ins == "deal into new stack")
            {
                a *= -1;
                b = m - 1 - b;
            }
            else if (arr[0] == "cut")
            {
                b += int.Parse(arr[^1]);
            }
            else
            {
                var p = ModPow(int.Parse(arr[^1]), m - 2);
                a *= p;
                b *= p;
            }
        }
        var res = (ModPow(a, k) * 2020 + (ModPow(a, k) - 1) * ModPow(a - 1, m - 2) * b) % m;
        Console.WriteLine(res);
    }
}
