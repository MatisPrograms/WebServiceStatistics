using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using ServerStats;
using AgeStats;
using CookieStats;

namespace HttpServer
{
    internal class WebApp
    {
        static void Main(string[] args)
        {
            // Http Listener Server on port 8080
            HttpListener listener = new HttpListener();
            const string url = "http://localhost:8080/";
            listener.Prefixes.Add(url);

            const string publicFiles = @"../../../";
            Router router = new Router();

            router.GET("/server", (request, response) =>
            {
                string urlToExecute = HttpUtility.ParseQueryString(request.Url.Query)["url"];
                if (string.IsNullOrEmpty(urlToExecute) ) return ServerTypes.Apply(new string[] { });
                return ServerTypes.Apply(urlToExecute.Split(' '));
            });

            router.GET("/age", (request, response) =>
            {
                string urlToExecute = HttpUtility.ParseQueryString(request.Url.Query)["url"];
                if (string.IsNullOrEmpty(urlToExecute)) return PageAge.Apply(new string[] { });
                return PageAge.Apply(urlToExecute.Split(' '));
            });

            router.GET("/cookie", (request, response) =>
            {
                string urlToExecute = HttpUtility.ParseQueryString(request.Url.Query)["url"];
                if (string.IsNullOrEmpty(urlToExecute)) return CookieCounter.Apply(new string[] { });
                return CookieCounter.Apply(urlToExecute.Split(' '));
            });

            // Trap Ctrl-C and exit 
            Console.CancelKeyPress += delegate
            {
                listener.Stop();
                Environment.Exit(0);
            };

            listener.Start();
            Console.WriteLine("Listening on " + url + "home");

            while (true)
            {
                // Get the context
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                // Get the request
                string requestUrl = request.Url.LocalPath.ToString();
                Console.WriteLine("Request: " + requestUrl);

                string responseData;
                if (requestUrl.Split('/')[1] == "api")
                {
                    responseData = router.apply(request, response);
                }
                else
                {
                    try
                    {
                        // Check if last split has a file extension
                        string[] splits = requestUrl.Split('/');
                        if (!splits[splits.Length - 1].Contains(".")) requestUrl += ".html";
                        responseData = new StreamReader(publicFiles + requestUrl, Encoding.UTF8).ReadToEnd();
                    }
                    catch (Exception ex)
                    {
                        responseData = ex.Message.Replace(Path.GetFullPath(publicFiles), "");
                    }
                }

                // Return data to client
                byte[] buffer = Encoding.UTF8.GetBytes(responseData);
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
        }
    }
}
