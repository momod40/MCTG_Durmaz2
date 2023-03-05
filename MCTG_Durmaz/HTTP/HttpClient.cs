using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Durmaz.HTTP
{
    public class HttpClient
    {
        private readonly TcpClient _connection;

        public HttpClient(TcpClient connection)
        {
            _connection = connection;
        }

        public RequestContext? ReceiveRequest()
        {
            var reader = new StreamReader(_connection.GetStream());
            var first = true;

            HttpMethod method = HttpMethod.Get;
            string? path = null;
            string? version = null;
            string? userToken = null;
            Dictionary<string, string> header = new();
            var contentLength = 0;
            string? payload = null;

            try
            {
                // read header
                string? line;

                // read line for line until the end or until we reach an empty line
                while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
                {
                    line.Trim();

                    // if first line
                    if (first)
                    {
                        Console.WriteLine(line);
                        // read method and version
                        var info = line.Split(' ');
                        if (info.Length != 3)
                        {
                            throw new InvalidDataException();
                        }
                        method = MethodUtilities.GetMethod(info[0].Trim());
                        path = info[1];
                        version = info[2];

                        first = false;
                    }
                    else if (line.StartsWith("Authorization"))
                    {
                        string[] authorizationParts = line.Split(' ');
                        //Console.WriteLine($"DAS IST DIE LINE {user}");
                        userToken = authorizationParts[2];
                    }
                    else
                    {
                        Console.WriteLine(line);
                        // read HTTP headers
                        var info = line.Split(':', 2);
                        header.Add(info[0].Trim(), info[1].Trim());
                        if (info[0] == "Content-Length")
                        {
                            contentLength = int.Parse(info[1]);
                        }
                    }
                }
            }
            catch (Exception e) when (e is IOException || e is InvalidDataException)
            {
                return null;
            }

            if (path == null || version == null)
            {
                return null;
            }

            // Read optional http body
            if (contentLength > 0 && header.ContainsKey("Content-Type"))
            {
                var data = new StringBuilder();
                var buffer = new char[1024];
                var totalBytesRead = 0;
                while (totalBytesRead < contentLength)
                {
                    try
                    {
                        var bytesRead = reader.Read(buffer, 0, 1024);
                        totalBytesRead += bytesRead;
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        data.Append(buffer, 0, bytesRead);
                    }
                    catch (IOException)
                    {
                        break;
                    }
                }
                payload = data.ToString();

            }


            // build RequestContext from data
            return new RequestContext()
            {
                Method = method,
                ResourcePath = path,
                token = userToken,
                HttpVersion = version,
                Header = header,
                Payload = payload
            };
        }

        public void SendResponse(Response response)
        {
            // https://stackoverflow.com/questions/5757290/http-header-line-break-style

            var writer = new StreamWriter(_connection.GetStream());
            writer.Write($"HTTP/1.1 {(int)response.StatusCode} {response.StatusCode}\r\n");
            if (!string.IsNullOrEmpty(response.Payload))
            {
                var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response.Payload));
                writer.Write($"Content-Length: {payload.Length}\r\n");
                writer.Write("\r\n");
                writer.Write(Encoding.UTF8.GetString(payload));
                writer.Close();
                ;
            }
            else
            {
                writer.Write("\r\n");
                writer.Close();
            }
        }

        public void Close()
        {
            _connection.Close();
        }
    }
}
