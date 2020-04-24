using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace cs_HttpClient {
    public class Post {
        public int id { get; set; }
        public int userId { get; set; }
        public string title { get; set; }
        public string body { get; set; }
    }


    public class LoginInfo {
        public LoginInfo(string login, string password) {
            Login = login;
            Password = password;
        }
        public string Login { get; }
        public string Password { get; }
    }

    class Program {
        static void Main(string[] args) {
            var client = new HttpClient();
            HttpResponseMessage response = null;
            string json = null;
            StringContent content = null;
            while (true) {
                Console.Clear();

                Console.WriteLine("1. Get all users\n2. Create new user");
                var input = Console.ReadLine();

                if (input == "1") {
                    response = client.GetAsync("http://localhost:45678").Result;
                    json = response.Content.ReadAsStringAsync().Result;
                    var usernames = JSON.Deserialize<string[]>(json);
                    Console.WriteLine("All users:");
                    foreach (var user in usernames) {
                        Console.WriteLine($"    {user}");
                    }
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadLine();
                } else if (input == "2") {
                    Console.WriteLine("Enter login and password:");
                    var login = Console.ReadLine();
                    var password = Console.ReadLine();

                    var info = new LoginInfo(login, password);

                    // CREATE   POST
                    // READ     GET
                    // UPDATE   PUT/PATCH
                    // DELETE   DELETE

                    json = JSON.Serialize(info);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = client.PostAsync("http://localhost:45678", content).Result;
                }
            }













            // HttpWebRequest, HttpWebResponse
            //var client = new HttpClient();
            //var message = new HttpRequestMessage {
            //    Method = HttpMethod.Get,
            //    RequestUri = new Uri("https://jsonplaceholder.typicode.com/posts?userId=1")
            //};
            // HttpResponseMessage
            //var response = client.SendAsync(message).Result;

            //var response = client.GetAsync("https://jsonplaceholder.typicode.com/posts?userId=1").Result;
            //if (response.StatusCode == HttpStatusCode.OK) {
            //    var data = response.Content.ReadAsStringAsync().Result;
            //    var posts = JSON.Deserialize<Post[]>(data);
            //    foreach (var post in posts) {
            //        Console.WriteLine($"{post.title}");
            //    }
            //}
            //else {
            //    Console.WriteLine("Error?");
            //}
        }
    }
}
