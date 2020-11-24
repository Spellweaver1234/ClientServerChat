using System;

namespace Server
{
    class Program
    {
        static ServerObject server;
        static void Main(string[] args)
        {
            try
            {
                server = new ServerObject();
                server.StartReceivingMessages();
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
