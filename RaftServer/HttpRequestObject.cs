using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RaftServer
{
    public class HttpRequestObject
    {
        /// <summary>
        /// 文字列とリクエストタイプを連動させます
        /// </summary>
        internal static Dictionary<string, RequestType> RequestDictionary = new Dictionary<string, RequestType>() {
            {"GET", RequestType.Get},
            {"PUT", RequestType.Put},
            {"POST", RequestType.Post},
            {"HEAD", RequestType.Head},
            {"OPTIONS", RequestType.Options},
            {"DELETE", RequestType.Delete}
        };

        /// <summary>
        /// リクエストタイプ
        /// </summary>
        public RequestType RqType { get; private set; }

        /// <summary>
        /// ファイルパス
        /// </summary>
        public string Path { get; private set; }
        public string HttpVersion { get; private set; }
        public Dictionary<string, string> Header { get; private set; }
        public string Ingredients { get; private set; }

        public HttpRequestObject(string request) {
            RqType = RequestType.Err;
            Path = @"/";
            HttpVersion = "1.1";
            Header = new Dictionary<string, string>();
            Ingredients = "";
            Parse(request);
        }

        private void Parse(string request) {
            var RequestPerLine = request.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');

            bool isHeader = false;
            StringBuilder sb = new StringBuilder();

            // リクエストを1行ずつ処理
            for (int i = 0; i < RequestPerLine.Length; i++) {
                if (i == 0) {
                    // GET / HTTP/1.1
                    //     to
                    // [GET], [/], [HTTP/1.1]
                    var firstline = RequestPerLine[i].Split(' ');

                    if (firstline.Length != 3) // Bad Request
                        return;

                    RqType = RequestDictionary.ContainsKey(firstline[0])
                        ? RequestDictionary[firstline[0]]
                        : RequestType.Err;
                    Path = firstline[1];
                    if (Path.Last() == '/') {
                        Path += "index.html";
                    }

                    HttpVersion = firstline[2].Replace("HTTP/", "");
                    isHeader = true;
                } else if (isHeader) {
                    // 
                    // ヘッダーの部分
                    // 
                    if (RequestPerLine[i] == @"\r\n") {
                        isHeader = false;
                    } else {
                        Regex headerRegex = new Regex(@"(?<key>[\w-]*?):\s(?<value>.*?)$");
                        var headerMatches = headerRegex.Match(RequestPerLine[i]);
                        Header.Add(headerMatches.Groups["key"].Value, headerMatches.Groups["value"].Value);
                    }
                } else {
                    // 
                    // コンテンツの中身の部分
                    // 
                    sb.AppendLine(RequestPerLine[i]);
                }
            }

            Ingredients = sb.ToString();
        }

        /// <summary>
        /// 指定されたヘッダーの値を取得します
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

        public bool Validate() { return RqType != RequestType.Err; }

        public static bool Validate(string value) {
            Regex rg = new Regex(".*");
            return rg.IsMatch(value);
            // return true;
        }
    }
}