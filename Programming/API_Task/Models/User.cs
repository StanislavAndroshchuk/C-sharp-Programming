using System.ComponentModel.DataAnnotations.Schema;

namespace API_Task.Models;

public enum UserRole { Admin, Manager, Customer }

public class User
{
    public int Id { get; set; }
    [Column(TypeName = "varchar(20)")]
    public UserRole Role { get; set; }
    [Column(TypeName = "varchar(20)")]
    public string FirstName { get; set; } = string.Empty;
    [Column(TypeName = "varchar(20)")]
    public string LastName { get; set; } = string.Empty;
    [Column(TypeName = "varchar(30)")]
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public override string ToString() => $"[{Email}] {FirstName} {LastName}";
    
    public bool IsAdmin()
    {
        return Role == UserRole.Admin;
    }
    public bool IsManager()
    {
        return Role == UserRole.Manager;
    }
    public bool IsCustomer()
    {
        return Role == UserRole.Customer;
    }
}