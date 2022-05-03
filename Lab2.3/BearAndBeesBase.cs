using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab2._3
{
    internal abstract class BearAndBeesBase
    {

        protected const int BeeFlightTimeMs_Min = 1000;
        protected const int BeeFlightTimeMs_Max = 5000;
        protected const int BearPortionEatingTimeMs = 500;
        protected readonly ThreadLocal<Random> threadRandom;
        protected readonly Task bearTask;
        protected readonly Task[] beeTasks;
        protected readonly CancellationTokenSource tokenSource;
        protected readonly int honeyPotCapacity;
        protected readonly int beesCount;
        protected int currentPartionCounter;

        public BearAndBeesBase(int honeyPotCapacity, int beesCount)
        {
            this.honeyPotCapacity = honeyPotCapacity;
            this.beesCount = beesCount;
            threadRandom = new ThreadLocal<Random>(() => new Random());
            tokenSource = new CancellationTokenSource();


            bearTask = Task.Run(() => BearThread(tokenSource.Token), tokenSource.Token);
            beeTasks = new Task[beesCount];
            for (int i = 0; i < beesCount; i++)
            {
                var beeIndex = i + 1;
                beeTasks[i] = Task.Run(() => BeeThread(beeIndex, tokenSource.Token), tokenSource.Token);
            }

        }


        public abstract void Dispose();
        protected abstract void BearThread(CancellationToken token);
        protected abstract void BeeThread(int beeIndex, CancellationToken token);
    }
}