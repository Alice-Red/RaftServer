using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace RUtil.RTcp
{
    public class Server
    {
        /// <summary>
        /// サーバーが起動した際に呼び出されます
        /// </summary>
        /// <param name="sender">呼び出したサーバークラス</param>
        /// <param name="e"></param>
        public delegate void ServerAwakenedHandler(Server sender, ServerAwakedArgs e);
        public event ServerAwakenedHandler ServerAwakened;

        /// <summary>
        /// メッセージを受け取った際に呼び出されます
        /// </summary>
        /// <param name="sender">呼び出したサーバークラス</param>
        /// <param name="e"></param>
        public delegate void MessageReceivedHandler(Server sender, MessageReceivedArgs e);
        public event MessageReceivedHandler MessageReceived;

        /// <summary>
        /// データを受け取った際に呼び出されます
        /// </summary>
        /// <param name="sender">呼び出したサーバークラス</param>
        /// <param name="e"></param>
        public delegate void DataReceivedHandler(Server sender, DataReceivedArgs e);
        public event DataReceivedHandler DataReceived;

        /// <summary>
        /// クライアントと接続した際に呼び出されます
        /// </summary>
        /// <param name="sender">呼び出したサーバークラス</param>
        /// <param name="e"></param>
        public delegate void ConnectionSuccessfulHandler(Server sender, ConnectionSuccessfullArgs e);
        public event ConnectionSuccessfulHandler ConnectionSuccessful;

        /// <summary>
        /// クライアントとの接続が切られたときに呼び出されます
        /// </summary>
        /// <param name="sender">呼び出したサーバークラス</param>
        /// <param name="e"></param>
        public delegate void DisConnectedHandler(Server sender, DisConnectedArgs e);
        public event DisConnectedHandler DisConnected;

        /// <summary>
        /// 現在の接続数が変更された際に呼び出されます
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ConnectedCountChangedHandler(Server sender, ConnectedCountChangedArgs e);
        public event ConnectedCountChangedHandler ConnectedCountChanged;

        /// <summary>
        /// ポートを設定します
        /// </summary>
        public int Port { get; set; }


        /// <summary>
        /// 現在の接続数を返します
        /// </summary>
        public int ConnectedCount {
            get => connectedCount;
            private set {
                if (connectedCount != value)
                    ConnectedCountChanged?.Invoke(this, new ConnectedCountChangedArgs(value));
                connectedCount = value;
            }
        }
        private int connectedCount = 0;

        // 強制終了
        private bool ForcedTermination = false;

        /// <summary>
        /// タイムアウトの時間を設定します
        /// </summary>
        public long TimeoutMilliSec = 10_000;

        private Dictionary<string, Socket> ConnectingList = new Dictionary<string, Socket>();

        /// <summary>
        /// コンストラクタ１
        /// </summary>
        public Server() { connectedCount = 0; }

        /// <summary>
        /// コンストラクタ２
        /// </summary>
        /// <param name="port">バインドするポート</param>
        public Server(int port) : base() { Create(port); }

        /// <summary>
        /// サーバーを作成します
        /// </summary>
        /// <param name="port">バインドするポート</param>
        public void Create(int port) { Port = port; }

        /// <summary>
        /// サーバーを起動します
        /// </summary>
        public void Boot() {
            Socket server = new Socket(SocketType.Stream, ProtocolType.IP);
            server.Bind(new IPEndPoint(IPAddress.Any, Port));
            server.Listen(8);

            Task.Factory.StartNew(() => { StartAccept(server); });
        }

        // 受け入れを開始
        private void StartAccept(Socket server) {
            string hostname = Dns.GetHostName();
            ServerAwakened?.Invoke(this, new ServerAwakedArgs(new string[] { server.LocalEndPoint.ToString() }, Port));
            server.BeginAccept(new AsyncCallback(AcceptCallback), server);
        }

        // 少しは改善されたと信じている
        private void AcceptCallback(IAsyncResult ar) {
            if (ForcedTermination)
                return;

            var server = (Socket)ar.AsyncState;
            Socket client;

            var ipadd = "";
            client = server.EndAccept(ar);

            server.BeginAccept(new AsyncCallback(AcceptCallback), server);
            ConnectionSuccessful?.Invoke(this, new ConnectionSuccessfullArgs(client.RemoteEndPoint.ToString()));
            ConnectingList.Add(client.RemoteEndPoint.ToString(), client);
            ConnectedCount++;

            ipadd = client.RemoteEndPoint.ToString();
            var sw = new Stopwatch();
            sw.Start();

            while (sw.ElapsedMilliseconds < TimeoutMilliSec) {
                using MemoryStream ms = new System.IO.MemoryStream();
                byte[] resBytes = new byte[255];
                int resSize = 0;

                // 強制的に切断された時用の try-catch
                try {
                    do {
                        if (TimeoutMilliSec <= sw.ElapsedMilliseconds) {
                            break;
                        }
                        resSize = client.Receive(resBytes);
                        if (resSize == 0) {
                            continue;
                        }

                        ms.Write(resBytes, 0, resSize);
                        sw.Restart();
                    } while (resBytes[resSize - 1] != '\n');
                    var resData = resBytes.ToArray();
                    DataReceived?.Invoke(this, new DataReceivedArgs(ipadd, resData));
                    var resMsg = Encoding.UTF8.GetString(ms.ToArray()).Trim('\r', '\n');
                    MessageReceived?.Invoke(this, new MessageReceivedArgs(ipadd, resMsg));
                } catch (Exception e) {
                    // 受信中に切断された場合 System.IndexOutOfRangeException が発動する
                    // タイミングによっては Socket の例外が来る
                    Debug.WriteLine(e);
                    break;
                }
            }

            sw.Stop();
            //Disconnect(ipadd);
        }

        /// <summary>
        /// 未定
        /// </summary>
        /// <param name="cmd"></param>
        [Obsolete]
        public void Command(string cmd) { }

        /// <summary>
        /// 接続しているクライアントすべてにテキストメッセージを一斉送信します
        /// </summary>
        /// <param name="message">送信するメッセージ</param>
        public void SendAll(string message) {
            foreach (var item in ConnectingList) {
                Send(item.Value, message);
            }
        }

        /// <summary>
        /// 指定したIPアドレスを持つ接続中のクライアントにテキストメッセージを送信します
        /// </summary>
        /// <param name="ip">指定するIPアドレス</param>
        /// <param name="message">送信するメッセージ</param>
        public void Send(string ip, string message) {
            if (ConnectingList.ContainsKey(ip)) {
                Send(ConnectingList[ip], message);
            }
        }

        /// <summary>
        /// 指定したIPアドレスを持つ接続中のクライアントにデータを送信します
        /// </summary>
        /// <param name="ip">指定するIPアドレス</param>
        /// <param name="data">送信するデータ</param>
        public void Send(string ip, byte[] data) {
            if (ConnectingList.ContainsKey(ip)) {
                Send(ConnectingList[ip], data);
            }
        }

        /// <summary>
        /// 指定したソケットにテキストメッセージを送信します
        /// </summary>
        /// <param name="target">指定するソケット</param>
        /// <param name="message">送信するメッセージ</param>
        private static void Send(Socket target, string message) {
            byte[] sendBytes = Encoding.UTF8.GetBytes(message);
            target?.Send(sendBytes);
        }

        /// <summary>
        /// 指定したソケットにデータを送信します
        /// </summary>
        /// <param name="target">指定するソケット</param>
        /// <param name="data">送信するデータ</param>
        private static void Send(Socket target, byte[] data) { target?.Send(data); }

        /// <summary>
        /// 指定した条件に合うソケットに対しテキストメッセージを送信します
        /// </summary>
        /// <param name="target">指定するソケットの条件</param>
        /// <param name="message">送信するメッセージ</param>
        public void Send(Func<Socket, int, bool> target, string message) {
            byte[] sendBytes = Encoding.UTF8.GetBytes(message);
            for (int i = 0; i < ConnectingList.Count(); i++) {
                if (target(ConnectingList.ElementAt(i).Value, i)) {
                    ConnectingList.ElementAt(i).Value.Send(sendBytes);
                }
            }
        }

        /// <summary>
        /// 指定したIPのクライアントとの接続を切断します
        /// </summary>
        /// <param name="ip">指定するIPアドレス</param>
        public void Disconnect(string ip) {
            if (!ConnectingList.ContainsKey(ip))
                return;
            ConnectingList[ip].Disconnect(true);
            // ConnectingList[ip].Shutdown(SocketShutdown.Both);
            ConnectingList[ip].Close();
            DisConnected?.Invoke(this, new DisConnectedArgs(ip));
            ConnectingList.Remove(ip);
            ConnectedCount -= 1;
        }

        /// <summary>
        /// 指定した条件に合うソケットとの接続を切断します
        /// </summary>
        /// <param name="target"></param>
        public void Disconnect(Func<Socket, int, bool> target) {
            for (int i = 0; i < ConnectingList.Count(); i++) {
                if (target(ConnectingList.ElementAt(i).Value, i)) {
                    ConnectingList.ElementAt(i).Value.Disconnect(true);
                }
            }
        }

        /// <summary>
        /// サーバーを停止させます
        /// </summary>
        public void ShutDown() {
            ForcedTermination = true;
            foreach (var item in ConnectingList) {
                Disconnect(item.Key);
            }
        }
    }
}