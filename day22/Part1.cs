class Part1
{
    class Deck()
    {
        LinkedList<int> l = new([.. Enumerable.Range(0, 10007)]);
        public LinkedList<int> L { get => l; }

        public void Deal() => l = new([.. l.Reverse()]);

        public void Cut(int n)
        {
            LinkedList<int> ll = [];
            if (n > 0)
            {
                for (; n > 0; n--)
                {
                    ll.AddLast(l.First!.Value);
                    l.RemoveFirst();
                }
                while (ll.Count > 0)
                {
                    l.AddLast(ll.First!.Value);
                    ll.RemoveFirst();
                }
            }
            else
            {
                for (; n < 0; n++)
                {
                    ll.AddFirst(l.Last!.Value);
                    l.RemoveLast();
                }
                while (ll.Count > 0)
                {
                    l.AddFirst(ll.Last!.Value);
                    ll.RemoveLast();
                }
            }
        }

        public void DealWithIncrement(int n)
        {
            int[] a = [.. l];
            for (var i = 0; l.Count > 0; i = (i + n) % a.Length)
            {
                a[i] = l.First!.Value;
                l.RemoveFirst();
            }
            l = new(a);
        }
    }

    public static void Solve(string raw)
    {
        Console.WriteLine(raw);
        var deck = new Deck();
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
        var res = deck.L.Index().Where(e => e.Item == 2019).First().Index;
        Console.WriteLine(res);
    }
}