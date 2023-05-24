using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
//using API_Task.Interfaces;
using API_Task.Models;
using API_Task.Dto;
using System.Security.Cryptography;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using API_Task.Database;


namespace API_Task.Controllers;

[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(DatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    
    [HttpPost("Register")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseMessage))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Register([FromBody] UserDto userCreate)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!IsDuplicateEmail(userCreate.Email))
        {
            ResponseMessage badRequestDetails = new ResponseMessage()
            {
                Status = StatusCodes.Status400BadRequest,
                Message = "Email is already used."
            };
            return BadRequest(badRequestDetails);
        }
        CreateUser(userCreate);// if email exist = bad request

        ResponseMessage responseDetails = new ResponseMessage()
        {
            Status = StatusCodes.Status201Created,
            Message = "User has been successfully created."
        };

        return StatusCode(StatusCodes.Status201Created, responseDetails);
    }
    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseMessage))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Login(UserLoginDto userLogin)
    {
        User? user = FindByEmail(userLogin.Email);

        if (user == null || !CheckPassword(user,userLogin.Password)) 
        {
            var details = new ResponseMessage()
            {
                Status = StatusCodes.Status400BadRequest,
                Message = "Invalid email or password."
            };
            return BadRequest(details);
        }
        
        DateTime expiration = DateTime.Now.AddHours(2);
        string jwtToken = CreateJwtToken(user, expiration);
        
        var response = new
        {
            token = jwtToken,
            message = "User has been successfully authorized."

        };
        return Ok(response);
    }
    
    private User? FindByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email);
    }

    private void CreateUser(UserDto userCreate)
    {
        User user = new User
        {
            Role = UserRole.Customer,
            FirstName = userCreate.FirstName,
            LastName = userCreate.LastName,
            Email = userCreate.Email,
            Password = HashPassword(userCreate.Password)
        };

        _context.Add(user);
        _context.SaveChanges();
    }

    private User? GetByJwtToken(string jwtToken)
    {
        string email = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken).Claims
            .First(c => c.Type == ClaimTypes.Email).Value;
        
        return FindByEmail(email);
    }

    private string CreateJwtToken(User user, DateTime expiration)
    {
        Claim[] claims = new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            signingCredentials: credentials,
            expires: expiration
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }

    private bool CheckPassword(User user, string password)
    {
        return user.Password == HashPassword(password);
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256Hash.ComputeHash(bytes);

            string hashPassword = "";

            for (int i = 0; i < hashBytes.Length; ++i)
            {
                hashPassword += hashBytes[i].ToString("x2");
            }
            
            return hashPassword;
        }
    }

    private bool IsDuplicateEmail(string email)
    {
        return FindByEmail(email) == null;
    }

    
   
}
