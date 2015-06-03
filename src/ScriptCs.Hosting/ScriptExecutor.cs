namespace ScriptCs.MyHosting
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Versioning;

    using Common.Logging;

    using ScriptCs.Contracts;
    using ScriptCs.Engine.Mono;
    using ScriptCs.Engine.Roslyn;
    using ScriptCs.Hosting;

    using LogLevel = ScriptCs.Contracts.LogLevel;

    public class ScriptExecutor
    {
        /// <summary>
        /// The reply.
        /// </summary>
        private readonly Action<object> reply;

        private ScriptServicesBuilder scriptservicesBuilder;

        private IScriptExecutor scriptExecutor;

        private IScriptToBeExecuted scriptToBeExecuted;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptExecutor"/> class.
        /// </summary>
        /// <param name="paramReply">
        /// The param reply.
        /// </param>
        /// <param name="scriptToBeExecuted">
        /// The script to be executed.
        /// </param>
        public ScriptExecutor(Action<object> paramReply, IScriptToBeExecuted scriptToBeExecuted)
        {
            if (paramReply == null)
            {
                throw new ArgumentNullException("paramReply");
            }

            if (scriptToBeExecuted == null)
            {
                throw new ArgumentNullException("scriptToBeExecuted");
            }

            this.reply = paramReply;
            this.scriptToBeExecuted = scriptToBeExecuted;
            this.CreateScriptServices(false, LogLevel.Debug, this.reply);
        }

        public ScriptResult Execute()
        {
            var scriptServices = this.SetupExecution();
            var scriptResult = this.scriptExecutor.Execute(this.scriptToBeExecuted.ScriptContent, string.Empty);
            return this.EvaluateScriptResult(scriptResult, scriptServices);

        }

        private ScriptResult EvaluateScriptResult(ScriptResult scriptResult, ScriptServices scriptServices)
        {
            if (scriptResult != null)
            {
                if (scriptResult.CompileExceptionInfo != null)
                {
                    if (scriptResult.CompileExceptionInfo.SourceException != null)
                    {
                        scriptServices.Logger.Debug(scriptResult.CompileExceptionInfo.SourceException.Message);
                    }
                }
            }
            this.scriptExecutor.Terminate();

            this.reply(new ScriptExecutionLifetimeStatus
            {
                ExecutionLifetime = ScriptExecutionLifetime.Terminated
            });

            return scriptResult;
        }

        private ScriptServices SetupExecution()
        {
            this.reply(new ScriptExecutionLifetimeStatus
            {
                ExecutionLifetime = ScriptExecutionLifetime.Started
            });
            var scriptServices = this.scriptservicesBuilder.Build();
            Environment.CurrentDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, scriptServices.FileSystem.BinFolder);
            this.scriptExecutor = scriptServices.Executor;
            var scriptPackResolver = scriptServices.ScriptPackResolver;
            scriptServices.InstallationProvider.Initialize();
            var assemblies = this.PreparePackages(
                scriptServices.PackageAssemblyResolver,
                scriptServices.PackageInstaller,
                this.PrepareAdditionalPackages(this.scriptToBeExecuted.NuGetDependencies),
                this.scriptToBeExecuted.LocalDependencies,
                scriptServices.Logger);

            this.scriptExecutor.Initialize(assemblies, scriptPackResolver.GetPacks());
            var namespaces = this.scriptToBeExecuted.Namespaces;
            if (namespaces != null)
            {
                this.scriptExecutor.ImportNamespaces(namespaces);
            }
            var localDependencies = this.scriptToBeExecuted.LocalDependencies;
            if (localDependencies != null)
            {
                this.scriptExecutor.AddReferences(localDependencies);
            }
            return scriptServices;




        }
        private IEnumerable<IPackageReference> PrepareAdditionalPackages(IEnumerable<string> dependencies)
        {
            if (dependencies != null)
            {
                return from dep in dependencies
                       select new PackageReference(dep, new FrameworkName(".NETFramework,Version=v4.0"), string.Empty);
            }

            return null;
        }

        private IEnumerable<string> PreparePackages(IPackageAssemblyResolver packageAssemblyResolver, IPackageInstaller packageInstaller, IEnumerable<IPackageReference> additionalNuGetReferences, IEnumerable<string> localDependencies, ILog logger)
        {
            var workingDirectory = Environment.CurrentDirectory;

            var packages = packageAssemblyResolver.GetPackages(workingDirectory);
            if (additionalNuGetReferences != null)
            {
                packages = packages.Concat(additionalNuGetReferences);
            }

            try
            {
                packageInstaller.InstallPackages(
                    packages, true);
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Installation failed: {0}.", e.Message);
            }
            var assemblyNames = packageAssemblyResolver.GetAssemblyNames(workingDirectory);
            if (localDependencies != null)
            {
                assemblyNames = assemblyNames.Concat(localDependencies);
            }
            return assemblyNames;
        }

        private void CreateScriptServices(bool useMono, LogLevel logLevel, Action<object> reply)
        {
            var console = new MessagingConsole(reply);
            var configurator = new LoggerConfigurator(logLevel);
            configurator.Configure(console);
            var logger = configurator.GetLogger();
            this.scriptservicesBuilder = new ScriptServicesBuilder(console, logger);

            if (useMono)
            {
                this.scriptservicesBuilder.ScriptEngine<MonoScriptEngine>();
            }
            else
            {
                this.scriptservicesBuilder.ScriptEngine<RoslynScriptEngine>();
            }

            this.scriptservicesBuilder.FileSystem<FileSystem>();
        }
    }

    public enum ScriptExecutionLifetime
    {
        Started = 0,
        Terminated = 1,
    }
}