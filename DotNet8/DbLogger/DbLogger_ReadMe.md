-------------------------------------------------------------------------
# IN WEB.CONFIG :
-------------------------------------------------------------------------
```json
"Logging": {
   "Database": {
     "Options": {
       "ConnectionString": "Server=.....",
       "LogFields": [
         "LogLevel",
         "ThreadId",
         "EventId",
         "EventName",
         "ExceptionMessage",
         "ExceptionStackTrace",
         "ExceptionSource"
       ],
       "LogTable": "Errors"
     },
     "LogLevel": {
       "Default": "Warning",
       "Microsoft.AspNetCore": "Error"
     }
   }
},
```
-------------------------------------------------------------------------
# IN PROGRAM :
-------------------------------------------------------------------------
```csharp
using Calendarium.DbLogger;

builder.Logging.AddDbLogger(options =>
    builder.Configuration.GetSection("Logging").GetSection("Database").GetSection("Options").Bind(options));
```
-------------------------------------------------------------------------
# IN A CONTROLLER :
-------------------------------------------------------------------------
```csharp
_logger.LogWarning(new EventId(1), new Exception("WARNING TEST"), "");
_logger.LogError(new EventId(1), new Exception("ERROR TEST"), "");
```