namespace ScriptCsClient
{
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    using ScriptCs.Contracts;
    using ScriptCs.MyHosting;

    public class HelloWorldExecutor : IScriptToBeExecuted
    {

        public HelloWorldExecutor()
        {
            this.ScriptContent = this.GetScriptText();
            this.UseMono = false;
            this.LogLevel = LogLevel.Error;
        }

        private string GetScriptText()
        {
            var fileContents = File.ReadAllText(@"D:\Afzal.Qureshi\My Projects\ScriptCs\HelloWorld.csx", Encoding.UTF8);
            fileContents = Regex.Replace(fileContents, @"\t|\n|\r|\\", string.Empty);
            return fileContents;
        }

        public string ScriptContent { get; set; }

        public bool UseMono { get; set; }

        public string[] NuGetDependencies { get; set; }

        public string[] Namespaces { get; set; }

        public string[] LocalDependencies { get; set; }

        public LogLevel LogLevel { get; set; }
    }
}