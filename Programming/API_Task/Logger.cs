using System.Text.Json;
using System.Text.Json.Nodes;
using API_Task.Models;

namespace API_Task;

public class Logger
{
    public static void LoggerJson(string action, object value, User user)
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
        string file = "logs.json";
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

}

