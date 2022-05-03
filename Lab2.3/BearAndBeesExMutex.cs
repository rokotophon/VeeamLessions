using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab2._3
{
    internal class BearAndBeesExMutex : BearAndBeesBase, IDisposable
    {
        private readonly AutoResetEvent bearAutoEvent;
        private readonly Mutex beeMutex;

        public BearAndBeesExMutex(int honeyPotCapacity, int beesCount) : base(honeyPotCapacity, beesCount)
        {
            bearAutoEvent = new AutoResetEvent(false);
            beeMutex = new Mutex();
        }

        public override void Dispose()
        {
            tokenSource.Cancel();
            Task.WaitAll(beeTasks);

            bearTask.Wait();

            bearAutoEvent.Dispose();
            beeMutex.Dispose();
            threadRandom.Dispose();

        }

        protected override void BearThread(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                bearAutoEvent.WaitOne();
                beeMutex.WaitOne();
                Console.WriteLine($"Bear starts to eat a honey");
                while (currentPartionCounter > 0)
                {
                    Thread.Sleep(BearPortionEatingTimeMs);
                    Console.WriteLine($"Bear eate one of {currentPartionCounter} portions");
                    currentPartionCounter--;
                }
                Console.WriteLine($"Pot is empty. Bear goes to sleep");
                beeMutex.ReleaseMutex();
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

                if(currentPartionCounter == honeyPotCapacity)
                    SpinWait.SpinUntil(() => currentPartionCounter == 0 || token.IsCancellationRequested);

                beeMutex.WaitOne();

                currentPartionCounter++;
                Console.WriteLine($"Bee #{beeIndex} puts honey portion ({currentPartionCounter}) to the Pot");
                if (currentPartionCounter == honeyPotCapacity)
                {
                    Console.WriteLine($"Bee #{beeIndex} woke up a Bear");
                    bearAutoEvent.Set();
                }
                beeMutex.ReleaseMutex();

            }
            Console.WriteLine($"Bee #{beeIndex} thread complete");
        }
    }
}
