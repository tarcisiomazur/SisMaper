using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SisMaper.Tools;

public static class Logger
{
    public enum Level
    {
        Message,
        Warning,
        Critical
    }

    public static bool ConsoleLog { get; set; } = true;

    public static string? FileName { get; set; } = "SisMaper";
        
    public static void Write(string message, Level level = 0)
    {
        if(ConsoleLog)
            WriteOnConsole(message, level);
        var sw = new StreamWriter( FileName + " - " + DateTime.Now.ToString("dd-MM-yy") + ".log");
        sw.WriteLine(message);
        sw.Close();
    }

    private static void WriteOnConsole(string message, Level level = 0)
    {
        var fore = Console.ForegroundColor;

        Console.ForegroundColor = level switch
        {
            Level.Critical => ConsoleColor.DarkRed,
            Level.Warning => ConsoleColor.DarkYellow,
            Level.Message => ConsoleColor.DarkGreen,
            _ => Console.ForegroundColor
        };
        Console.WriteLine(message);
        Console.ForegroundColor = fore;
    }

    public static void Log(string message, Level level = 0)
    {
        var stack = GenerateStack(new StackTrace(true));
        Write($"[{DateTime.Now.ToString("g")}] - {level.ToString()}\n{stack}\n{message}\n", level);
    }
    
    private static void Log(string message, Level level, StackTrace st)
    {
        var stack = GenerateStack(st);
        Write($"[{DateTime.Now.ToString("g")}] - {level.ToString()}\n{stack}\n{message}\n", level);
    }
    
    public static void Error(string message)
    {
        Log(message, Level.Critical, new StackTrace(true));
    }
    
    public static void Message(string message)
    {
        Log(message, Level.Message, new StackTrace(true));
    }
    
    public static void Warning(string message)
    {
        Log(message, Level.Warning, new StackTrace(true));
    }

    private static string GenerateStack(StackTrace st)
    {
        var stackIndent = "";
        var sb = new StringBuilder();
        for (var i = 1; i < st.FrameCount; i++)
        {
            var sf = st.GetFrame(i);
            sb.AppendLine(stackIndent + $" Method: {sf.GetMethod()}");
            sb.AppendLine(stackIndent + $" File: {sf.GetFileName()}");
            sb.AppendLine(stackIndent + $" Line Number: {sf.GetFileLineNumber()}");
            stackIndent += "  ";
        }

        return sb.ToString();
    }
}