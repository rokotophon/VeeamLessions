using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab2._3
{
    internal class BearAndBeesAutoLock : BearAndBeesBase, IDisposable
    {
        protected readonly AutoResetEvent bearWakeupEvent;
        protected readonly object potLocker;

        public BearAndBeesAutoLock(int honeyPotCapacity, int beesCount) : base(honeyPotCapacity, beesCount)
        {
            potLocker = new object();
            bearWakeupEvent = new AutoResetEvent(false);

        }

        public override void Dispose()
        {
            tokenSource.Cancel();
            Task.WaitAll(beeTasks);
            bearWakeupEvent.Set();
            bearTask.Wait();
            tokenSource.Dispose();
            bearWakeupEvent.Dispose();
            threadRandom.Dispose();
        }

        protected override void BearThread(CancellationToken token)
        {               
            while (!token.IsCancellationRequested)
            {
                bearWakeupEvent.WaitOne();
                currentPartionCounter = 0;
                Console.WriteLine($"Bear ate honey and goues to sleep ");
            }
            Console.WriteLine($"Bear thread complete");

        }
        protected override void BeeThread(int beeIndex, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Console.WriteLine($"Bee #{beeIndex} began to collect honey...");
                var time = threadRandom.Value.Next(BeeFlightTimeMs_Min, BeeFlightTimeMs_Max);
                Thread.Sleep(time);

                lock (potLocker)
                {
                    currentPartionCounter++;
                    Console.WriteLine($"Bee #{beeIndex} puts honey portion ({currentPartionCounter}) to the Pot");
                    if (currentPartionCounter == honeyPotCapacity)
                    {
                        Console.WriteLine($"Bee #{beeIndex} woke up the Bear");
                        bearWakeupEvent.Set();
                    }
                }                        
            }
            Console.WriteLine($"Bee #{beeIndex} thread complete");

        }

    }
}
