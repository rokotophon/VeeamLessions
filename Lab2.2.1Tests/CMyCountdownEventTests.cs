using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lab2._2._1.Program;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Lab2._2._1.Program.Tests
{
    [TestClass()]
    public class CMyCountdownEventTests
    {
        private void DoCountdownWork(CMyCountdownEvent countdownEvent, TimeSpan timeSpan, ref int workCounter)
        {
            countdownEvent.Wait(timeSpan);
            Interlocked.Increment(ref workCounter);
        }
        [TestMethod()]
        public void BaseTest()
        {
            int signalCount = 5;
            CMyCountdownEvent countdownEvent = new CMyCountdownEvent(signalCount);
            int workCounter = 0;
            const int threadCount = 5;
            var tasks = new Task[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                tasks[i] = Task.Run(() => DoCountdownWork(countdownEvent, TimeSpan.FromSeconds(signalCount + 1), ref workCounter));
            }
            do
            {
                Thread.Sleep(1000);
                Assert.AreEqual(0, workCounter);
                countdownEvent.Signal();
                signalCount--;
            } while (signalCount > 0);

            Task.WaitAll(tasks);
            Assert.AreEqual(threadCount, workCounter);
        }        
        
        [TestMethod()]
        public void TimeoutTest()
        {
            int signalCount = 1;
            var countdownEvent = new CMyCountdownEvent(signalCount);
            int workCounter = 0;
            const int threadCount = 5;
            var tasks = new Task[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                var timeout = TimeSpan.FromSeconds(i + 1);
                tasks[i] = Task.Run(() => DoCountdownWork(countdownEvent, timeout, ref workCounter));
            }
            Thread.Sleep(500);
            for (int i = 0; i < threadCount; i++)
            {
                Thread.Sleep(1000);
                Assert.AreEqual(i + 1, workCounter);
            } 
            
            countdownEvent.Signal();

            Task.WaitAll(tasks);
            Assert.AreEqual(threadCount, workCounter);
        }

        [TestMethod()]
        public void MultiThreadingSignalTest()
        {
            int signalCount = 10;
            var countdownEvent = new CMyCountdownEvent(signalCount);
            int workCounter = 0;
            const int threadCount = 10;
            var tasks = new Task[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    DoCountdownWork(countdownEvent, TimeSpan.FromMilliseconds(500), ref workCounter);
                    countdownEvent.Signal();
                });
            }

            countdownEvent.Wait(TimeSpan.FromSeconds(10));

            Assert.AreEqual(threadCount, workCounter);
        }

        [TestMethod]
        public void ExceptionTest()
        {
            var countdownEvent = new CMyCountdownEvent(2);
            countdownEvent.Signal();

            try
            {
                countdownEvent.Signal(2);
            }
            catch(InvalidOperationException)
            {
                try
                {
                    countdownEvent = new CMyCountdownEvent(1);
                    countdownEvent.Signal();
                    countdownEvent.Signal();
                }
                catch (InvalidOperationException)
                {
                    return;
                }
                Assert.Fail();
            }
            Assert.Fail();
        }

        [TestMethod]
        public void TimeoutTest2()
        {
            var countdownEvent = new CMyCountdownEvent(1);

            Task.Run(() =>
            {
                Thread.Sleep(5000);
                countdownEvent.Signal();
                Assert.IsTrue(countdownEvent.Wait(TimeSpan.FromMilliseconds(1000)));
            });

            Assert.IsFalse(countdownEvent.Wait(TimeSpan.FromMilliseconds(1000)));
            
            Thread.Sleep(5000);

            Assert.IsTrue(countdownEvent.Wait(TimeSpan.FromMilliseconds(10000)));
            
        }

    }
}