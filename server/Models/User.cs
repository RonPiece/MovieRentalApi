using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace hw4.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Active { get; set; }
        public DateTime? DeletedAt { get; set; } // The question mark means that the value can be NULL.

        // Empty constructor
        public User() { }

        // Constructor with parameters
        public User(int id, string name, string email, string password, bool active)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Active = active;
        }

        // Email format validation
        public static bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-z0-9!#$%&'+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";
            return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
        }

        // Instance methods
        public int InsertUser()
        {
            DBservices dbs = new DBservices();
            return dbs.InsertUser(this);
        }

        public int UpdateUser()
        {
            DBservices dbs = new DBservices();
            return dbs.UpdateUser(this.Id, this);
        }

        public int DeleteUser()
        {
            DBservices dbs = new DBservices();
            return dbs.DeleteUser(this.Id);
        }

        public int UpdateUserActiveStatus(bool isActive)
        {
            DBservices dbs = new DBservices();
            return dbs.UpdateUserActiveStatus(this.Id, isActive);
        }

        // Static methods
        public static User? Login(string email, string password)
        {
            DBservices dbs = new DBservices();
            return dbs.Login(email, password);
        }

        public static List<User> GetAllUsers()
        {
            DBservices dbs = new DBservices();
            return dbs.GetAllUsers();
        }



        //////-----------------------------------------------------------Methods-----------------------------------------------------------//

        //public List<User> Read()
        //{
        //    return usersList;
        //}

        //public bool Register(string name, string email, string plainPassword)
        //{
        //    foreach (var user in usersList)
        //    {
        //        if (user.Email == email)
        //        {
        //            return false;
        //        }
        //    }

        //    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        //    int newId = usersList.Count > 0 ? usersList.Max(u => u.Id) + 1 : 1;
        //    User newUser = new User(newId, name, email, hashedPassword, true);
        //    usersList.Add(newUser);
        //    return true;
        //}

        //public static User Login(string email, string plainPassword)
        //{
        //    foreach (var user in usersList)
        //    {
        //        if (user.Email == email && BCrypt.Net.BCrypt.Verify(plainPassword, user.Password))
        //        {
        //            return user;
        //        }
        //    }
        //    return null;
        //}
    }
}