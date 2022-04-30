using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab2._2._1
{
    public class CMyCountdownEvent : IDisposable
    {
        private int counter;
        private readonly object lockObject;
        private readonly ManualResetEvent manualReset;
        public CMyCountdownEvent(int initialCount)
        {
            counter = initialCount;
            manualReset = new ManualResetEvent(false);
            lockObject = new object();
        }
        public void Signal()
        {
            lock(lockObject)
            {
                counter--;
                if (counter == 0)
                    manualReset.Set();
                if (counter < 0)
                    throw new InvalidOperationException();
            }
        }
        public void Signal(int signalCount)
        {
            lock (lockObject)
            {
                counter -= signalCount;
                if (counter == 0)
                    manualReset.Set();
                if (counter < 0)
                    throw new InvalidOperationException();
            }

        }
        public bool Wait(TimeSpan timeout)
        {
            //return Task.Run(() => manualReset.WaitOne()).Wait(timeout);
            
            return manualReset.WaitOne(timeout);

            var signaled = false;
            Task.WhenAny(
                Task.Run(() => Thread.Sleep(timeout)), 
                Task.Run(() => 
                { 
                    manualReset.WaitOne(); 
                    signaled = true;
                })).Wait();
            return signaled;
        }
        public void Dispose()
        {
            manualReset.Dispose();
        }
    }
}
