using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Lab1._5
{
    internal class MyCache : IDisposable
    {
        //private readonly Timer timer;
        private readonly TimeSpan itemLivetime;
        private readonly List<SomeHandle> items;
        private readonly int maxCapacity;
        public MyCache(int itemLivetimeSec = 2, int maxCapacity = 10)
        {
            this.maxCapacity = maxCapacity;
            this.itemLivetime = TimeSpan.FromSeconds(itemLivetimeSec);
            items = new List<SomeHandle>(maxCapacity);
            //timer = new Timer(1000); конфликт перечисления/изменения items
            //timer.Start();
            //timer.Elapsed += Timer_Elapsed;
        }
        public SomeHandle Add(IntPtr handle)
        {
            RemoveExpired();
            var sh = new SomeHandle(handle);
            if (items.Count == maxCapacity)
               RemoveOldest();

            items.Add(sh);
            return sh;
        }

        private SomeHandle RemoveOldest()
        {
            var old = items.Aggregate((i1, i2) => i1.LasUsed < i2.LasUsed ? i1 : i2);
            Console.WriteLine($"Capacity overflow. Removing handle {old.Handle} created on {old.LasUsed}");

            items.Remove(old);
            old.Dispose();
            return old;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RemoveExpired();
        }

        public void RemoveExpired()
        {
            for (int i = items.Count - 1; i > -1; i--)
            {
                if (DateTime.Now - items[i].LasUsed > itemLivetime)
                {
                    items[i].Dispose();
                    items.RemoveAt(i--);
                }
            }
        }

        public void Dispose()
        {
            Console.WriteLine("MyCache Disposing...");
            //timer.Dispose();
            foreach (var item in items)
            {
                item.Dispose();
            }
        }
    }
}
