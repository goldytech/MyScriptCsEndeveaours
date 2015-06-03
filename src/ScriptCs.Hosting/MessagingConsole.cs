namespace ScriptCs.MyHosting
{
    using System;

    using ScriptCs.Contracts;
    using ScriptCs.Hosting;

    public class MessagingConsole : IConsole
    {
        private readonly Action<object> reply;

        public MessagingConsole(Action<object> reply)
        {
            if (reply == null)
            {
                throw new ArgumentNullException("reply");
            }

            this.reply = reply;

            Console.SetOut(new MessagingWriter(reply));
        }

        public void Write(string value)
        {
            throw new NotImplementedException();
        }

        public void WriteLine()
        {
        }

        public void WriteLine(string value)
        {
            object reply = null;

            if (value.StartsWith("DEBUG:"))
            {
                reply = new ScriptExecutionLogEntry(value, LogLevel.Debug);
            }

            if (value.StartsWith("INFO:"))
            {
                reply = new ScriptExecutionLogEntry(value, LogLevel.Info);
            }

            if (value.StartsWith("ERROR:"))
            {
                reply = new ScriptExecutionLogEntry(value, LogLevel.Error);
            }

            if (value.StartsWith("TRACE:"))
            {
                reply = new ScriptExecutionLogEntry(value, LogLevel.Trace);
            }

            if (reply != null)
            {
                this.reply(reply);
            }
        }

        public string ReadLine()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }

        public void ResetColor()
        {
            throw new NotImplementedException();
        }

        public ConsoleColor ForegroundColor
        {
            get
            {
                return Console.ForegroundColor;
            }
            
            set
            {
                Console.ForegroundColor = value;
            }
        }
    }
}