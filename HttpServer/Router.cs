using System;
using System.Collections.Generic;
using System.Net;

namespace HttpServer
{
    internal class Router
    {
        private List<Route> routes = new List<Route>();

        internal string apply(HttpListenerRequest request, HttpListenerResponse response)
        {
            string requestUrl = request.Url.LocalPath.ToString().Replace("/api", "");
            string method = request.HttpMethod.ToUpper();
            if (method != "GET" && method != "POST" && method != "PUT" && method != "DELETE")
            {
                response.StatusCode = 405;
                return "Method not allowed";
            }

            foreach (Route route in this.routes)
            {
                if (route.method == method && route.path == requestUrl) return route.callback(request, response);
            }

            return "Not found";
        }

        internal void route(string method, string path, Func<HttpListenerRequest, HttpListenerResponse, string> callback)
        {
            this.routes.Add(new Route(method, path, callback));
        }

        internal void GET(string path, Func<HttpListenerRequest, HttpListenerResponse, string> callback)
        {
            this.route("GET", path, callback);
        }

        internal void POST(string path, Func<HttpListenerRequest, HttpListenerResponse, string> callback)
        {
            this.route("POST", path, callback);
        }

        internal void PUT(string path, Func<HttpListenerRequest, HttpListenerResponse, string> callback)
        {
            this.route("PUT", path, callback);
        }
        internal void DELETE(string path, Func<HttpListenerRequest, HttpListenerResponse, string> callback)
        {
            this.route("DELETE", path, callback);
        }

        private class Route
        {
            public string method;
            public string path;
            public Func<HttpListenerRequest, HttpListenerResponse, string> callback;

            public Route(string method, string path, Func<HttpListenerRequest, HttpListenerResponse, string> callback)
            {
                this.method = method;
                this.path = path;
                this.callback = callback;
            }
        }
    }
}
