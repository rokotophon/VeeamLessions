using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Lab1._5
{
    class SomeHandle : IDisposable
    {
        private DateTime lastUsed;
        private readonly IntPtr handle;
        private volatile int isDisposed;
        public SomeHandle(IntPtr handle)
        {
            this.handle = handle;
            Console.WriteLine($"Handle #{handle} received");
            lastUsed = DateTime.Now;
        }
        public DateTime LasUsed => lastUsed;
        public IntPtr Handle
        {
            get
            {
                if (isDisposed == 1)
                    throw new ObjectDisposedException($"Handle #{handle}");
                lastUsed = DateTime.Now;
                return handle;
            }
        }
        public bool IsDisposed => isDisposed == 1;

        public void DoSomeWork()
        {
            if (isDisposed == 0)
                Console.WriteLine($"Working with handle #{Handle}...");
            else
                Console.WriteLine($"Can't use handle #{handle}, handle released!");
        }

        public void Dispose()
        {
            if(Interlocked.CompareExchange(ref isDisposed, 1, 0) == 0)
            {
                Console.WriteLine($"Releasing Handle #{handle}, created {lastUsed}");
            }
        }
    }
}
