class Part2
{
    class Reaction
    {
        public record Ingredient(string Name, int N);
        readonly List<Ingredient> ingredients = [];
        public List<Ingredient> Ingredients { get => ingredients; }
        readonly Ingredient result;
        public Ingredient Result { get => result; }

        public Reaction(string s)
        {
            var l = s.Replace(",", "").Replace("=> ", "").Split(' ').ToArray();
            for (int i = 0; i < l.Length - 3; i += 2)
            {
                ingredients.Add(new(l[i + 1], int.Parse(l[i])));
            }
            result = new(l[^1], int.Parse(l[^2]));
        }
    }

    class Producer(Dictionary<string, Reaction> m)
    {
        readonly Dictionary<string, Reaction> m = m;
        readonly Dictionary<string, long> inventory = [];
        long ore = 0;
        public long Ore { get => ore; }

        bool CanProduce(string s, long k)
        {
            return m[s]
                .Ingredients
                .All(ing => inventory.GetValueOrDefault(ing.Name) >= ing.N * k);
        }

        public void Produce(string s, long n)
        {
            if (s == "ORE")
            {
                ore += n;
                inventory[s] = inventory.GetValueOrDefault(s) + n;
                return;
            }
            var k = (long)Math.Ceiling((double)n / m[s].Result.N);
            while (!CanProduce(s, k))
            {
                foreach (var ing in m[s].Ingredients)
                {
                    if (inventory.GetValueOrDefault(ing.Name) >= k * ing.N) continue;
                    Produce(ing.Name, k * ing.N - inventory.GetValueOrDefault(ing.Name));
                }
            }
            foreach (var ing in m[s].Ingredients) inventory[ing.Name] -= k * ing.N;
            inventory[s] = inventory.GetValueOrDefault(s) + k * m[s].Result.N;
        }
    }

    class Solver(Dictionary<string, Reaction> m)
    {
        static readonly long maxOre = 1000000000000;

        readonly Dictionary<string, Reaction> m = m;

        long Cost(long fuel)
        {
            Producer producer = new(m);
            producer.Produce("FUEL", fuel);
            return producer.Ore;
        }

        public long MaxFuelProduceable(long n1, long n2)
        {
            var n = (n1 + n2) / 2;
            return Cost(n) switch
            {
                var cost when cost <= maxOre && Cost(n + 1) > maxOre => n,
                var cost when cost < maxOre => MaxFuelProduceable(n, n2),
                _ => MaxFuelProduceable(n1, n),
            };
        }

        public long MaxFuelProduceable() => MaxFuelProduceable(0, maxOre);
    }

    public static void Solve(string raw)
    {
        var m = raw
            .Split("\n")
            .Select(line => new Reaction(line))
            .ToDictionary(r => r.Result.Name, r => r);
        Solver solver = new(m);
        var res = solver.MaxFuelProduceable();
        Console.WriteLine(res);
    }
}
