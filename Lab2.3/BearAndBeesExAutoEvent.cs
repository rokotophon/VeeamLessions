using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab2._3
{
    internal class BearAndBeesExAutoEvent : BearAndBeesBase, IDisposable
    {
        private readonly ManualResetEventSlim bearWakeupEvent;
        private readonly AutoResetEvent potAccessEvent;

        public BearAndBeesExAutoEvent(int honeyPotCapacity, int beesCount) : base(honeyPotCapacity, beesCount)
        {
            bearWakeupEvent = new ManualResetEventSlim(false);
            potAccessEvent = new AutoResetEvent(true);
        }

        public override void Dispose()
        {
            tokenSource.Cancel();
            Task.WaitAll(beeTasks);
            bearWakeupEvent.Set();
            bearTask.Wait();
            bearWakeupEvent.Dispose();
            potAccessEvent.Dispose();
            threadRandom.Dispose();
        }

        protected override void BearThread(CancellationToken token)
        {               
            while (!token.IsCancellationRequested)
            {
                bearWakeupEvent.Wait();

                Console.WriteLine($"Bear starts to eat a honey");
                while(currentPartionCounter > 0)
                {
                    Thread.Sleep(BearPortionEatingTimeMs);
                    Console.WriteLine($"Bear eate one of {currentPartionCounter} portions");
                    currentPartionCounter--;
                }
                Console.WriteLine($"Pot is empty. Bear goes to sleep");

                bearWakeupEvent.Reset();
                potAccessEvent.Set();

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

                if (bearWakeupEvent.IsSet)
                {
                    Console.WriteLine($"Bee #{beeIndex} can't put a honey into the Pot");
                }


                try
                {
                    potAccessEvent.WaitOne();
                }
                catch
                {
                    Console.WriteLine($"Bee #{beeIndex} flight interrupted");
                    break;
                }

                currentPartionCounter++;
                Console.WriteLine($"Bee #{beeIndex} puts honey portion ({currentPartionCounter}) to the Pot");
                if (bearWakeupEvent.IsSet)
                    throw new Exception();
                if (currentPartionCounter == honeyPotCapacity)
                {
                    Console.WriteLine($"Bee #{beeIndex} woke up a Bear");
                    bearWakeupEvent.Set();
                }
                else
                    potAccessEvent.Set();

            }
            Console.WriteLine($"Bee #{beeIndex} thread complete");

        }

    }
}
