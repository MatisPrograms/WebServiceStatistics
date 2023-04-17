using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using MoreLinq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ServerStats
{
    public class ServerTypes
    {
        private static readonly HttpClient http = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(1)
        };

        static void Main(string[] args)
        {
            Console.WriteLine(ServerTypes.Apply(new string[] { }));
        }

        static public string Apply(string[] urls)
        {
            string dataPath = @"../../../";
            string dataFile = "websites.json";
            string statFile = "servers.json";
            List<Website> websites = new List<Website>();

            if (urls.Length == 0)
            {
                websites = JsonConvert.DeserializeObject<List<Website>>(File.ReadAllText(dataPath + (File.Exists(dataPath + statFile) ? statFile : dataFile)));

                websites.ForEach(website => website.Url = website.Url.Contains("https://") || website.Url.Contains("http://") ? website.Url : "https://" + website.Url);
            }
            else
            {
                foreach (string url in urls)
                {
                    Website website = new Website();
                    website.Url = url.Contains("https://") || url.Contains("http://") ? url : "https://" + url;
                    websites.Add(website);
                }
            }

            try
            {
                foreach (Website website in websites)
                {
                    try
                    {
                        if (website.Server != null) continue;
                        Console.WriteLine("Requesting " + website.Url + "'s server");
                        website.Server = http.GetAsync(website.Url.ToLower()).Result.Headers.Server.FirstOrDefault().ToString();

                        if (string.IsNullOrEmpty(website.Server))
                        {
                            var req = new HttpRequestMessage(HttpMethod.Head, website.Url.ToLower());
                            Task<HttpResponseMessage> res = new HttpClient(new HttpClientHandler()).SendAsync(req);
                            res.Wait();

                            website.Server = res.Result.Headers.Server.FirstOrDefault().ToString();
                        }

                        if (string.IsNullOrEmpty(website.Server)) throw new Exception("Server was empty");
                    }
                    catch (Exception)
                    {
                        website.Server = "N/A";
                    }
                }
            }
            catch (Exception)
            {
            }

            if (File.Exists(dataPath + statFile))
            {
                websites.AddRange(JsonConvert.DeserializeObject<List<Website>>(File.ReadAllText(dataPath + statFile)));
                websites = websites.DistinctBy(w => w.Url).ToList();
            }
            File.WriteAllText(dataPath + statFile, JsonConvert.SerializeObject(websites, Formatting.Indented));

            return JsonConvert.SerializeObject(websites, Formatting.Indented);
        }

        private class Website
        {
            public string Url { get; set; }
            public string Server { get; set; }
        }
    }
}
