class Part1
{
    class Deck(int cardCount, int i0)
    {
        int i = i0;
        public int I { get => i; }

        public void Deal() => i = cardCount - i - 1;

        public void Cut(int n) => i = (i - n) % cardCount;

        public void DealWithIncrement(int n) => i = i * n % cardCount;
    }

    public static void Solve(string raw)
    {
        var deck = new Deck(10007, 2019);
        foreach (var ins in raw.Split("\n"))
        {
            string[] arr = [.. ins.Split(" ")];
            if (ins == "deal into new stack")
            {
                deck.Deal();
            }
            else if (arr[0] == "cut")
            {
                deck.Cut(int.Parse(arr[^1]));
            }
            else
            {
                deck.DealWithIncrement(int.Parse(arr[^1]));
            }
        }
        Console.WriteLine(deck.I);
    }
}
