using System;
using System.IO;
using System.Threading;
using RUtil.RTcp;

namespace RaftServer
{
    public class RaftServer
    {
        private Server server;
        private const string targetPath = "target";
        

        public RaftServer() {
            server = new Server(12345);
            server.MessageReceived += Server_MessageReceived;
            server.ConnectionSuccessfull += Server_Connected;
            server.Boot();
            Console.WriteLine($"serverStarted --> [ http://127.0.0.1:{server.Port} ]");
        }

        public void Stop() {
            server.ShutDown();
        }
        
        private void Server_Connected(Server sender, ConnectionSuccessfullArgs e) {
            Console.WriteLine($"{e.IpAddress} : Entry");
        }

        private void Server_Disconnected(Server sender, DisConnectedArgs e) {
            Console.WriteLine($"{e.IpAddress} : Exit");
        }
        
        private void Server_MessageReceived(Server sender, MessageReceivedArgs e) {
            // Console.WriteLine(e.IpAddress);
            // Console.WriteLine(e.Message);
            HttpRequestObject req = new HttpRequestObject(e.Message);
            HttpResponseObject res = new HttpResponseObject("1.1");
            Console.WriteLine($"{e.IpAddress} =-= {req.Path}");
            // Console.WriteLine($"");
            
            
            string localPath = targetPath + req.Path;
            if (File.Exists(localPath)) {
                res.ResponseCode = 200;
            
                var ext = Path.GetExtension(localPath);
                res.StoreHeader("Content-Type", Extension.ToContentType[ext.Trim('.')] + ";");
                res.StoreHeader("Connection", "keep-alive");
            
                using (FileStream fs = new FileStream(targetPath + req.Path, FileMode.Open,FileAccess.Read)) {
                    byte[] bs = new byte[fs.Length];
                    fs.Read(bs, 0, bs.Length);
                    res.Ingredients = bs;
                }
            } else {
                res.ResponseCode = 404;
            }
            
            Console.WriteLine($"{req.Path} : {res.ResponseCode}");
            sender.Send(e.IpAddress,res.ToByteArrayAll());


            // sender.Send(e.IpAddress, "HTTP/1.1 200 OK\r\n");
            // sender.Send(e.IpAddress, "Content-Type: text/plain; charset=UTF-8\r\n");
            // sender.Send(e.IpAddress, "Connection: close\r\n");
            // sender.Send(e.IpAddress, "\r\n");
            // sender.Send(e.IpAddress, "Copyright (C) Redkun. 2020\r\n");
            // sender.Send(e.IpAddress, $"now is {DateTime.Now}\r\n");
            //
            // sender.Disconnect(e.IpAddress);
        }
    }
}