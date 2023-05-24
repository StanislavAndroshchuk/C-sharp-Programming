using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
//using API_Task.Interfaces;
using API_Task.Models;
using API_Task.Dto;
using System.Security.Cryptography;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using API_Task.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace API_Task.Controllers;

[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;

    public OrderController(DatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    //Get orders, search
    /*[HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseMessage))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseMessageBody))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetOrders([FromQuery]string? toSearchQuery)
    {
        List<Order> orders;

        if (string.IsNullOrEmpty(toSearchQuery))
        {
            orders = _context.Orders.ToList();
        }
        else
        {
            orders = _context.Orders.AsEnumerable().Where(o =>
                o.Id.ToString().Contains(toSearchQuery) ||
                o.OrderStatus.ToString().Contains(toSearchQuery) ||
                o.Amount.ToString().Contains(toSearchQuery) ||
                o.Discount.ToString().Contains(toSearchQuery) ||
                o.OrderDate.ToString().Contains(toSearchQuery) ||
                o.ShippedDate.ToString().Contains(toSearchQuery) ||
                o.ReadedState.ToString().Contains(toSearchQuery)).ToList();
        }

        if (orders.Count == 0)
        {
            ResponseMessage notfoundResponse = new ResponseMessage()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "No orders found"
            };
            return NotFound(notfoundResponse);
        }

        ResponseMessageBody foundResponse = new ResponseMessageBody()
        {
            Status = StatusCodes.Status200OK,
            Message = "Orders found",
            Body = orders
        };
        return Ok(foundResponse);
    }*/
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseMessageBody))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseMessage))]
    [Authorize(Roles = "Admin,Manager,Customer")]
    public IActionResult GetOrders([FromQuery] string? toSearch, [FromQuery] string? toSortBy)
    {
        List<Order> orders;
        var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var userMail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        User user = FindByEmail(userMail);
        
        if (!string.IsNullOrEmpty(toSearch))
        {
            orders = _context.Orders.AsEnumerable().Where(o => 
                o.Id.ToString().Contains(toSearch) ||
                o.OrderStatus.ToString().Contains(toSearch) ||
                o.Amount.ToString().Contains(toSearch) ||
                o.Discount.ToString().Contains(toSearch) ||
                o.OrderDate.ToString().Contains(toSearch) ||
                o.ShippedDate.ToString().Contains(toSearch)).ToList();

            if(userRole == "Customer")
                orders = orders.Where(o => o.ReadedState == StatesEnum.Published).ToList();
        }
        else
        {
            orders = userRole == "Customer" ? _context.Orders.Where(o => o.ReadedState == StatesEnum.Published).ToList() : _context.Orders.ToList();
        }

        if (orders.Count == 0)
        {
            ResponseMessage notfoundResponse = new ResponseMessage()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "No orders found"
            };
            return NotFound(notfoundResponse);
        }

        if (!string.IsNullOrEmpty(toSortBy))
        {
            var propertyInfo = typeof(Order).GetProperty(toSortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                orders = orders.OrderBy(o => propertyInfo.GetValue(o, null)).ToList();
            }
            else
            {
                ResponseMessage badResponse = new ResponseMessage()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Property is not found"
                };
                return BadRequest(badResponse);
            }
        }

        ResponseMessageBody foundResponse = new ResponseMessageBody()
        {
            Status = StatusCodes.Status200OK,
            Message = $"{orders.Count} order(s) found",
            Body = orders
        };
        var toLogger = new
        {
            List = orders,
            SearchQuery = toSearch,
            SortQuery = toSortBy
        };
        Logger.LoggerJson("Get list",toLogger,user);
        return Ok(foundResponse);
    }



    //Get order by id
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseMessage))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseMessageBody))]
    [Authorize(Roles = "Admin,Manager,Customer")]
    public IActionResult GetOrder(int id)
    {
        Order order = _context.Orders.Find(id);
        var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var userEmail =  User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        User user = FindByEmail(userEmail);
        if (order == null || userRole == "Customer" && order.ReadedState != StatesEnum.Published)
        {
            ResponseMessage notfoundResponse = new ResponseMessage()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "No orders by this id"
            };
            return NotFound(notfoundResponse);
        }

        ResponseMessageBody foundResponse = new ResponseMessageBody()
        {
            Status = StatusCodes.Status200OK,
            Message = $"Order has been found by {id}",
            Body = order
        };
        Logger.LoggerJson("Get order by id",order,user);
        return Ok(foundResponse);
    }
    

    //Create order 
    [HttpPost("Create")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseMessageBody))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Order> Post(DtoOrderPostPut orderPost)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        Order order = CreateOrder(orderPost);
        var userEmail =  User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        User user = FindByEmail(userEmail);
        Logger.LoggerJson("Create new order",order,user);
        return Ok(new ResponseMessageBody()
        {
            Status = StatusCodes.Status201Created,
            Message = "Order created successfully",
            Body = order
        });
    }
    
    [HttpPut("{id}")] 
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseMessageBody))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseMessage))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult UpdateOrder(int id, [FromBody] DtoUpdateOrder orderUpdate)
    {
        if (!ModelState.IsValid) 
        {
            return BadRequest(ModelState);
        }
    
        Order order = _context.Orders.Find(id);
        if (order == null)
        {
            ResponseMessage notfoundResponse = new ResponseMessage()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "No orders found with this id"
            };
            return NotFound(notfoundResponse);
        }

        if (orderUpdate.OrderStatus != null)
        {
            order.OrderStatus = (OrderTypes)orderUpdate.OrderStatus;
        }

        if (orderUpdate.Amount != null)
        {
            order.Amount = (int)orderUpdate.Amount;
        }

        if (orderUpdate.Discount != null)
        {
            order.Discount = (int)orderUpdate.Discount;
        }

        if (orderUpdate.OrderDate != null)
        {
            order.OrderDate = (DateOnly)orderUpdate.OrderDate;
        }
        if (orderUpdate.ShippedDate != null)
        {
            order.ShippedDate = (DateOnly)orderUpdate.ShippedDate;
        }

        order.ReadedState = StatesEnum.Draft;

        _context.Orders.Update(order);
        _context.SaveChanges();

        ResponseMessageBody updateResponse = new ResponseMessageBody()
        {
            Status = StatusCodes.Status200OK,
            Message = "Order updated successfully",
            Body = order
        };
        var userEmail =  User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        User user = FindByEmail(userEmail);
        var ToLogger = new
        {
            Id = id,
            Order = order
        };
        Logger.LoggerJson("Edit order by id",ToLogger,user);
        return Ok(updateResponse);
    }
    
    //Delete order by id
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResponseMessage))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseMessageBody))]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteOrder(int id)
    {
        Order order = _context.Orders.Find(id);
        if (order == null)
        {
            ResponseMessage notfoundResponse = new ResponseMessage()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "No orders found with this id"
            };
            return NotFound(notfoundResponse);
        }

        _context.Orders.Remove(order);
        _context.SaveChanges();

        ResponseMessageBody deleteResponse = new ResponseMessageBody()
        {
            Status = StatusCodes.Status200OK,
            Message = "Order deleted successfully",
            Body = order
        };
        var userEmail =  User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        User user = FindByEmail(userEmail);
        var ToLogger = new
        {
            Id = id,
            Order = order
        };
        Logger.LoggerJson("Delete order by id",ToLogger,user);
        return Ok(deleteResponse);
    }

    [HttpPut("/publish/{id}")]
    [Authorize(Roles = "Admin, Manager")]
    public IActionResult Publish(int id)
    {
        Order order = _context.Orders.Find(id);
        if (order == null)
        {
            ResponseMessage notfoundResponse = new ResponseMessage()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "No orders by this id"
            };
            return NotFound(notfoundResponse);
        }
        
        var CurrentUser = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
        var userId = int.Parse(CurrentUser.Value);
        var user = _context.Users.Find(userId);
        if (user == null)
        {
            ResponseMessage notfoundResponse = new ResponseMessage()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "User not found"
            };
            return NotFound(notfoundResponse);
        }
        var currState = State.ConvertEnum(order.ReadedState, order);
        currState.Publishing(user);
        _context.Entry(order).State = EntityState.Modified;
        _context.SaveChanges();
        var ToLogger = new
        {
            Id = id,
            Order = order
        };
        Logger.LoggerJson("Publish order by id",ToLogger,user);
        return NoContent();
    }


    private User? FindByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email);
    }

    private Order CreateOrder(DtoOrderPostPut orderDto)
    {
        Order order = new Order
        {
            OrderStatus = orderDto.OrderStatus,
            Amount = orderDto.Amount,
            Discount = orderDto.Discount,
            
            OrderDate = orderDto.OrderDate,
            ShippedDate = orderDto.ShippedDate,
            ReadedState = StatesEnum.Draft
        };

        _context.Add(order);
        _context.SaveChanges();
        return order;
    }


    /*
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
    }*/

    private bool IsDuplicateEmail(string email)
    {
        return FindByEmail(email) == null;
    }

    
   
}
