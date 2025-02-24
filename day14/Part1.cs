class Part1
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

    public static void Solve(string raw)
    {
        var m = raw
            .Split("\n")
            .Select(line => new Reaction(line))
            .ToDictionary(r => r.Result.Name, r => r);
        Producer solver = new(m);
        solver.Produce("FUEL", 1);
        Console.WriteLine(solver.Ore);
    }
}
