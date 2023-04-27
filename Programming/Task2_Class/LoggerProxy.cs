using System.Text.Json.Nodes;
using System.Text.Json;
namespace Task2_Class;

public class LoggerProxy<T> : IUserAction<T> where T : class
{
    private readonly IUserAction<T> userActions;
    private readonly User user;
    public LoggerProxy(User user, IUserAction<T> userActions)
    {
        this.user = user;
        this.userActions = userActions;
    }

    public void Logger(string action, object value)
    {
        Console.WriteLine($"{action} : {DateTime.Now},{user.FirstName},{user.LastName},{user.Email}, {action}, {value} ");
    }
    public List<T> ViewList()
    {
        var element = userActions.ViewList();
        Logger("View list",element);
        return element;
    }

    public T ViewById(int id)
    {
        var element = userActions.ViewById(id);
        Logger("View list by id",element);
        return element;
    }

    public List<T> Search(string query)
    {
        var element = userActions.Search(query);
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var jsonString = JsonSerializer.Serialize(element,options);
        Logger("Search list",new{query, ToList = jsonString});
        return element;
    }

    public List<T> Sort(string sortBy)
    {
        var element = userActions.Sort(sortBy);
        Logger("Sort list",new{sortBy,element});
        return element;
    }

    public T Create(T student)
    {
        var element = userActions.Create(student);
        Logger("Sort list",element);
        return element;
    }

    public T Edit(T student)
    {
        var element = userActions.Edit(student);
        Logger("Sort list",element);
        return element;
    }

    public void Delete(int id)
    {
        userActions.Delete(id);
        Logger("Delete by id",id);
        return;
    }
}