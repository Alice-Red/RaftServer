using NUnit.Framework;
using RaftServer;

namespace TestRaft
{
    public class Tests
    {
        [SetUp]
        public void Setup() { }

        /// <summary>
        /// HttpRequestObject
        /// </summary>
        [Test]
        public void TestRequestObject() {
            HttpRequestObject hreq = new HttpRequestObject(
                @"GET /resource/c2.png HTTP/1.1\r\n" +
                @"Host: 127.0.0.1:12345\r\n" +
                @"Connection: keep-alive\r\n" +
                @"User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36\r\n" +
                @"Accept: image/webp,image/apng,image/*,*/*;q=0.8\r\n" +
                @"Sec-Fetch-Site: same-origin\r\n" +
                @"Sec-Fetch-Mode: no-cors\r\n" +
                @"Sec-Fetch-Dest: image\r\n" +
                @"Referer: http://127.0.0.1:12345/\r\n" +
                @"Accept-Encoding: gzip, deflate, br\r\n" +
                @"Accept-Language: ja,en-US;q=0.9,en;q=0.8\r\n" +
                @"Cookie: backendVersion=1.1.0.2686; localauth=localapia1c8ea6d5fcdfaef:; isNotIncognito=true; _ga=GA1.1.1850969703.1593050777\r\n"
            );

            Assert.Equals(hreq.RqType, RequestType.Get);
            Assert.Equals(hreq.Path, "/resource/c2.png");
            Assert.Equals(hreq.HttpVersion, "1.1");

            Assert.True(hreq.Header.ContainsKey("Host"));
            Assert.True(hreq.Header.ContainsKey("User-Agent"));
            Assert.True(hreq.Header.ContainsKey("Cookie"));

            Assert.Equals(hreq.Header["Connection"], "keep-alive");
            Assert.Equals(hreq.Header["Accept"], "image/webp,image/apng,image/*,*/*;q=0.8");
            Assert.Equals(hreq.Header["Sec-Fetch-Site"], "same-origin");

            HttpRequestObject hreq2 = new HttpRequestObject(
                @"GET /containts/new/ HTTP/1.1\r\n" +
                @"Connection: keep-alive\r\n"
            );

            Assert.Equals(hreq2.RqType, RequestType.Get);
            Assert.Equals(hreq2.Path, "/containts/new/index.html");
            Assert.Equals(hreq2.HttpVersion, "1.1");


        }
    }
}