using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream NetworkStream { get; private set; }
        string userName;
        TcpClient tcpClient;
        ServerObject serverObject; // объект сервера

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            this.tcpClient = tcpClient;
            this.serverObject = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                NetworkStream = tcpClient.GetStream();
                // получаем имя пользователя
                var message = GetMessage();
                userName = message;

                // посылаем пользователю список команд
                var welcomeMessage =
                    $"Добро пожаловать {userName}" +
                    $"\n{UserCommands.HelpMessage()}" +
                    $"\nВведите сообщение: ";
                serverObject.BroadcastMessagePersonal(welcomeMessage, Id);

                // оповещаем всех пользователей о новичке в чате
                var newUserMessage = userName + " вошел в чат";
                serverObject.BroadcastMessageToAll(newUserMessage, Id);

                Console.WriteLine(newUserMessage);

                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        DecodingMessage(message);
                    }
                    catch
                    {
                        message = string.Format("{0}: покинул чат", userName);
                        Console.WriteLine(message);
                        serverObject.BroadcastMessageToAll(message, Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                serverObject.RemoveConnectionById(Id);
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку
        private string GetMessage()
        {
            var data = new byte[64]; // буфер для получаемых данных
            var stringBuilder = new StringBuilder();

            do
            {
                var bytes = NetworkStream.Read(data, 0, data.Length);
                stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (NetworkStream.DataAvailable);

            return stringBuilder.ToString();
        }

        private void DecodingMessage(string message)
        {
            switch (message)    // обработка команд 
            {
                case UserCommands.Bye:
                    message = UserCommands.ByeMessage();
                    serverObject.BroadcastMessagePersonal(message, Id);
                    serverObject.BroadcastMessagePersonal(UserCommands.Bye, Id);
                    serverObject.RemoveConnectionById(Id);
                    Close();
                    break;
                case UserCommands.All:
                    message = UserCommands.AllMessage(serverObject.clients.Select(e => e.userName).ToList());
                    serverObject.BroadcastMessagePersonal(message, Id);
                    break;
                case UserCommands.Count:
                    message = UserCommands.CountMessage(serverObject.clients.Count);
                    serverObject.BroadcastMessagePersonal(message, Id);
                    break;
                case UserCommands.Time:
                    message = UserCommands.TimeMessage();
                    serverObject.BroadcastMessagePersonal(message, Id);
                    break;
                case UserCommands.Help:
                    message = UserCommands.HelpMessage();
                    serverObject.BroadcastMessagePersonal(message, Id);
                    break;
                default:    // по умолчанию рассылаем всем сообщение 
                    message = string.Format("{0}: {1}", userName, message);
                    Console.WriteLine(message);
                    serverObject.BroadcastMessageToAll(message, Id);
                    break;
            }
        }

        // закрытие подключения
        protected internal void Close()
        {
            if (NetworkStream != null)
                NetworkStream.Close();
            if (tcpClient != null)
                tcpClient.Close();
        }
    }
}
