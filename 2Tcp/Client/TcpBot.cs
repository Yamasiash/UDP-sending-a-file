using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{

    class TcpBot
    {
        string username;
        TcpClient client;

        public TcpBot(string address, int port)
        {
            Console.WriteLine("Enter name");
            username = Console.ReadLine();
            try
            {
                client = new TcpClient(address, port);
                while (true)
                {
                    Console.Write($"{username}: ");
                    SendMessage(Console.ReadLine());
                    Console.WriteLine(ReceiveMessage());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public string ReceiveMessage()
        {
            string message;
            NetworkStream stream = null;
            stream = client.GetStream();
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);
            message = builder.ToString();
            return message;
        }

        public void SendMessage(string message)
        {
            NetworkStream stream = null;
            stream = client.GetStream();
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        static void Main(string[] args)
        {
            string address = "10.0.176.2";//Console.ReadLine();
            int port = 8888;//Int32.Parse(Console.ReadLine());
            TcpBot chat = new TcpBot(address, port);
        }

    }
}
