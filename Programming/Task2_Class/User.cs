
namespace Task2_Class
{
    public enum Roles{
        Customer,
        Admin
    }
    public class User
    {
        public string FirstName;
        public string LastName;
        public string Email;
        public Roles Role;
        public string Password;

        public User(string firstName, string lastName, string email, Roles role, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Role = role;
            Password = password;
        }
    }
    
}

