using System;

public class LoggerService : ILoggerService
{
    public void LogAction(string message)
    {
        Console.WriteLine($"LOG: {message}"); 
    }
}
