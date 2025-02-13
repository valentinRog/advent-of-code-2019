using System.Diagnostics;

class Part2
{
    class Layers
    {
        readonly int width;
        readonly int height;

        readonly List<char[]> layers = [];

        public Layers(int width, int height, string raw)
        {
            this.width = width;
            this.height = height;
            int windowSize = width * height;
            for (int i = 0; i < raw.Length; i += windowSize)
            {
                var a = new char[width * height];
                for (int ii = 0; ii < windowSize; ii++)
                {
                    a[ii] = raw[i + ii];
                    layers.Add(a);
                }
            }
        }

        char[] DecodeImage()
        {
            var res = new char[width * height];
            foreach (var i in Enumerable.Range(0, width * height))
            {
                foreach (var layer in layers)
                {
                    res[i] = layer[i];
                    if (layer[i] != '2') break;
                }
            }
            return res;
        }

        public void Dump()
        {
            var a = DecodeImage();
            foreach (var y in Enumerable.Range(0, height))
            {
                foreach (var x in Enumerable.Range(0, width))
                {
                    var c = a[y * width + x] switch
                    {
                        '0' => ' ',
                        '1' => '#',
                        _ => throw new UnreachableException(),
                    };
                    Console.Write(c);
                }
                Console.WriteLine();
            }
        }
    }

    public static void Solve(string raw)
    {
        Layers layers = new(25, 6, raw);
        layers.Dump();
    }
}
