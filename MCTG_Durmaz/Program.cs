using MCTG_Durmaz.DB;
using MCTG_Durmaz.HTTP;
using System;
using System.Net;

namespace MCTG_Durmaz
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello world");
            Datenbank db = new Datenbank();
            db.createTables();
            Router router = new Router();
            HttpServer server = new HttpServer(IPAddress.Loopback, 10001, router);
            server.Start();
        }
    }
}
