using System;
using System.Threading;

namespace Lab1._5
{
    class Program
    {
        static void Main(string[] args)
        {
            var cache = new MyCache(1, 10);
            SomeHandle sh;
            var rnd = new Random();

            using(cache)
            for (int i = 0; i < 10; i++)
            {
                sh = cache.Add(new IntPtr(i));
                Thread.Sleep(rnd.Next(500, 1500));
                cache.RemoveExpired();
                sh.DoSomeWork();
            }
            
            Thread.Sleep(1500);

            using(cache = new MyCache(20, 10))
            for (int i = 0; i < 12; i++)
            {
                sh = cache.Add(new IntPtr(i));
                Thread.Sleep(100);
                sh.DoSomeWork();
            }
        }
    }
}
