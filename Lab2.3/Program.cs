using System;

namespace Lab2._3
{
    partial class Program
    {
        static void Main(string[] args)
        {

            ////no eating delay, bear ManualEvent, lock
            //using (var bab = new BearAndBeesAutoLock(5, 5))
            //    UserProc();

            ////eating delay, bear ManualEvent, pot ManualEvent, lock
            //using (var bab = new BearAndBeesExManualEvents(5, 5))
            //    UserProc();

            ////eating delay, bear ManualEvent, pot AutoEvent
            //using (var bab = new BearAndBeesExAutoEvent(5, 1))
            //    UserProc();

            ////eating delay, bear AutoEvent, bee Mutex, SpinWait.SpinUntil
            //using (var bab = new BearAndBeesExMutex(5, 5))
            //    UserProc();

            //eating delay, no synch objects
            using (var bab = new BearAndBeesExNoSynchObjects(5, 5))
                UserProc();


            Console.WriteLine("Complete");

        }

        private static void UserProc()
        {
            while (true)
                if (Console.ReadKey(true).KeyChar == 'c')
                {
                    Console.WriteLine("Cancel");
                    return;
                }   
        }
    }
}
