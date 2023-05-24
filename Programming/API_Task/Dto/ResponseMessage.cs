using System.ComponentModel.DataAnnotations;

namespace API_Task.Dto;

public class ResponseMessage
{
    public int Status { get; set; }
    public string Message { get; set; }

}
public class ResponseMessageBody
{
    public int Status { get; set; }
    public string Message { get; set; }
    public object Body { get; set; }
    
}