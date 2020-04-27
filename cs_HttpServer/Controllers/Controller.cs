using System;
using System.Collections.Generic;
using System.Linq;
using cs_HttpServer.Models;

namespace cs_HttpServer.Controllers {
    public class CreateUserResult {
        public User User { get; }
        public bool HasErrors => _errors.Count > 0;
        private List<string> _errors = new List<string>();
        public IReadOnlyCollection<string> Errors => _errors.AsReadOnly();
        public CreateUserResult(string error) {
            _errors.Add(error);
        }
        public CreateUserResult(User user) {
            User = user;
        }
    }

    class Controller {
        private static int GlobalId = 8;

        private static List<User> Users { get; } = new List<User>();

        public Controller() {
            Users.Add(new User(1, "John", "qwerty123", DateTime.Now.AddDays(-10)));
            Users.Add(new User(2, "Jack", "qwerty456", DateTime.Now.AddDays(-11)));
            Users.Add(new User(3, "Jason", "qwerty789", DateTime.Now.AddDays(-5)));
            Users.Add(new User(4, "Jeremy", "qwerty1263", DateTime.Now));
            Users.Add(new User(5, "Jeremiah", "qwerty1234", DateTime.Now.AddDays(-3)));
            Users.Add(new User(6, "Jerome", "qwerty45324", DateTime.Now.AddDays(-100)));
            Users.Add(new User(7, "Jane", "qwerty576456", DateTime.Now.AddDays(-42)));
        }

        public CreateUserResult CreateUser(LoginInfo info) {
            if (Users.FirstOrDefault(u => u.Login.ToUpper() == info.Login.ToUpper()) != null) {
                return new CreateUserResult($"User with login {info.Login} already exists");
            }

            var user = new User(GlobalId++, info.Login, info.Password, DateTime.Now);
            Users.Add(user);
            return new CreateUserResult(user);
        }

        public bool DeleteUser(string login) {
            var u = Users.FirstOrDefault(user => user.Login == login);
            if (u != null) {
                Users.Remove(u);
                return true;
            } else {
                return false;
            }
        }

        // TODO: implement filter
        public IEnumerable<User> GetUsers(Func<User, bool> predicate = null) {
            IEnumerable<User> query = Users;
            if (predicate != null) {
                query = query.Where(predicate);
            }
            return query;
        }

        public IEnumerable<User> FilterUsers(DateTime? from = null, DateTime? to = null, string login = null) {
            IEnumerable<User> query = Users;
            if (from != null) {
                query = query.Where(u => u.CreatedAt > from);
            }

            if (to != null) {
                query = query.Where(u => u.CreatedAt < to);
            }

            if (login != null) {
                query = query.Where(u => u.Login == login);
            }

            return query;
        }

    }
}
