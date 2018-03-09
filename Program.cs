using System;
using System.IO;
using System.Net;
using System.Text;

namespace Hackathon {

    class Program {
        private static string api_basic_auth = "Basic XXXX";
        private static string api_project_id = "YYYY";

        private static string api_base_url = "https://api.disruptive-technologies.com/v2beta1/projects/";
        private static string api_stream_devices = "/devices:stream?";
        private static string url = Program.api_base_url + Program.api_project_id + Program.api_stream_devices;

        static void Main(string[] args) {
            Console.WriteLine("Attempting to listen for SSE from " + Program.url);
            var response = Program.OpenSSEStream(Program.url);
            Console.WriteLine("Success! \n");
        }

        public static Stream OpenSSEStream(string url) {
            var headers = new WebHeaderCollection();
            headers.Add("Authorization", Program.api_basic_auth);
            var request = WebRequest.Create(new Uri(url));
            request.Headers = headers;
            ((HttpWebRequest)request).AllowReadStreamBuffering = false;
            var response = request.GetResponse();
            var stream = response.GetResponseStream();
            Program.ReadStreamForever(stream);
            return stream;
        }

        public static void ReadStreamForever(Stream stream) {
            var encoder = new UTF8Encoding();
            var buffer = new byte[2048];
            while (true) {
                if (stream.CanRead) {
                    int len = stream.Read(buffer, 0, 2048);
                    if (len > 0) {
                        var text = encoder.GetString(buffer, 0, len);
                        Program.ProcessJson(text);
                    }
                }
            }
        }

        public static void ProcessJson(string text) {
            if (String.IsNullOrWhiteSpace(text)) return;
            Console.WriteLine("Incoming Data:");
            Console.WriteLine(text);
        }

    }
}
