using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UdpClientApp
{
    class Program
    {
        static string remoteAddress = "192.168.0.14"; // хост для отправки данных
                                                     //(Только не ищите меня по IP плес)
        static int remotePort = 8001; // порт для отправки данных
        static int localPort = 8001; // локальный порт для прослушивания входящих подключений

        static void Main(string[] args)
        {
            try
            {
            //    Console.Write("Введите порт для прослушивания: "); // локальный порт
            //    localPort = Int32.Parse(Console.ReadLine());
            //    Console.Write("Введите удаленный адрес для подключения: ");
            //    remoteAddress = Console.ReadLine(); // адрес, к которому мы подключаемся
            //    Console.Write("Введите порт для подключения: ");
            //    remotePort = Int32.Parse(Console.ReadLine()); // порт, к которому мы подключаемся

                Thread receiveThread = new Thread(new ThreadStart(SendMessage));
                receiveThread.Start();
                ReceiveMessage(); // отправляем сообщение
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void SendMessage()
        {
            UdpClient sender = new UdpClient(); // создаем UdpClient для отправки сообщений
            try
            {
                string fileName = Console.ReadLine();
                FileInfo info = new FileInfo(fileName);
                string message = $"{fileName}:{info.Length}";
                byte[] data = Encoding.Unicode.GetBytes(message);
                sender.Send(data, data.Length, remoteAddress, remotePort);
                FileStream input = info.OpenRead();
                int count = 1024;
                //int offset = 0;
                data = new byte[count];

                while (input.Read(data, 0, count) > 0)
                {
                    //string fileName = "filename";
                    //FileInfo info = new FileInfo(fileName);
                    //string message = Console.ReadLine(); // сообщение для отправки
                    //byte[] data = Encoding.Unicode.GetBytes(message);
                    sender.Send(data, data.Length, remoteAddress, remotePort); // отправка
                    //offset += count;
                }
                input.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sender.Close();
            }
        }

        private static void ReceiveMessage()
        {
            UdpClient receiver = new UdpClient(localPort); // UdpClient для получения данных
            IPEndPoint remoteIp = null; // адрес входящего подключения
            try
            {
                while (true)
                {
                    byte[] data = receiver.Receive(ref remoteIp); // Вывод кол-ва символов
                    string[] str = Encoding.Unicode.GetString(data).Split(':');
                    //string message = Encoding.Unicode.GetString(data);
                    int lenght = Int32.Parse(str[1]);
                    Console.WriteLine($"{remoteIp.Address}{lenght}");
                    Console.WriteLine("Check outputFile");

                    int recieved = 0;
                    FileStream output = File.Create(@"./folder/outputFile.txt");
                    while(recieved<lenght)
                    {
                        receiver.Receive(ref remoteIp);
                        output.Write(data, 0, data.Length);
                        recieved += data.Length;
                    }
                    output.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                receiver.Close();
            }
        }
    }
}