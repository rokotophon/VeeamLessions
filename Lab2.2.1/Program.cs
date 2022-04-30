using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab2._2._1
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Begin");

            var countDown = new CMyCountdownEvent(1);

            Task.Run(() => CountdownTask(countDown, 3000));
            Task.Run(() => CountdownTask(countDown, 5000));


            Thread.Sleep(4000);
            Console.WriteLine($"Main Thread Signal");
            countDown.Signal();
            Thread.Sleep(1500);

            countDown.Dispose();
            Console.WriteLine("End");
        }

        static void CountdownTask(CMyCountdownEvent countDown, int timeoutMs)
        {
            Console.WriteLine($"Task {Thread.CurrentThread.ManagedThreadId} start");

            Console.WriteLine($"Task {Thread.CurrentThread.ManagedThreadId} Wait return: {countDown.Wait(TimeSpan.FromMilliseconds(timeoutMs))}");

            Console.WriteLine($"Task {Thread.CurrentThread.ManagedThreadId} End");
        }
    }
}
