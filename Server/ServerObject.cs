using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    public class ServerObject
    {
        private static TcpListener tcpListener; // сервер для прослушивания
        private Thread listenThread;

        public List<ClientObject> clients = new List<ClientObject>(); // все подключения

        internal void StartReceivingMessages()
        {
            listenThread = new Thread(new ThreadStart(Listen));
            listenThread.Start(); //старт потока
        }

        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }

        protected internal void RemoveConnectionById(string id)
        {
            // получаем по id закрытое подключение
            var clientObject = clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            if (clientObject != null)
                clients.Remove(clientObject);
        }

        // прослушивание входящих подключений
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    var tcpClient = tcpListener.AcceptTcpClient();
                    var clientObject = new ClientObject(tcpClient, this);
                    var thread = new Thread(new ThreadStart(clientObject.Process));
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // отправка сообщения всем 
        protected internal void BroadcastMessageToAll(string message, string id)
        {
            var data = Encoding.Unicode.GetBytes(message);

            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id) // если id клиента не равно id отправляющего
                {
                    clients[i].NetworkStream.Write(data, 0, data.Length); //передача данных
                }
            }
        }

        // отправка лично
        protected internal void BroadcastMessagePersonal(string message, string id)
        {
            var data = Encoding.Unicode.GetBytes(message);

            clients.SingleOrDefault(x => x.Id == id).NetworkStream.Write(data, 0, data.Length); //передача данных
        }

        // отключение всех клиентов
        protected internal void Disconnect()
        {
            tcpListener.Stop(); //остановка сервера

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //отключение клиента
            }

            listenThread.Abort();
            Environment.Exit(0); //завершение процесса
        }
    }
}
