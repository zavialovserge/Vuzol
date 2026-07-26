using System;
using System.IO;
using System.Text;

namespace WpfApp2.Services
{
    public static class ErrorLogger
    {
        private static readonly string LogFilePath = Path.Combine(Directory.GetCurrentDirectory(), "error_log.txt");
        private static readonly object _lock = new object();

        public static void LogError(Exception exception, string additionalInfo = "")
        {
            lock (_lock)
            {
                try
                {
                    StringBuilder logEntry = new StringBuilder();
                    logEntry.AppendLine("=".PadRight(80, '='));
                    logEntry.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ПОМИЛКА");
                    logEntry.AppendLine("=".PadRight(80, '='));

                    if (!string.IsNullOrWhiteSpace(additionalInfo))
                    {
                        logEntry.AppendLine($"Додаткова інформація: {additionalInfo}");
                        logEntry.AppendLine();
                    }

                    logEntry.AppendLine($"Тип помилки: {exception.GetType().FullName}");
                    logEntry.AppendLine($"Повідомлення: {exception.Message}");
                    logEntry.AppendLine($"Джерело: {exception.Source}");

                    if (!string.IsNullOrWhiteSpace(exception.StackTrace))
                    {
                        logEntry.AppendLine();
                        logEntry.AppendLine("Stack Trace:");
                        logEntry.AppendLine(exception.StackTrace);
                    }

                    if (exception.InnerException != null)
                    {
                        logEntry.AppendLine();
                        logEntry.AppendLine("--- Внутрішня помилка ---");
                        logEntry.AppendLine($"Тип: {exception.InnerException.GetType().FullName}");
                        logEntry.AppendLine($"Повідомлення: {exception.InnerException.Message}");

                        if (!string.IsNullOrWhiteSpace(exception.InnerException.StackTrace))
                        {
                            logEntry.AppendLine("Stack Trace:");
                            logEntry.AppendLine(exception.InnerException.StackTrace);
                        }
                    }

                    if (exception.Data.Count > 0)
                    {
                        logEntry.AppendLine();
                        logEntry.AppendLine("Додаткові дані:");
                        foreach (var key in exception.Data.Keys)
                        {
                            logEntry.AppendLine($"  {key}: {exception.Data[key]}");
                        }
                    }

                    logEntry.AppendLine();
                    logEntry.AppendLine();

                    File.AppendAllText(LogFilePath, logEntry.ToString(), Encoding.UTF8);
                }
                catch
                {
                    // Якщо не вдається записати в лог, ігноруємо помилку логування
                }
            }
        }

        public static void ClearLog()
        {
            lock (_lock)
            {
                try
                {
                    if (File.Exists(LogFilePath))
                    {
                        File.Delete(LogFilePath);
                    }
                }
                catch
                {
                    // Ігноруємо помилки при очищенні
                }
            }
        }

        public static string GetLogPath()
        {
            return LogFilePath;
        }
    }
}