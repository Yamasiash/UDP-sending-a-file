using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using _2Tcp;

namespace TcpServer
{
    class BasicHttpHandler : TcpHandler
    {
        public BasicHttpHandler(TcpClient client) : base(client)
        { }

        public string GETResponse(HttpStatusCode code, string contentType, int length)
        {
            StringBuilder headers = new StringBuilder().
                AppendLine("HTTP/1.1 " + ((Int16)code).ToString() + " " + code.ToString()).
                AppendLine("Content-type: " + contentType).
                AppendLine("Content-Length: " + length.ToString()).
                AppendLine();
            return headers.ToString();
        }

        public string GETRequest()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = client.GetStream().Read(data, 0, data.Length);
                builder.Append(Encoding.ASCII.GetString(data, 0, bytes));
            }
            while (client.GetStream().DataAvailable);
            string message = builder.ToString();
            string lines = message.Split('\n')[0];
            string[] headers = lines.Split(' ');
            foreach (var header in headers)
                header.Trim();
            if (headers[0] != "GET")
                throw new HttpListenerException(400);
            return headers[1].Substring(1);
            
        }

        public override void Process()
        {
            try
            {
                NetworkStream output = client.GetStream();
                string body3 = GETRequest();

               
                string body = body3;
                string response = GETResponse(HttpStatusCode.OK, "text/html", body.Length);
                Console.WriteLine(response);
                byte[] buffer = Encoding.ASCII.GetBytes(response + body);
                output.Write(buffer, 0, buffer.Length);

                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                client.Close();
            }
        }
    }
}
