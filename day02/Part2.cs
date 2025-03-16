using System.Diagnostics;

class Part2
{
    static T Run<T>(Func<T> f) => f();

    class Computer(IEnumerable<int> data)
    {
        readonly int[] a = [.. data];

        public void SetRegister(int index, int value) => a[index] = value;

        public int Compute()
        {
            int[] a = [.. this.a];
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
    }

    public static void Solve(string raw)
    {
        var computer = new Computer(raw.Split(',').Select(int.Parse));
        var res = Run(() =>
        {
            foreach (var noun in Enumerable.Range(1, 100))
            {
                foreach (var verb in Enumerable.Range(1, 100))
                {
                    computer.SetRegister(1, noun);
                    computer.SetRegister(2, verb);
                    if (computer.Compute() == 19690720) return 100 * noun + verb;
                }
            }
            throw new UnreachableException();
        });
        Console.WriteLine(res);
    }
}
