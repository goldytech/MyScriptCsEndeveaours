namespace ScriptCs.MyHosting
{
    using System;
    using System.IO;
    using System.Text;

    using ScriptCs.Hosting;

    /// <summary>
    /// The messaging writer.
    /// </summary>
    public class MessagingWriter : TextWriter
    {
        private readonly Action<object> reply;

        public MessagingWriter(Action<object> reply)
        {
            if (reply == null)
            {
                throw new ArgumentNullException("reply");
            }

            this.reply = reply;
        }

        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }

        public override void WriteLine(string value)
        {
            this.reply(new ScriptExecutionConsoleOutput(value));
        }

    }
}