using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCTG_Durmaz.HTTP
{
    public class HttpServer
    {
        private bool _listening;

        private readonly TcpListener _listener;
        private readonly Router _router;

        public HttpServer(IPAddress address, int port, Router router)
        {
            _listener = new TcpListener(address, port);
            _router = router;
        }

        public void Start()
        {
            _listener.Start();
            _listening = true;

            while (_listening)
            {
                var connection = _listener.AcceptTcpClient();
                Console.WriteLine("client connected");

                // create a new thread to handle the client connection
                var clientThread = new Thread(() =>
                {
                    var client = new HttpClient(connection);

                    var request = client.ReceiveRequest();

                    Response response = new Response();

                    var command = _router.Resolve(request);

                    response = command.Execute();

                    client.SendResponse(response);
                });

                clientThread.Start();
                
            }
        }

        public void Stop()
        {
            _listening = false;
            _listener.Stop();
        }
    }
}
