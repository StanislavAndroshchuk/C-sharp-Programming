using System.ComponentModel.DataAnnotations;

namespace API_Task.Dto;

public class UserBaseDto
{
    [Required(ErrorMessage = "Email is required.")]
    [MaxLength(30, ErrorMessage = "Email should not exceed 30 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Incorrect email!")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{4,20}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit and be between 4 and 20 characters long.")] 
    public string Password { get; set; } = string.Empty;
}

public class UserDto : UserBaseDto 
{
    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(20, ErrorMessage = "First name should not exceed 20 characters.")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "The first name field must contain only letters!")]
    public string FirstName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(20, ErrorMessage = "Last name should not exceed 20 characters.")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "The last name field must contain only letters!")]
    public string LastName { get; set; } = string.Empty;
}


public class UserLoginDto : UserBaseDto {}