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
            set {
                ingredients = value;
                //StoreHeader("Content-Length", ingredients.Length.ToString());
            }
        }

        public HttpResponseObject(string httpVersion) {
            HttpVersion = httpVersion;
            ResponseCode = 418;
            Header = new Dictionary<string, string>();
            ingredients = new byte[0];
        }

        public HttpResponseObject(string httpVersion, int code) {
            HttpVersion = httpVersion;
            ResponseCode = code;
            Header = new Dictionary<string, string>();
            ingredients = new byte[0];
        }

        public void StoreHeader(string key, string value) {
            if (!Header.ContainsKey(key)) {
                Header.Add(key, value);
            } else {
                Header[key] = value;
            }
        }

        public void DeleteHeader(string key) {
            if (Header.ContainsKey(key)) {
                Header.Remove(key);
            }
        }

        public string HeaderValue(string key) {
            if (Header.ContainsKey(key))
                return Header[key];
            else
                return "";
        }

        public void StoreFile(string path) {
            if (!File.Exists(path))
                if (path.LastOrDefault() == '/') {
                    var indexes = new string[] { "index.html", "index.php" };
                    foreach (var item in indexes) {
                        if (File.Exists(path + item)) {
                            path += item;
                            break;
                        }
                    }
                } else
                    return;







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
            sb.AppendLine($"HTTP/{HttpVersion} {ResponseCode} {HttpResponseCode.ResponseCode[ResponseCode]}");
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