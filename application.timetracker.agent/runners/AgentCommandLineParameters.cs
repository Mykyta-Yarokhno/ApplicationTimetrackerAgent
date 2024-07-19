using System.Linq;

using Microsoft.Extensions.Configuration.CommandLine;

namespace application.timetracker.agent.runners
{
    #region Version #1
    public class AgentCommandLineParameters
    {
        private static class CommandLineArguments
        {
            public const string RUN_AS_CONSOLE = "runAsConsole";
            public const string RUN_AS_SERVICE = "runAsService";
        }

        public static AgentCommandLineParameters Parse(ref string[] args)
        {
            return
                new AgentCommandLineParameters
                {
                    RunAsService = args.FirstOrDefault(argName => string.Compare(argName, CommandLineArguments.RUN_AS_CONSOLE, true) == 0) == null
                };
        }

        public bool RunAsService { get; init; } = true;
    }
    #endregion

}
