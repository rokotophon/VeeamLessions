using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab1._3
{
    class Program
    {
        static void Main(string[] args)
        {
            var elements = Enumerable.Range(0, 10).Select(i => new { Name = $"Item{i}" });
            var delimeter = ' ';
            Console.WriteLine("Задание 1");
            Console.WriteLine(string.Concat(elements.Skip(3).Select(a => a.Name + delimeter)));

            Console.WriteLine("Задание 2");
            Console.WriteLine(string.Concat(elements.Where((a, i) => a.Name.Length > i)));

            Console.WriteLine("Задание 3");
            Lab3("Это что же получается: ходишь, ходишь в школу, а потом бац - вторая смена");

            Console.WriteLine("Задание 4");
            var enSourceStr = "This dog eats too much vegetables after lunch";
            var ruSourceStr = "Эта собака ест слишком много овощей после обеда";

            var enDicSource = enSourceStr.Split(' ');
            var ruDicSource = ruSourceStr.Split(' ');

            var dic = ruDicSource.ToDictionary(k => enDicSource[Array.IndexOf(ruDicSource, k)]);

            foreach (var p in GetPages(enSourceStr, dic, 3))
                Console.WriteLine(p);

        }
        private static IEnumerable<string> GetPages(string str, Dictionary<string, string> dic, int N)
        {
            var counter = 0;
            foreach (var chunk in str.Split(' ').Select(w => dic[w].ToUpper()).MyChunk(N))
            {
                counter++;
                yield return string.Join(' ', chunk) + $" // {counter} страница";
            } 
        }


        private static void Lab3(string sourceStr)
        {
            var preparedStr = sourceStr.Split(" .,:;!?-".ToCharArray()).Where(s => s.Length > 0);
            var groups = preparedStr.GroupBy(s => s.Length).OrderByDescending(g => g.Count());

            foreach (var g in groups)
            {
                Console.WriteLine($"Группа {g.Key}. Длинна {g.First().Length}. Количество {g.Count()}");
                foreach (var w in g)
                {
                    Console.WriteLine(w);
                }
            }
        }
    }
    static class Extensions
    {
        public static IEnumerable<IEnumerable<T>> MyChunk<T>(this IEnumerable<T> source, int size)
        {
            while(source.Any())
            {
                yield return source.Take(size);
                source = source.Skip(size);
            }
        }
    }
}
