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

    public void LoggerTxt(string action, object value)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var logEntry = $"{action} : {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, {user.FirstName}, {user.LastName}, {user.Email} : {JsonSerializer.Serialize(value, options)}";

        // Запис логу до файлу logs.txt
        using (StreamWriter sw = File.AppendText("../../../logs.txt"))
        {
            sw.WriteLine(logEntry);
        }
    }
    public void Logger(string action, object value)
    {
        var logEntry = new
        {
            action,
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            userFirstName = user.FirstName,
            userLastName = user.LastName,
            userEmail = user.Email,
            value
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var logEntryJson = JsonSerializer.Serialize(logEntry, options);
        string logsJsonContent;
        string file = "../../../logs.json";
        JsonArray logs;

        if (File.Exists(file))
        {
            logsJsonContent = File.ReadAllText(file);
            try
            {
                logs = JsonSerializer.Deserialize<JsonArray>(logsJsonContent, options) ?? new JsonArray();
            }
            catch (JsonException)
            {
                logs = new JsonArray();
            }
        }
        else
        {
            logs = new JsonArray();
            File.WriteAllText(file, JsonSerializer.Serialize(logs, options));
        }
        logs.Add(JsonDocument.Parse(logEntryJson).RootElement);
        File.WriteAllText(file, JsonSerializer.Serialize(logs, options));
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
        Logger("Search list", new { query, ToList = element });
        return element;
    }

    public List<T> Sort(string sortBy)
    {
        var element = userActions.Sort(sortBy);
        Logger("Sort list",new{sortBy,ToList = element});
        return element;
    }

    public T Create(T student)
    {
        var element = userActions.Create(student);
        Logger("Sort list",element);
        return element;
    }

    public T Edit(int getId, string attribute, object value)
    {
        var element = userActions.Edit(getId,attribute,value);
        Logger("Edit by",new{id = getId, attribute = attribute, tochange = value, element});
        return element;
    }

    public T Delete(int id)
    {
        var element = userActions.Delete(id);
        Logger("Delete by id",id);
        return element;
    }
}