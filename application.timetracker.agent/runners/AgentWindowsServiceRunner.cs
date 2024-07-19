using System.Runtime.Versioning;
using System.ServiceProcess;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace application.timetracker.agent.runners
{
    public class AgentWindowsServiceRunner : IAgentRunner
    {
        public static AgentWindowsServiceRunner Create()
        {
            return new AgentWindowsServiceRunner();
        }

        [SupportedOSPlatform("windows")]
        public void Run()
        {
            ServiceBase.Run(new ApplicationTimeTrackerAgent());


            CreateHostBuilder().Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder()
        {
            //Host.CreateDefaultBuilder()
            //        .ConfigureServices((hostContext, services) =>
            //            /*services.AddHostedService<ImageClassifierWorker>()*/
            //            )
            //        .UseWindowsService();

            return null;
        }
    }
}
