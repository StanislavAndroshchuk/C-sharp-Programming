using System.ComponentModel.DataAnnotations;
using API_Task.Models;

namespace API_Task.Dto;

public class OrderBaseDto
{
    [Required(ErrorMessage = "Order status is required")]
    public OrderTypes OrderStatus { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0,int.MaxValue, ErrorMessage = "Amount has to be positive")]
    public int Amount { get; set; }
    
    [Required(ErrorMessage = "Discount is required")]
    [Range(0,100, ErrorMessage = "Discount has to be between 0-100%")]
    public int Discount { get; set; }

    [Required(ErrorMessage = "Order Date is required")]
    public DateOnly OrderDate { get; set; }
    
    [Required(ErrorMessage = "Shipped Date is required")]
    [DateGreaterThan("OrderDate")]
    public DateOnly ShippedDate { get; set; }
}

public class OrderDto : OrderBaseDto
{
    [Required(ErrorMessage = "State is required")]
    public StatesEnum ReadedState{ get; set; }
}
public class DtoOrderPostPut : OrderBaseDto{}

public class DtoUpdateOrder
{
    public OrderTypes? OrderStatus { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Amount has to be positive")]
    public int? Amount { get; set; }

    [Range(0, 100, ErrorMessage = "Discount has to be between 0-100%")]
    public int? Discount { get; set; }
    
    public DateOnly? OrderDate { get; set; }
    
    [DateGreaterThan("OrderDate")]
    public DateOnly? ShippedDate { get; set; }
}


