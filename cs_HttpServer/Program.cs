using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace cs_HttpServer {
    public class User {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class LoginInfo {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    class Program {
        private static int GlobalId = 1;
        private static List<User> Users { get; } = new List<User>();

        static void Main(string[] args) {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:45678/"); // IPEndPoint
            listener.Start(); // Listen/Start

            while (true) {
                // HttpListenerContext
                var context = listener.GetContext(); // Accept + Receive
                // HttpListenerRequest
                var request = context.Request; // index.html, list of friends, playlist for metallica etc.
                // HttpListenerResponse
                var response = context.Response;

                var reader = new StreamReader(request.InputStream);
                var writer = new StreamWriter(response.OutputStream);

                if (request.HttpMethod == "GET") {
                    var json = JsonConvert.SerializeObject(Users.Select(u => u.Login));
                    writer.WriteLine(json);
                    writer.Flush();
                    response.StatusCode = 200;
                    response.ContentType = "application/json";
                } else if (request.HttpMethod == "POST") {
                    var userJson = reader.ReadToEnd();
                    var info = JsonConvert.DeserializeObject<LoginInfo>(userJson);
                    var user = new User {
                        Id = GlobalId++,
                        Login = info.Login,
                        Password = info.Password
                    };
                    Users.Add(user);
                    writer.WriteLine(JsonConvert.SerializeObject(user.Id));
                    response.StatusCode = 201;
                    response.ContentType = "application/json";
                } else if (request.HttpMethod == "DELETE") {

                }

                response.Close();
            }
        }
    }
}
