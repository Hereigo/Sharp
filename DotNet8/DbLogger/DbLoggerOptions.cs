namespace Calendarium.DbLogger
{
    public class DbLoggerOptions
    {
        public string ConnectionString { get; init; } = string.Empty;

        public string[] LogFields { get; init; } = [];

        public string LogTable { get; init; } = string.Empty;
    }
}
