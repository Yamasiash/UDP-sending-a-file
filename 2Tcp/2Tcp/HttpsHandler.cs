using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TcpServer;

namespace _2Tcp
{
    class HttpHandler : BasicHttpHandler
    {
        string rootPath = "";
        public HttpHandler(string root, TcpClient client) : base(client)
        {
            rootPath = root;
        }

        public string ContentType(string filepath)
        {
            string ext = Path.GetExtension(filepath);
            switch(ext)
            {
                case ".htm":
                case ".html":
                    return "text/html";
                case ".css":
                    return "text/stylesheet";
                case ".js":
                    return "text/javascript";
                case ".jpg":
                    return "image/jpeg";
                case ".jpeg":
                case ".png":
                case ".gif":
                    return $"image/{ext.Substring(1)}";
                default:
                    if (ext.Length > 1)
                    {
                        return $"application/{ext.Substring(1)}";
                    }
                    else
                        return "application/unknown";
            }
        }
        public override void Process()
        {
            string header = String.Empty;
            string body = String.Empty;
            FileStream content = null;
            try
            {
                string requestUrl = GETRequest();
                requestUrl = Uri.UnescapeDataString(requestUrl);
                if (requestUrl.EndsWith("/"))
                {
                    requestUrl = "index.html";
                }

                string filePath = Path.Combine(rootPath, requestUrl);
                Console.WriteLine(filePath);

                if(!File.Exists(filePath))
                {
                    throw new HttpListenerException(404, filePath);
                }

                try
                {
                    content = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    string contentType = ContentType(filePath);
                    header = GETResponse(HttpStatusCode.OK, contentType, (int)content.Length);
                    byte[] headerBuffer = Encoding.UTF8.GetBytes(header);
                    client.GetStream().Write(headerBuffer, 0, headerBuffer.Length);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if(content!=null)
                    {
                        byte[] buffer = new byte[1024];
                        var outp = client.GetStream();
                        while(content.Position < content.Length)
                        {
                            int count = content.Read(buffer, 0, buffer.Length);
                            outp.Write(buffer, 0, count);
                        }
                        content.Close();
                    }
                    else
                    {
                    }
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
