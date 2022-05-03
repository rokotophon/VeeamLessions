using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab2._3
{
    internal class BearAndBeesExNoSynchObjects : BearAndBeesBase, IDisposable
    {
        public BearAndBeesExNoSynchObjects(int honeyPotCapacity, int beesCount) : base(honeyPotCapacity, beesCount)
        {

        }

        public override void Dispose()
        {
            tokenSource.Cancel();
            bearTask.Wait();
            Task.WaitAll(beeTasks);
            tokenSource.Dispose();
            threadRandom.Dispose();
        }

        protected override void BearThread(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var completedIndex = Task.WaitAny(beeTasks);
                currentPartionCounter++;
                Console.WriteLine($"Bee #{completedIndex+1} puts honey portion ({currentPartionCounter}) to the Pot");
                if (currentPartionCounter == honeyPotCapacity)
                {
                    Console.WriteLine($"Bee #{completedIndex+1} woke up a Bear");
                    beeTasks[completedIndex] = Task.Run(() => BeeThread(completedIndex+1, token));
                    Console.WriteLine($"Bear starts to eat a honey");
                    while (currentPartionCounter > 0)
                    {
                        Thread.Sleep(BearPortionEatingTimeMs);
                        Console.WriteLine($"Bear eate one of {currentPartionCounter} portions");
                        currentPartionCounter--;
                    }
                    Console.WriteLine($"Pot is empty. Bear goes to sleep");
                }
                else
                    beeTasks[completedIndex] = Task.Run(() => BeeThread(completedIndex+1, token));
            }

            Console.WriteLine($"Bear thread complete");
        }

        protected override void BeeThread(int beeIndex, CancellationToken token)
        {
            Console.WriteLine($"Bee #{beeIndex} began to collect honey...");
            var time = threadRandom.Value.Next(BeeFlightTimeMs_Min, BeeFlightTimeMs_Max);
            Thread.Sleep(time);
        }
    }
}
