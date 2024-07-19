using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System;

using NLog;
using NLog.Config;
using NLog.Targets;

namespace application.timetracker.agent
{
    using application.timetracker.agent.Models;
    using application.timetracker.agent.runners;
    using application.timetracker.agent.test;

    using MyApp = test.Application;
    using ModelApp = Models.Application;

    [SupportedOSPlatform("windows")]
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            ConfigureLogger();

            var commandLineArgs = AgentCommandLineParameters.Parse(ref args);

            IAgentRunner runner = null;

            if (commandLineArgs.RunAsService)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    runner = AgentWindowsServiceRunner.Create();
                }
            }
            else
            {
                runner = AgentDebugRunner.Create();
            }

            runner?.Run();
        }

        static void ConfigureLogger()
        {
            
            var config = new LoggingConfiguration();

            var fileTarget =
                new FileTarget
                {
                    Name = "file",
                    Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}",
                    FileName = $"Log/time_tracker_{DateTime.Now.ToString("dd.MM.yyyy")}.log"
                };

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTarget, "*");

            
            LogManager.Configuration = config;
        }
    }
}
