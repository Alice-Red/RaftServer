using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace RaftServer
{
    public class HttpResponseObject
    {

        public string HttpVersion { get; set; }
        public int ResponseCode { get; set; }
        public Dictionary<string, string> Header { get; private set; }

        private byte[] ingredients;
        public byte[] Ingredients {
            get => ingredients;
            set { ingredients = value; }
        }

        /// <summary>
        /// コンストラクタ１
        /// </summary>
        /// <param name="httpVersion">HTTPバージョン</param>
        public HttpResponseObject(string httpVersion) {
            HttpVersion = httpVersion;
            ResponseCode = 418;
            Header = new Dictionary<string, string>();
            ingredients = new byte[0];
        }

        /// <summary>
        /// コンストラクタ２
        /// </summary>
        /// <param name="httpVersion">HTTPバージョン</param>
        /// <param name="code">レスポンスコード</param>
        public HttpResponseObject(string httpVersion, int code) {
            HttpVersion = httpVersion;
            ResponseCode = code;
            Header = new Dictionary<string, string>();
            ingredients = new byte[0];
        }

        /// <summary>
        /// ヘッダーを登録します
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void StoreHeader(string key, string value) {
            if (!Header.ContainsKey(key)) {
                Header.Add(key, value);
            } else {
                Header[key] = value;
            }
        }

        /// <summary>
        /// 指定したヘッダーを削除します
        /// </summary>
        /// <param name="key"></param>
        public void DeleteHeader(string key) {
            if (Header.ContainsKey(key)) {
                Header.Remove(key);
            }
        }

        /// <summary>
        /// 指定したヘッダーの値を取得します
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string HeaderValue(string key) {
            if (Header.ContainsKey(key)) {
                return Header[key];
            } else {
                return "";
            }
        }

        /// <summary>
        /// ディレクトリを指定された場合にindexのファイルを指定します
        /// </summary>
        /// <param name="path"></param>
        public void StoreFile(string path) {
            if (!File.Exists(path) && path.LastOrDefault() == '/') {
                var indexes = new string[] { "index.html", "index.php" };
                foreach (var item in indexes) {
                    if (File.Exists(path + item)) {
                        path += item;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// レスポンス用のデータをバイト配列に変換します
        /// </summary>
        /// <returns>byte[] に変換されたレスポンスデータ</returns>
        public byte[] ToByteArrayAll() {
            var head = Encoding.UTF8.GetBytes(HeaderToString());
            return head.Concat(Ingredients).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string HeaderToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"HTTP/{HttpVersion} {ResponseCode} {HttpResponse.Code[ResponseCode]}");
            foreach (var item in Header) {
                sb.AppendLine($"{item.Key}: {item.Value}");
            }

            sb.AppendLine();
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetModifired() {
            return GetModifired("");
        }

        public static string GetModifired(string path) {

            //  Wed, 21 Oct 2015 07:28:00 GMT 
            string format = @"ddd, dd MMM yyyy hh:mm:ss GMT";

            // temporary
            if (!File.Exists(path)) {
                return new DateTime(0).ToString(format);
            }

            return File.GetLastWriteTimeUtc(path).ToString(format);
        }

    }
}