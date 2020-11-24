using System;

namespace Client
{
    class Program
    {
        static Client client;

        static void Main(string[] args)
        {
            client = new Client();

            try
            {
                client.Connecting();
                client.StartReceivingMessages();
                client.StartSendingMessages();
            }
            catch (Exception ex)
            {
                client.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}