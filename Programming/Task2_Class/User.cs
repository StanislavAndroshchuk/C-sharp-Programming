
using System.Reflection;

namespace Task2_Class
{
    public enum Roles{
        Customer,
        Admin
    }
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Roles Role { get; set; }
        public string Password { get; set; }
        public User(){}
        public User(string firstName, string lastName, string email, Roles role, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Role = role;
            Password = password;
        }
        public bool HasDefaultValues()
        {
            return string.IsNullOrEmpty(FirstName) &&
                   string.IsNullOrEmpty(LastName) &&
                   string.IsNullOrEmpty(Email) &&
                   Role == default(Roles) &&
                   string.IsNullOrEmpty(Password);
        }
        public override string ToString()
        {
            string to_return = "";
            foreach (PropertyInfo x in this.GetType().GetProperties())
            {
                to_return += x.Name + " - " + x.GetValue(this) + "\n"; 
            }
            return to_return;
        }
    }
    
}

