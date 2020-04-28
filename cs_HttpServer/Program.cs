using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using cs_HttpServer.Controllers;
using cs_HttpServer.Models;
using Newtonsoft.Json;

namespace cs_HttpServer {

    public static class HttpListenerContextExtensions {
        public static async Task<T> ReadAsync<T>(this HttpListenerRequest request) {
            using (var reader = new StreamReader(request.InputStream)) {
                var str = await reader.ReadToEndAsync();
                var obj = JsonConvert.DeserializeObject<T>(str);
                return obj;
            }
        }

        public static int? GetIntParam(this HttpListenerRequest request, string paramName) {
            var param = request.QueryString[paramName];
            if (param == null)
                return null;
            return int.Parse(param);
        }

        public static string GetStringParam(this HttpListenerRequest request, string paramName) {
            return request.QueryString[paramName];
        }

        public static DateTime? GetDateTimeParam(this HttpListenerRequest request, string paramName) {
            var param = request.QueryString[paramName];
            if (param == null)
                return null;
            return DateTime.Parse(param);
        }

        public static async Task WriteAsync<T>(this HttpListenerResponse response, T data, int statusCode) {
            using (var sw = new StreamWriter(response.OutputStream)) {
                response.StatusCode = statusCode;
                response.ContentType = "application/json";
                var json = JsonConvert.SerializeObject(data);
                await sw.WriteLineAsync(json);
            }
        }
    }

    class Program {
        private static readonly Controller Controller = new Controller();
        private static HttpListener _listener;

        static async Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response) {
            switch (request.HttpMethod) {
                case "GET":
                    var registeredFrom = request.GetDateTimeParam("registeredFrom");
                    var registeredTo = request.GetDateTimeParam("registeredTo");
                    var users = registeredFrom == null ?
                        Controller.GetUsers() :
                        Controller.GetUsers(u => u.CreatedAt >= registeredFrom);

                    await response.WriteAsync(users, 200);
                    break;

                case "POST":
                    var info = await request.ReadAsync<LoginInfo>();
                    var result = Controller.CreateUser(info);
                    if (result.HasErrors) {
                        await response.WriteAsync(result.Errors, 400);
                    } else {
                        await response.WriteAsync(result.User.Id, 201);
                    }
                    break;

                case "DELETE":
                    var login = await request.ReadAsync<string>();
                    response.StatusCode = (Controller.DeleteUser(login) ? 204 : 404);
                    break;
            }
        }

        static async Task StartServer() {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:45678/");
            _listener.Prefixes.Add("http://127.0.0.1:45678/");
            _listener.Start();

            while (true) {
                var context = await _listener.GetContextAsync();

                await HandleRequest(context.Request, context.Response);

                context.Response.Close();
            }
        }

        static void Main(string[] args) {
            StartServer().GetAwaiter().GetResult();
        }
    }
}
