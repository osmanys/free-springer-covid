using System;
using System.IO;
using System.Net;

namespace SpringerDownload
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Downloading!");
            var root = "Springer";
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            foreach (var line in File.ReadAllLines($"books.txt"))
            {
                var data = line.Split(";");

                if (!Directory.Exists($"{root}/{data[0]}"))
                {
                    Directory.CreateDirectory($"{root}/{data[0]}");
                }

                var url = GetUrl(data[1]).Replace("book", "content/pdf");
                Console.WriteLine($"Downloading {url}");

                bool hadError;
                int attemp = 0;
                do
                {
                    attemp++;
                    try
                    {
                        hadError = false;
                        var request = WebRequest.Create(url);
                        var response = request.GetResponse();
                        var name = response.Headers["Content-Disposition"].Split("=")[1];
                        using (Stream output = File.OpenWrite($"{root}/{data[0]}/{name}"))
                        {
                            using (var st = response.GetResponseStream())
                            {
                                st.CopyTo(output);
                            }
                        }
                    }
                    catch
                    {
                        hadError = true;
                    }
                }
                while (hadError && attemp < 5);
                if (attemp == 5)
                    Console.Error.WriteLine("Book not available");
            }

            Console.WriteLine("Succesfully!");
            Console.ReadKey();
        }

        static string GetUrl(string url)
        {
            string uriString = "";

            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.AllowAutoRedirect = true;
            webRequest.Timeout = 10000;
            webRequest.Method = "HEAD";

            using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                uriString = webResponse.ResponseUri.OriginalString;
                webResponse.Close();
            }

            return uriString;
        }
    }
}
