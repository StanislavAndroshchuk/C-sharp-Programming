
using System.Reflection;

namespace Task2_Class
{
    public enum Roles{
        Customer,
        Admin,
        Manager
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

        public bool IsAdmin()
        {
            return Role == Roles.Admin;
        }
        public bool IsManager()
        {
            return Role == Roles.Manager;
        }
        public bool IsCustomer()
        {
            return Role == Roles.Customer;
        }
        public override string ToString()
        {
            string toReturn = "";
            foreach (PropertyInfo x in this.GetType().GetProperties())
            {
                toReturn += x.Name + " - " + x.GetValue(this) + "\n"; 
            }
            return toReturn;
        }
    }
    
}

