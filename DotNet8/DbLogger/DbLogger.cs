// using Microsoft.Data.SqlClient;
// using Newtonsoft.Json.Linq;
// using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace DotNet8.DbLogger
{
    public class DbLogger : ILogger
    {
        private readonly DbLoggerProvider _dbLoggerProvider;

        public DbLogger([NotNull] DbLoggerProvider dbLoggerProvider)
        {
            _dbLoggerProvider = dbLoggerProvider;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) // Don't log the entry if it's not enabled.
            {
                return;
            }

            var threadId = Thread.CurrentThread.ManagedThreadId; // Get the current thread ID to use in the log file. 

            //using (var connection = new SqlConnection(_dbLoggerProvider.Options.ConnectionString))
            //{
            //    connection.Open();

            //    var jsonValues = new JObject();

            //    if (_dbLoggerProvider?.Options?.LogFields?.Any() ?? false)
            //    {
            //        foreach (var logField in _dbLoggerProvider.Options.LogFields)
            //        {
            //            switch (logField)
            //            {
            //                case "LogLevel":
            //                    if (!string.IsNullOrWhiteSpace(logLevel.ToString()))
            //                    {
            //                        jsonValues["LogLevel"] = logLevel.ToString();
            //                    }
            //                    break;
            //                case "ThreadId":
            //                    jsonValues["ThreadId"] = threadId;
            //                    break;
            //                case "EventId":
            //                    jsonValues["EventId"] = eventId.Id;
            //                    break;
            //                case "EventName":
            //                    if (!string.IsNullOrWhiteSpace(eventId.Name))
            //                    {
            //                        jsonValues["EventName"] = eventId.Name;
            //                    }
            //                    break;
            //                case "Message":
            //                    if (!string.IsNullOrWhiteSpace(formatter(state, exception)))
            //                    {
            //                        jsonValues["Message"] = formatter(state, exception);
            //                    }
            //                    break;
            //                case "ExceptionMessage":
            //                    if (exception != null && !string.IsNullOrWhiteSpace(exception.Message))
            //                    {
            //                        jsonValues["ExceptionMessage"] = exception?.Message;
            //                    }
            //                    break;
            //                case "ExceptionStackTrace":
            //                    if (exception != null && !string.IsNullOrWhiteSpace(exception.StackTrace))
            //                    {
            //                        jsonValues["ExceptionStackTrace"] = exception?.StackTrace;
            //                    }
            //                    break;
            //                case "ExceptionSource":
            //                    if (exception != null && !string.IsNullOrWhiteSpace(exception.Source))
            //                    {
            //                        jsonValues["ExceptionSource"] = exception?.Source;
            //                    }
            //                    break;
            //            }
            //        }
            //    }

            //    var values = JsonConvert.SerializeObject(jsonValues, new JsonSerializerSettings
            //    {
            //        NullValueHandling = NullValueHandling.Ignore,
            //        DefaultValueHandling = DefaultValueHandling.Ignore,
            //        Formatting = Formatting.None
            //    }).ToString();

            //    using (var command = new SqlCommand())
            //    {
            //        command.Connection = connection;
            //        command.CommandType = System.Data.CommandType.Text;
            //        command.CommandText = string.Format("INSERT INTO {0} ([Values], [Created]) VALUES (@Values, @Created)", _dbLoggerProvider.Options.LogTable);

            //        command.Parameters.Add(new SqlParameter("@Values", values));

            //        command.Parameters.Add(new SqlParameter("@Created", DateTimeOffset.Now));

            //        command.ExecuteNonQuery();
            //    }

            //    connection.Close();
            //}
        }
    }
}
