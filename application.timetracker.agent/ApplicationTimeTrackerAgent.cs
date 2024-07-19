using System;
using System.ServiceProcess;

using NLog;

namespace application.timetracker.agent
{
    using application.timetracker.agent.monitoring.configuration;
    using application.timetracker.agent.monitoring.statistic;

    

    public partial class ApplicationTimeTrackerAgent : ServiceBase
    {
        private ILogger _log = LogManager.GetLogger("DebugRunner");

        public ApplicationTimeTrackerAgent()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Console.WriteLine("********************************************");
            Console.WriteLine("* Welcome to Agent (v.0.1.)");
            Console.WriteLine("********************************************");

            _log.Info("********************************************");
            _log.Info("* Welcome to Agent (v.0.1.)");
            _log.Info("********************************************");


            Console.WriteLine("Starting ...");

            // Create configuration
            var trackerConfig = new ActivityTrackerConfiguration();


            var tracker = new ApplicationActivityTracker();
            var collector = new ApplicationStatisticCollector();

            // Subscribe to the new statistic data
            tracker.ApplicationStatisticReady += collector.OnUpdateStatistic;

            // Run process tracker
            tracker.Start(trackerConfig);
        }

        protected override void OnStop()
        {
        }
    }
}
