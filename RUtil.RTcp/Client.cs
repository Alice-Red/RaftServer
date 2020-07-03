using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RUtil.RTcp
{
    public class Client
    {

        public delegate void MessageReceivedHundler(Client sender, MessageReceivedArgs e);
        public event MessageReceivedHundler MessageReceived;

        public delegate void DisConnectedHundler(Client sender, DisConnectedArgs e);
        public event DisConnectedHundler DisConnected;

        public static string[] DisconnectKeyWord = { };

        private string hostAddress;

        public string HostAddress {
            get { return hostAddress; }
            set { hostAddress = value; }
        }

        private int port;
        public int Port {
            get { return port; }
            set { port = value; }

        }

        private Socket server;

        public bool Connecting { get; private set; }

        public Client() {

        }

        public Client(string address, int port) {
            Create(address, port);
        }

        public void Create(string address, int port) {

            Connecting = false;
            if (Regex.IsMatch(address, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d+")) {
                HostAddress = address.Split(':')[0];
                Port = int.Parse(address.Split(':')[1]);
            } else if (Regex.IsMatch(address, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")) {
                HostAddress = address;
                Port = port;
            } else if (Regex.IsMatch(address, @"^[a-zA-Z0-9][a-zA-Z0-9-]{1,61}[a-zA-Z0-9]\.[a-zA-Z]{2,}:\d+$")) {
                HostAddress = System.Net.Dns.GetHostEntry(address.Split(':')[0]).AddressList[0].ToString();
                Port = int.Parse(address.Split(':')[1]);
            } else if (Regex.IsMatch(address, @"^[a-zA-Z0-9][a-zA-Z0-9-]{1,61}[a-zA-Z0-9]\.[a-zA-Z]{2,}$")) {
                HostAddress = System.Net.Dns.GetHostEntry(address).AddressList[0].ToString();
                Port = port;
            }


            //HostAddress = System.Net.Dns.GetHostEntry(address).AddressList[0].ToString();

            //HostAddress = address;
            //Port = port;
        }

        //非同期データ受信のための状態オブジェクト
        private class AsyncStateObject
        {
            public System.Net.Sockets.Socket Socket;
            public byte[] ReceiveBuffer;
            public System.IO.MemoryStream ReceivedData;

            public AsyncStateObject(System.Net.Sockets.Socket soc) {
                this.Socket = soc;
                this.ReceiveBuffer = new byte[1024];
                this.ReceivedData = new System.IO.MemoryStream();
            }
        }

        public void Boot() {
            //System.Net.Sockets.TcpClient tcp = new System.Net.Sockets.TcpClient(HostAddress, Port);
            server = new Socket(SocketType.Stream, ProtocolType.IP);
            server.Connect(IPAddress.Parse(HostAddress), Port);
            //System.Net.Sockets.NetworkStream ns = tcp.GetStream();
            Task.Factory.StartNew(() => {
                StartReceive(server);

            });
            //while (true) {
            //    Console.Write(">> ");
            //    var sendMsg = Console.ReadLine();
            //    System.Text.Encoding enc = System.Text.Encoding.UTF8;
            //    byte[] sendBytes = enc.GetBytes(sendMsg + '\n');
            //    //データを送信する
            //    tcp.Client.Send(sendBytes);
            //    //ns.Write(sendBytes, 0, sendBytes.Length);
            //    //Console.WriteLine(sendMsg);
            //}

        }

        public void Send(string sendMsg) {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            byte[] sendBytes = enc.GetBytes(sendMsg + '\n');
            //データを送信する
            server.Send(sendBytes);
        }


        //データ受信スタート
        private void StartReceive(System.Net.Sockets.Socket soc) {
            AsyncStateObject so = new AsyncStateObject(soc);
            Connecting = true;
            //非同期受信を開始
            soc.BeginReceive(so.ReceiveBuffer,
                0,
                so.ReceiveBuffer.Length,
                System.Net.Sockets.SocketFlags.None,
                new System.AsyncCallback(ReceiveDataCallback),
                so);
        }

        //BeginReceiveのコールバック
        private void ReceiveDataCallback(System.IAsyncResult ar) {
            try {
                //状態オブジェクトの取得
                AsyncStateObject so = (AsyncStateObject) ar.AsyncState;

                //読み込んだ長さを取得
                int len = 0;
                len = so.Socket.EndReceive(ar);

                //切断されたか調べる
                if (len <= 0) {
                    //System.Console.WriteLine("切断されました。");
                    DisConnected(this, new DisConnectedArgs(server.RemoteEndPoint.ToString()));
                    so.Socket.Close();
                    Connecting = false;
                    return;
                }

                //受信したデータを蓄積する
                so.ReceivedData.Write(so.ReceiveBuffer, 0, len);
                if (so.Socket.Available == 0) {
                    //最後まで受信した時
                    //受信したデータを文字列に変換
                    string str = System.Text.Encoding.UTF8.GetString(
                        so.ReceivedData.ToArray()).Trim('\r', '\n');
                    //受信した文字列を表示
                    //System.Console.WriteLine(str);
                    MessageReceived?.Invoke(this, new MessageReceivedArgs(server.RemoteEndPoint.ToString(), str));
                    so.ReceivedData.Close();
                    so.ReceivedData = new System.IO.MemoryStream();
                }

                //再び受信開始
                so.Socket.BeginReceive(so.ReceiveBuffer,
                    0,
                    so.ReceiveBuffer.Length,
                    System.Net.Sockets.SocketFlags.None,
                    new System.AsyncCallback(ReceiveDataCallback),
                    so);
            } catch (System.ObjectDisposedException) {
                //閉じた時
                //System.Console.WriteLine("閉じました。");
                DisConnected(this, new DisConnectedArgs(server.RemoteEndPoint.ToString()));
                Connecting = false;
                return;
            }
        }

        [Obsolete]
        public static void ClientMain() {


            //サーバーのIPアドレス（または、ホスト名）とポート番号
            //string ipOrHost = "192.168.43.151";
            string ipOrHost = "localhost";
            int port = 34481;

            bool exit = false;

            //var ipGlobalProp = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
            //var usedPorts = ipGlobalProp.GetActiveTcpConnections();
            //while (usedPorts.Select(s => s.LocalEndPoint.Port).Contains(port)) {
            //    port += 1;
            //}


            ////サーバーに送信するデータを入力してもらう
            //Console.Write(" >> ");
            //string sendMsg = Console.ReadLine();
            ////何も入力されなかった時は終了
            //if (sendMsg == null || sendMsg.Length == 0) {
            //    return;
            //}


            //TcpClientを作成し、サーバーと接続する
            System.Net.Sockets.TcpClient tcp = new System.Net.Sockets.TcpClient(ipOrHost, port);
            Console.WriteLine("サーバー({0}:{1})と接続しました({2}:{3})。",
                ((System.Net.IPEndPoint) tcp.Client.RemoteEndPoint).Address,
                ((System.Net.IPEndPoint) tcp.Client.RemoteEndPoint).Port,
                ((System.Net.IPEndPoint) tcp.Client.LocalEndPoint).Address,
                ((System.Net.IPEndPoint) tcp.Client.LocalEndPoint).Port);


            System.Net.Sockets.NetworkStream ns = tcp.GetStream();
            System.Text.Encoding enc = System.Text.Encoding.UTF8;

            while (!exit) {

                //サーバーに送信するデータを入力してもらう
                Console.Write(" >> ");
                string sendMsg = Console.ReadLine();
                //何も入力されなかった時は終了
                if (sendMsg == null || sendMsg.Length == 0 || DisconnectKeyWord.Contains(sendMsg)) {
                    exit = true;
                    //break;
                }

                //NetworkStreamを取得する

                //読み取り、書き込みのタイムアウトを10秒にする
                //デフォルトはInfiniteで、タイムアウトしない
                //(.NET Framework 2.0以上が必要)
                //ns.ReadTimeout = 10000;
                //ns.WriteTimeout = 10000;

                //Console.ReadKey(true);

                try {

                    //サーバーにデータを送信する
                    //文字列をByte型配列に変換
                    byte[] sendBytes = enc.GetBytes(sendMsg + '\n');
                    //データを送信する
                    ns.Write(sendBytes, 0, sendBytes.Length);
                    Console.Write(": ");



                    //サーバーから送られたデータを受信する
                    //bool disconnected = false;
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    byte[] resBytes = new byte[255];

                    int resSize;
                    do {
                        //データの一部を受信する
                        resSize = tcp.Client.Receive(resBytes);
                        //Readが0を返した時はサーバーが切断したと判断
                        if (resSize == 0) {
                            //disconnected = true;
                            Console.WriteLine("クライアントが切断しました。");
                            break;
                        }
                        //受信したデータを蓄積する
                        ms.Write(resBytes, 0, resSize);

                        //まだ読み取れるデータがあるか、データの最後が\nでない時は、
                        // 受信を続ける
                    } while (resBytes[resSize - 1] != '\n');
                    //受信したデータを文字列に変換
                    string resMsg = enc.GetString(ms.GetBuffer(), 0, (int) ms.Length);
                    ms.Close();
                    //末尾の\nを削除
                    resMsg = resMsg.TrimEnd('\n');

                    Console.CursorLeft = 0;
                    Console.WriteLine(resMsg);
                    Console.WriteLine(">>");

                } catch (Exception e) {
                    Console.WriteLine(e);
                    exit = true;
                }
            }


            //閉じる
            //ns.Close();
            //tcp.Close();
            Console.WriteLine("切断しました。");

            //Console.ReadLine();
        }
    }
}
