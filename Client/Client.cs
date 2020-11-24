using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Client
    {
        private string userName;
        private readonly string host = "127.0.0.1";
        private readonly int port = 8888;
        private TcpClient tcpClient;
        private NetworkStream networkStream;

        public void Connecting()
        {
            Console.Write("Введите свое имя: ");
            userName = Console.ReadLine();
            tcpClient = new TcpClient();
            tcpClient.Connect(host, port); //подключение клиента
            networkStream = tcpClient.GetStream(); // получаем поток

            var message = userName;
            var data = Encoding.Unicode.GetBytes(message);
            networkStream.Write(data, 0, data.Length);
        }

        public void StartReceivingMessages()
        {
            var receiveThread = new Thread(new ThreadStart(ReceivingMessages));
            receiveThread.Start();
        }

        public void StartSendingMessages()
        {
            while (true)
            {
                var message = Console.ReadLine();
                var data = Encoding.Unicode.GetBytes(message);
                networkStream.Write(data, 0, data.Length);
            }
        }

        private void ReceivingMessages()
        {
            while (true)
            {
                try
                {
                    var data = new byte[64]; // буфер для получаемых данных
                    var builder = new StringBuilder();
                    var bytes = 0;

                    do
                    {
                        bytes = networkStream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (networkStream.DataAvailable);

                    var message = builder.ToString();
                    DecodingMessage(message);
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!"); //соединение было прервано
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        private void DecodingMessage(string message)
        {
            switch (message)    // обработка команд 
            {
                case ServerCommands.Bye:
                    message = ServerCommands.AllCommands[ServerCommands.Bye];
                    Disconnect();
                    break;
            }

            Console.WriteLine(message);
        }

        public void Disconnect()
        {
            if (networkStream != null)
                networkStream.Close();//отключение потока
            if (tcpClient != null)
                tcpClient.Close();//отключение клиента
        }
    }
}
