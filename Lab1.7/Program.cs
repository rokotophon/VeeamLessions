using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab1._7
{
    class Program
    {
        static void Main(string[] args)
        {
            var ip = IPAddress.Loopback;
            var port = 35548;
            var server = new ChatServer(ip, port);
            server.Start();
            server.ProcMsg();
            server.Stop();
        }
    }

    public static class ServiseCommands
    {
        public const string NextRoomRequest = "#next";
        public const string PingUser = "#ping";
        public const string SrvUsersList = "#users";
    }
    class ChatServer
    {
        IPAddress ip;
        int port;
        Socket listener;
        ConcurrentDictionary<int, User> usersPool;
        int idCounter;
        public ChatServer(IPAddress adr, int port)
        {
            this.ip = adr;
            this.port = 35548;
            usersPool = new ConcurrentDictionary<int, User>();
        }

        public void Start()
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var point = new IPEndPoint(ip, port);
            try
            {
                listener.Bind(point);
                listener.Listen(10);
                Console.WriteLine("Server started...");
                Task.Run(IncomingListener);
                Task.Run(PoolUsersProcess);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void PoolUsersProcess()
        {
            while(usersPool != null)
            {
                foreach (var kv in usersPool)
                {
                    if(kv.Value.Socket.Connected && (DateTime.Now - kv.Value.LastDataReceiving).TotalMilliseconds > 2000)
                        kv.Value.Socket.Send(Encoding.Unicode.GetBytes(ServiseCommands.PingUser));
                }

                Thread.Sleep(1000);

                foreach (var kv in usersPool)
                {
                    var ms = DateTime.Now - kv.Value.LastDataReceiving;
                    if (ms.TotalMilliseconds > 11000)
                    {
                        Console.WriteLine($"Disconnecting user \"{kv.Value.Name}\". Reason: last message was received {ms.TotalMilliseconds}ms ago.");
                        ShutdownUser(kv.Value);

                    }                
                }

                while(usersPool.Count > 1)
                {
                    var u1 = usersPool.FirstOrDefault();
                    var u2 = usersPool.Skip(1).RandomElement(new Random());
                    if(TryCreateRoom(u1.Value, u2.Value))
                    {
                        if(!usersPool.TryRemove(u1.Key, out var _))
                            throw new Exception($"Can't remove User \"{u1.Value.Name}\" from UserPool!");
                        if(!usersPool.TryRemove(u2.Key, out var _))
                            throw new Exception($"Can't remove User \"{u2.Value.Name}\" from UserPool!");
                    }
                }

            }
        }

        private bool TryCreateRoom(User u1, User u2)
        {
            if(u1.Socket.Connected && u2.Socket.Connected && 
                u1.Room == null && u2.Room == null)
            {
                var room = new ChatRoom(u1, u2);
                u1.Socket.Send(Encoding.Unicode.GetBytes($"You are chatting with \"{u2.Name}\""));
                u2.Socket.Send(Encoding.Unicode.GetBytes($"You are chatting with \"{u1.Name}\""));
                return true;
            }
            return false;
        }


        private void ShutdownUser(User user)
        {
            if(usersPool.ContainsKey(user.Id))
                if (!usersPool.TryRemove(user.Id, out var _))
                    throw new Exception($"Can't remove User \"{user.Name}\" from UserPool!");

            user.Socket.Shutdown(SocketShutdown.Both);
            user.Socket.Close();
            Console.WriteLine($"User \"{user.Name}\" with socket {user.Socket.Handle} disconnected.");

            if (user.Room != null)
                CloseRoom(user.Room);
        }

        private void CloseRoom(ChatRoom room)
        {
            if (room == null)
                throw new NullReferenceException();

            room.User1.Room = room.User2.Room = null;

            if (room.User1.Socket.Connected)
            {
                room.User1.Socket.Send(Encoding.Unicode.GetBytes("Chat closed"));
                room.User1.LastDataReceiving = DateTime.Now;
                if (!usersPool.TryAdd(room.User1.Id, room.User1))
                    throw new Exception("Can't add user to pool");
            }
            if (room.User2.Socket.Connected)
            {
                room.User2.LastDataReceiving = DateTime.Now;
                room.User2.Socket.Send(Encoding.Unicode.GetBytes("Chat closed"));
                if (!usersPool.TryAdd(room.User2.Id, room.User2))
                    throw new Exception("Can't add user to pool");
            }
        }

        private void IncomingListener()
        {
            while (true)
            {
                var clientSocked = listener.Accept();

                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                byte[] data = new byte[512];

                do
                {
                    bytes = clientSocked.Receive(data);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (clientSocked.Available > 0);

                var user = new User(Interlocked.Increment(ref idCounter), builder.ToString(), clientSocked);
                usersPool.TryAdd( user.Id, user );

                Task.Run( () => UserListenner(user));

                Console.WriteLine(string.Format("{0} : User connected and added to UserPool: \"{1}\", socket: {2}", DateTime.Now.ToString(), builder.ToString(), user.Socket.Handle));
                clientSocked.Send(Encoding.Unicode.GetBytes($"Welcome to ChatServer, {builder}!"));

            }
        }

        private void UserListenner(User user)
        {
            //Console.WriteLine($"Listener for user \"{user.Name}\", with soket {user.Socket.Handle} started!");
            byte[] data = new byte[512];
            while (true)
            {
                if (!user.Socket.Connected)
                {
                    Console.WriteLine($"Can't receive from disconnected: User \"{user.Name}\", with soket {user.Socket.Handle}!");
                    return;
                }
                try
                {
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = user.Socket.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (user.Socket.Available > 0);

                    ProcessUserMessage(user, builder.ToString());
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Connection error:\n User \"{user.Name}\"\n Socked: {user.Socket.Handle}\n Exception: {ex}\n Error: {ex.Message}");
                    ShutdownUser(user);
                    return;
                }
            }
        }

        private void ProcessUserMessage(User user, string msg)
        {
            user.LastDataReceiving = DateTime.Now;
            if (msg.StartsWith('#'))
            {
                var cmd = msg.ToLower().Split(' ');
                switch (cmd[0])
                {
                    case ServiseCommands.NextRoomRequest:
                        if(user.Room != null)
                            CloseRoom(user.Room);
                        break;
                    case ServiseCommands.PingUser:
                        break;
                }
            }
            else
            {
                if (user.Room != null)
                {
                    user.Room.User1.Socket.Send(Encoding.Unicode.GetBytes($"{user.Name} : {msg}"));
                    user.Room.User2.Socket.Send(Encoding.Unicode.GetBytes($"{user.Name} : {msg}"));
                }
            }
            //Console.WriteLine($"Message from \"{user.Name}\": {msg}");
        }

        private void NextRoomUserRequest(User user)
        {
            if(user.Room != null)
            {
                var room = user.Room;
                room.User1.Socket.Send(Encoding.Unicode.GetBytes($"Chat closed by {user.Name}"));
                room.User2.Socket.Send(Encoding.Unicode.GetBytes($"Chat closed by {user.Name}"));
            }
        }

        internal void ProcMsg()
        {
            while (true)
            {
                var msg = Console.ReadLine();

                if (msg.StartsWith('#'))
                {
                    var cmd = msg.ToLower().Split(' ');
                    switch (cmd[0])
                    {
                        case ServiseCommands.SrvUsersList:
                            Console.WriteLine($"Waiting Users: {usersPool.Count}");
                            foreach (var user in usersPool)
                            {
                                Console.WriteLine($"Uder ID#{user.Key}: Name=\"{user.Value.Name}\"; Connected=\"{user.Value.Socket.Connected}\"");
                            }
                            break;
                    }
                }
                else
                {
                    foreach (var kv in usersPool)
                    {
                        kv.Value.Socket.Send(Encoding.Unicode.GetBytes(msg));
                    }
                }
            }
        }

        internal void Stop()
        {
            listener.Dispose();
        }
    }
    class ChatRoom
    {
        public ChatRoom(User u1, User u2)
        {
            User1 = u1;
            User2 = u2;

            User1.Room = this;
            User2.Room = this;
        }

        public User User1 { get; }
        public User User2 { get; }
    }
    class User
    {
        public int Id { get; private set; }
        public Socket Socket { get; private set; }
        public string Name { get; private set; }
        public DateTime LastDataReceiving { get; set; }
        public ChatRoom Room { get; internal set; }

        public User(int id, string name, Socket socket)
        {
            Id = id;
            Name = name;
            Socket = socket;
            LastDataReceiving = DateTime.Now;
        }
    }
}
