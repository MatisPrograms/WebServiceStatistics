using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using Newtonsoft.Json;
using MoreLinq;
using System.Net;
using System.Threading.Tasks;

namespace AgeStats
{
    public class PageAge
    {
        private static readonly HttpClient http = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(1)
        };

        static void Main(string[] args)
        {
            Console.WriteLine(PageAge.Apply(new string[] { }));
        }

        static public string Apply(string[] urls)
        {
            string dataPath = @"../../../";
            string dataFile = "websites.json";
            string statFile = "ages.json";
            string sitemap = "sitemap.xml";
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

            foreach (Website website in websites)
            {
                try
                {
                    if (website.CountAge >= 0) continue;
                    Console.WriteLine("Requesting " + website.Url + "'s sub urls age");
                    List<string> subUrls = new List<string> { website.Url };
                    List<double> ages = new List<double> { };

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

                            if (!res.Result.Content.Headers.LastModified.HasValue) throw new Exception("Has no Last-Modified header");
                            TimeSpan timeSinceLastModified = DateTime.Now - res.Result.Content.Headers.LastModified.Value;
                            ages.Add(timeSinceLastModified.TotalSeconds);
                        }
                        catch (Exception)
                        {
                            if (++exceptions > 50) break;
                        }
                    }

                    website.CountAge = ages.Count;
                    if (!ages.Any()) continue;
                    website.AverageAge = ages.Average();
                    website.DeviationAge = Math.Sqrt(ages.Select(x => Math.Pow(x - website.AverageAge, 2)).Sum() / ages.Count);
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
            public int CountAge { get; set; }
            public double AverageAge { get; set; }
            public double DeviationAge { get; set; }
        }
    }
}
