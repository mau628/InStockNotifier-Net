using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InStockNotifier
{
    class Program
    {
        static void Main(string[] args)
        {
            int sleep_time = 60;
            while (true)
            {
                var arr = JArray.Parse(File.ReadAllText(@"config.json"));
                var index = 0;
                foreach (dynamic item in arr)
                {
                    bool enabled;
                    string url = null;
                    string kword = null;

                    index++;
                    try
                    {
                        enabled = item.enabled ?? false;
                        if (!enabled)
                            continue;

                        url = item.url;
                        if (url is null)
                        {
                            Console.WriteLine($"Invalid configuration [url], skipping item #{index}.");
                            continue;
                        }

                        kword = item.keyword;
                        if (kword is null)
                        {
                            Console.WriteLine($"Invalid configuration [keyword], skipping item #{index}.");
                            continue;
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"Invalid configuration, skipping item #{index}.");
                        continue;
                    }

                    bool alert = item.alert ?? true;
                    string name = item.name ?? url;

                    Console.Write($"Checking {name} @ {url} => ");
                    var data = HttpGet(url);

                    if (data.Item1)
                    {
                        if (data.Item2.ToUpper().Contains(kword.ToUpper()))
                        {
                            Console.WriteLine("Success!");
                            if (alert)
                            {
                                OpenBrowser(url);
                                Beeps.GoingUp();
                            }
                        }
                        else
                            Console.WriteLine($"Failed :(");
                    }
                    else
                        Console.WriteLine($"Failed ({data.Item2})");
                }
                Console.WriteLine($"Sleeping {sleep_time} seconds...");
                Thread.Sleep(sleep_time * 1000);
                Console.WriteLine();
            }
        }

        public static (bool, string) HttpGet(string url)
        {
            string data = null;
            bool isSuccess = false;

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add("Content-Encoding", "gzip");
            request.AutomaticDecompression = DecompressionMethods.GZip;

            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream);
                    data = reader.ReadToEnd();
                    reader.Close();
                }
                isSuccess = true;
            }
            else
                data = response.StatusCode.ToString();

            return (isSuccess, data);
        }

        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception ex)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    Console.Write($"Cannot launch browser: {ex.Message} => {ex.StackTrace}");
                }
            }
        }
    }
}
