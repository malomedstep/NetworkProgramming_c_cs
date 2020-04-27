using System;

namespace cs_HttpServer.Models {
    public class User {
        public User(int id, string login, string password, DateTime createdAt) {
            Id = id;
            Login = login;
            Password = password;
            CreatedAt = createdAt;
        }
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
