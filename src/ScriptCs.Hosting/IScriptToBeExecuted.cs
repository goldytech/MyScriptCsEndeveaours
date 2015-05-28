namespace ScriptCs.Hosting
{
    using ScriptCs.Contracts;

    public interface IScriptToBeExecuted
    {
        string ScriptContent
        {
            get;
            set;
        }
        bool UseMono
        {
            get;
            set;
        }
        string[] NuGetDependencies
        {
            get;
            set;
        }
        string[] Namespaces
        {
            get;
            set;
        }
        string[] LocalDependencies
        {
            get;
            set;
        }
        LogLevel LogLevel
        {
            get;
            set;
        }
    }
}