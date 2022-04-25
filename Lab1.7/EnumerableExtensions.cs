using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab1._7
{
    public static class EnumerableExtensions
    {
        public static T RandomElement<T>(this IEnumerable<T> source, Random rnd)
        {
            return source.ElementAt(rnd.Next(0, source.Count()));
        }
    }
}
