using System.Net;
using System.Configuration;

namespace cloudLogger
{
  public class CosmosLogger
  {
    public static Action<string>? OnLogNotification;
    static CosmosLogger()
    {
    InitLogPath();
    }
    // Helper class for Cosmos Logging utilities.
    private static string? _activeLogPath;
    private static void InitLogPath()
    {
      int counter = 1;
      string prefix = ConfigurationManager.AppSettings["log_file"] ?? "log_";
      string fullPath = "";
      try
      {
        string logPath = ConfigurationManager.AppSettings["log_dir"] ?? "./Logs/";
        if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);
        while (true)
        {
          string logFileName = $"{prefix}{counter}.txt";
          fullPath = Path.Combine(logPath,logFileName);
          if (File.Exists(fullPath)) counter++;
          else
          {
            _activeLogPath = fullPath;
            break;
          }
        }
      
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Error reading log_path: {ex.Message}",
        "Error",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
        return;
      }
    }
    private static string ReadLog(string fullLogPath)
    {
      try
      {
        string logContent = "";
        if (File.Exists(fullLogPath))
        {
          logContent = File.ReadAllText(fullLogPath);
          return logContent;
        }
        else return "";
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Error reading Log: {ex.Message}",
        "Error",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
        return "";
      }
    }
    public static void Log(string msg)
    {
      if (string.IsNullOrEmpty(_activeLogPath)) return;
      string currTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      string logData = "";
      try
      {
        logData = $"{currTime} | {msg}{Environment.NewLine}";
        File.AppendAllText(_activeLogPath,logData);
        OnLogNotification?.Invoke(logData);
        Console.Write("Log recorded successfully");
      }
      catch (Exception ex)
      {
        Console.Write($"Error writing to Log: {ex.Message}");
        return;
      }
    }
  }
}