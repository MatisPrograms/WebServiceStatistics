using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MoreLinq;
using System.Net;
using System.Xml.Linq;

namespace CookieStats
{
    public class CookieCounter
    {
        private static readonly HttpClient http = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(1)
        };

        static void Main(string[] args)
        {
            Console.WriteLine(CookieCounter.Apply(new string[] { }));
        }

        static public string Apply(string[] urls)
        {
            string dataPath = @"../../../";
            string dataFile = "websites.json";
            string statFile = "cookies.json";
            string sitemap = "sitemap.xml";
            List<Website> websites = new List<Website>();

            if (urls.Length == 0)
            {
                websites = JsonConvert.DeserializeObject<List<Website>>(File.ReadAllText(dataPath + (File.Exists(dataPath + statFile) ? statFile : dataFile)));

                websites.ForEach(website => website.Url = website.Url.Contains("https://") || website.Url.Contains("http://") ? website.Url : "https://" + website.Url);

                if (!File.Exists(dataPath + statFile))
                {
                    websites.ForEach(website => website.CountCookie = -1);
                }
            }
            else
            {
                foreach (string url in urls)
                {
                    Website website = new Website();
                    website.Url = url.Contains("https://") || url.Contains("http://") ? url : "https://" + url;
                    website.CountCookie = -1;
                    websites.Add(website);
                }
            }

            foreach (Website website in websites)
            {
                try
                {
                    if (website.CountCookie >= 0) continue;
                    Console.WriteLine("Requesting " + website.Url + "'s sub urls cookies");
                    List<string> subUrls = new List<string> { website.Url };
                    List<double> cookies = new List<double> { };

                    var request = http.GetAsync(website.Url + "/" + sitemap);
                    request.Wait();

                    if (request.Result.StatusCode == HttpStatusCode.OK)
                    {
                        var content = request.Result.Content.ReadAsStringAsync();
                        content.Wait();

                        XDocument doc = XDocument.Parse(content.Result);
                        string xmlns = doc.Descendants().FirstOrDefault().Attribute("xmlns").Value;

                        subUrls.AddRange(doc.Descendants("{" + xmlns + "}loc").Select(x => x.Value));
                    }

                    if (subUrls.Count == 1) subUrls.RemoveAt(0);
                    int exceptions = 0;
                    int count = 0;
                    foreach (string subUrl in subUrls)
                    {
                        try
                        {
                            if (++count > 100) break;
                            string url = subUrl.Replace("/" + sitemap, "").Replace("/" + sitemap.Replace(".xml", "") + "s.xml", "");
                            Console.WriteLine(" - " + url);

                            var req = new HttpRequestMessage(HttpMethod.Head, url);
                            Task<HttpResponseMessage> res = new HttpClient(new HttpClientHandler()).SendAsync(req);
                            res.Wait();

                            cookies.Add(res.Result.Headers.GetValues("Set-Cookie").Count());
                        }
                        catch (Exception)
                        {
                            if (++exceptions > 50) break;
                        }
                    }

                    website.CountCookie = cookies.Count;
                    if (!cookies.Any()) continue;
                    website.AverageCookie = cookies.Average();
                    website.DeviationCookie = Math.Sqrt(cookies.Select(x => Math.Pow(x - website.AverageCookie, 2)).Sum() / cookies.Count);

                    Console.WriteLine(JsonConvert.SerializeObject(website, Formatting.Indented));
                }
                catch (Exception)
                {
                }
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
            public int CountCookie { get; set; }
            public double AverageCookie { get; set; }
            public double DeviationCookie { get; set; }
        }
    }
}
