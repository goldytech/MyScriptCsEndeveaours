namespace ScriptCs.MyHosting
{
    public class ScriptExecutionConsoleOutput
    {
        public ScriptExecutionConsoleOutput(string output)
        {
            this.Output = output;
        }

        public string Output { get; private set; }
    }
}