using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Darkages.Common
{
    public static class Generator
    {
        public static Random Random { get; private set; }
        public static Collection<int> GeneratedNumbers { get; private set; }
        public static Collection<string> GeneratedStrings { get; private set; }

        static Generator()
        {
            Generator.Random = new Random();
            Generator.GeneratedNumbers = new Collection<int>();
            Generator.GeneratedStrings = new Collection<string>();
        }

        public static int GenerateNumber()
        {
            int id;

            do
            {
                id = Random.Next();
            }
            while (Generator.GeneratedNumbers.Contains(id));

            lock (Random)
            {
                GeneratedNumbers.Add(id);
            }

            return id;
        }

        public static string CreateString(int size)
        {
            var value = new StringBuilder();

            for (var i = 0; i < size; i++)
            {
                var binary = Generator.Random.Next(0, 2);

                switch (binary)
                {
                    case 0:
                        value.Append(Convert.ToChar(Generator.Random.Next(65, 91)));
                        break;

                    case 1:
                        value.Append(Generator.Random.Next(1, 10));
                        break;
                }
            }

            return value.ToString();
        }
        public static string GenerateString(int size)
        {
            string s;

            do
            {
                s = Generator.CreateString(size);
            }
            while (Generator.GeneratedStrings.Contains(s));

            Generator.GeneratedStrings.Add(s);

            return s;
        }
    }
}