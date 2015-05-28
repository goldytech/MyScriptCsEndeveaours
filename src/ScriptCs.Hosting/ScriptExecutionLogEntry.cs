namespace ScriptCs.Hosting
{
    using ScriptCs.Contracts;

    public class ScriptExecutionLogEntry
    {
        public ScriptExecutionLogEntry(string logEntry, LogLevel logLevel)
        {
            this.Level = logLevel;
            this.LogEntry = logEntry;
        }

        public LogLevel Level { get; private set; }

        public string LogEntry { get; private set; }
    }
}