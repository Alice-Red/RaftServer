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
            //server.ConnectionSuccessfull += Server_Connected;
            //server.DisConnected += Server_Disconnected;
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
            //Console.WriteLine($"{e.IpAddress} =-= {req.Path}");
            // Console.WriteLine($"");
            bool close = false;

            string localPath = targetPath + req.Path;
            if (File.Exists(localPath)) {
                res.ResponseCode = 200;

                var ext = Path.GetExtension(localPath);
                res.StoreHeader("Content-Type", Extension.ToContentType[ext.Trim('.')] + ";");

                using (FileStream fs = new FileStream(targetPath + req.Path, FileMode.Open, FileAccess.Read)) {
                    byte[] bs = new byte[fs.Length];
                    fs.Read(bs, 0, bs.Length);
                    res.Ingredients = bs;
                }

                switch (req.HeaderValue("Connection")) {
                    case "keep-alive":
                        res.StoreHeader("Connection", "keep-alive");
                        res.StoreHeader("Content-Length", res.Ingredients.Length.ToString());
                        break;
                    case "close":
                        res.StoreHeader("Connection", "close");
                        close = true;
                        break;
                }

            } else if (req.RqType == RequestType.Err) {
                res.ResponseCode = 400;
            } else {
                res.ResponseCode = 404;
            }

            //Console.WriteLine($"{req.Path} : {res.ResponseCode}");
            sender.Send(e.IpAddress, res.ToByteArrayAll());

            if (close)
                sender.Disconnect(e.IpAddress);
        }
    }
}