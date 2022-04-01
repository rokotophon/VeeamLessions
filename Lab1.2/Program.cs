using System;
using System.Collections.Generic;

namespace Lab1._2
{
    class Program
    {
        static void Main(string[] args)
        {
            var pub1 = new Publisher("Publisher #1");

            var sub1 = new Subscriber("Subscriber #1");
            var sub2 = new Subscriber("Subscriber #2");

            pub1.Post();
            EventBus.UnSubscribe(sub2);

            using (var sub3 = new Subscriber("Subscriber #3"))
            {
                pub1.Post();
            }
            pub1.Post();

        }
    }
    internal interface ISubscriber
    {
        void OnEvent();
    }    
    class Publisher
    {
        private string name;

        public Publisher(string name) => this.name = name;

        public void Post()
        {
            Console.WriteLine($"[{name}] post msg");
            EventBus.Raise();
        }

    }
    static class EventBus
    {
        private static Action actions;
        internal static void Raise() => actions?.Invoke();
        internal static void Subscribe(ISubscriber subscriber) => actions += subscriber.OnEvent;
        internal static void UnSubscribe(ISubscriber subscriber) => actions -= subscriber.OnEvent;
    }


    class Subscriber : ISubscriber, IDisposable
    {
        private string name;

        public Subscriber(string name)
        {
            this.name = name;
            EventBus.Subscribe(this);
        }

        public void Dispose() => EventBus.UnSubscribe(this);
        public void OnEvent() => Console.WriteLine($"[{name}] msg received");
    }
}
