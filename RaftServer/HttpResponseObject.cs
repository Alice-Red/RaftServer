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
        /// �R���X�g���N�^�P
        /// </summary>
        /// <param name="httpVersion">HTTP�o�[�W����</param>
        public HttpResponseObject(string httpVersion) {
            HttpVersion = httpVersion;
            ResponseCode = 418;
            Header = new Dictionary<string, string>();
            ingredients = new byte[0];
        }

        /// <summary>
        /// �R���X�g���N�^�Q
        /// </summary>
        /// <param name="httpVersion">HTTP�o�[�W����</param>
        /// <param name="code">���X�|���X�R�[�h</param>
        public HttpResponseObject(string httpVersion, int code) {
            HttpVersion = httpVersion;
            ResponseCode = code;
            Header = new Dictionary<string, string>();
            ingredients = new byte[0];
        }

        /// <summary>
        /// �w�b�_�[��o�^���܂�
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
        /// �w�肵���w�b�_�[���폜���܂�
        /// </summary>
        /// <param name="key"></param>
        public void DeleteHeader(string key) {
            if (Header.ContainsKey(key)) {
                Header.Remove(key);
            }
        }

        /// <summary>
        /// �w�肵���w�b�_�[�̒l���擾���܂�
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
        /// �f�B���N�g�����w�肳�ꂽ�ꍇ��index�̃t�@�C�����w�肵�܂�
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
        /// ���X�|���X�p�̃f�[�^���o�C�g�z��ɕϊ����܂�
        /// </summary>
        /// <returns>byte[] �ɕϊ����ꂽ���X�|���X�f�[�^</returns>
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