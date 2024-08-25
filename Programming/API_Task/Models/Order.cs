using System.ComponentModel.DataAnnotations.Schema;
/*using API_Task.Interfaces;*/

namespace API_Task.Models;

public enum OrderTypes {Paid,NotPaid,Refunded}
public enum StatesEnum {Draft,Moderation,Published}

public class Order : IContext
{
    public int Id { get; set; }
    [Column(TypeName = "varchar(20)")]
    public OrderTypes OrderStatus { get; set; }
    public int Amount { get; set; }
    public int Discount { get; set; } 
    public DateOnly OrderDate  { get; set; }
    public DateOnly ShippedDate   { get; set; }
    [Column(TypeName = "varchar(20)")]
    public StatesEnum ReadedState { get; set; }
    private State _currentState => State.ConvertEnum(ReadedState, this);
       
    public void ChangeState(StatesEnum state)
    {
        ReadedState = state;
    }
    public void Publishing(User user)
    {
        _currentState.Publishing(user);
    }
}


