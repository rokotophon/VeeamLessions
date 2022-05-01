using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Lab2._2._2
{
    class Program
    {
        static void Main(string[] args)
        {
            var barbershop = new Barbershop(2);
            Task.Run(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    barbershop.AddClient(new Client());
                    Thread.Sleep(500);
                }
            });

            while(true)
            {
                if (Console.ReadKey(true).KeyChar == 'c')
                    break;
                else
                    barbershop.AddClient();
            }

            barbershop.Dispose();
        }
    }

    public class Barbershop : IDisposable
    {
        Barber barber;
        BlockingCollection<Client> quene;
        CancellationTokenSource closeToken;
        Client currentClient;

        public Barbershop(int waitingChairsLimit)
        {
            quene = new BlockingCollection<Client>(new ConcurrentQueue<Client>(), waitingChairsLimit );
            closeToken = new CancellationTokenSource();

            barber = new Barber("Matthew");
            barber.DoWork(BarberWork);

        }

        public void Dispose()
        {
            closeToken.Cancel();
            barber.WorkTask.Wait();
            quene.Dispose();
            closeToken.Dispose();
        }

        public void AddClient() => AddClient(new Client());
        public void AddClient(Client client)
        {
            Console.WriteLine($"Client {client.Name}: comes in");
            if(Interlocked.CompareExchange(ref currentClient, client, null) != null)
            {
                if(!quene.TryAdd(client))
                {
                    Console.WriteLine($"===Client {client.Name}: kicked off from Barbershop, there is no empty chair");
                }                
            }
        }
        private void BarberWork()
        {
            while(true)
            {
                SpinWait.SpinUntil(() => currentClient != null || closeToken.IsCancellationRequested);
                if (closeToken.IsCancellationRequested)
                {
                    Console.WriteLine("Barber finished his work today");
                    return;
                }             
                
                Console.WriteLine($"Barber {barber.Name} cut's the client's hair: {currentClient.Name}");
                barber.State = BarberState.CutoffCliensHair;
                Thread.Sleep(1000);
                Console.WriteLine($"Barber {barber.Name} escorts the client to the door: {currentClient.Name}");
                barber.State = BarberState.EscortClient;
                Thread.Sleep(500);

                if (quene.TryTake(out var nextClient))
                {
                    currentClient = nextClient;
                }
                else
                {
                    Console.WriteLine($"Barber {barber.Name} sleeping");
                    currentClient = null;
                }
            }
        }

    }
    public enum BarberState
    {
        Sleeping,
        CutoffCliensHair,
        EscortClient
    }
    public class Barber
    {
        private BarberState state;

        public Barber(string name)
        {
            this.Name = name;
            State = BarberState.Sleeping;
        }
        public string Name { get; private set; }
        public Task WorkTask { get; private set; }
        public BarberState State
        {
            get => state; 
            set
            {
                if (state != value)
                {
                    //Console.WriteLine($"Barber {Name}: state change {state} -> {value}");
                    state = value;
                }
            }
        }

        internal void DoWork(Action work)
        {
            WorkTask = Task.Run(work);
        }
    }

    public class Client
    {
        static int ClientsCounter = 0;
        public string Name { get; private set; }
        public Client()
        {
            Name = "Client #" + Interlocked.Increment(ref ClientsCounter);
        }

    }

}
