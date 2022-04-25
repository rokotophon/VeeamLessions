using Lab1._7;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lab1._7Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var login = string.Empty;
            while (true)
            {
                Console.Write("Enter your name: ");
                login = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(login) || login.Length > 50)
                    Console.WriteLine($"Error: \"{login}\" is'nt valid name.");
                else
                    break;
            }
            

            var client = new ChatClient();
            if (!client.Connect(IPAddress.Loopback, 35548, login))
                return;
            client.ProcMsg();

        }
    }
    class ChatClient
    {
        Socket socket;
        public bool Connect(IPAddress ip, int port, string login)
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(ip, port);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket.Connect(ipPoint);

                Send(login);

                Task.Run(Receive);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        private void Receive()
        {
            byte[] data = new byte[512];
            while (socket.Connected)
            {
                try
                {
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = socket.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (socket.Available > 0);

                    ProcessRecivedMessage(builder.ToString());
                }
                catch
                {
                    Console.WriteLine("Connection error!");
                    return;
                }
            }
        }

        private void ProcessRecivedMessage(string msg)
        {
            if (msg.StartsWith('#'))
            {
                var cmd = msg.ToLower().Split(' ');
                switch (cmd[0])
                {
                    case ServiseCommands.PingUser:
                        socket.Send(Encoding.Unicode.GetBytes(ServiseCommands.PingUser));
                        break;
                }
            }
            else
            {
                Console.WriteLine(msg);
            }

        }

        internal void ProcMsg()
        {
            while (true)
            {
                Send(Console.ReadLine());
            }
        }

        private void Send(string msg)
        {
            byte[] data = Encoding.Unicode.GetBytes(msg);
            socket.Send(data);
        }
    }

}
