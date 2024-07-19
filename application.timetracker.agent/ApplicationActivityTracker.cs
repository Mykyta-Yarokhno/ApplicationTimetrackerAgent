using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace application.timetracker.agent
{
    using application.timetracker.agent.monitoring.configuration;
    using application.timetracker.agent.utils;
    using application.timetracker.agent.utils.win32;

    public class ApplicationActivityTracker
    {
        #region win32 API

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        #endregion

        public delegate void ApplicationStatisticDelegate(ApplicationStatisticRaw statistic);

        private ActivityTrackerConfiguration    _config;
        
        private ApplicationStatisticRaw         _lastStatistic;
        
        private CancellationTokenSource         _cancelSrc = new CancellationTokenSource();

        private Task                            _trackTask = null;

        private bool                            _stopped = false;


        public event ApplicationStatisticDelegate ApplicationStatisticReady;

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Tracker was previously stopped or is stopping now</exception>
        /// <param name="trackerConfig"></param>
        public void Start(ActivityTrackerConfiguration trackerConfig)
        {
            #region Validate the Tracker state
            if (_stopped)
            {
                throw new InvalidOperationException("Tracker is already stopped");
            }

            if (_cancelSrc.IsCancellationRequested)
            {
                throw new InvalidOperationException("Tracker is stopping now");
            }
            #endregion


            _config = trackerConfig;

            _lastStatistic = null;

            _trackTask = new Task(() => DoTrackRocesses(trackerConfig, _cancelSrc.Token));

            _trackTask.Start();
        }

        public async Task Stop()
        {
            _cancelSrc.Cancel();

            //
            // Wait for DoTrackRocesses completed asynchronously
            //
            await _trackTask;

            // Set flag to prevent the tracker from starting over again
            _stopped = true;
        }

        /// <summary>
        /// Test func
        /// </summary>
        /// <param name="trackerConfig"></param>
        public void UpdateConfiguration(ActivityTrackerConfiguration trackerConfig)
        {
            
        }
        
        private void DoTrackRocesses(ActivityTrackerConfiguration trackerConfig, CancellationToken cancelToken)
        {
            Console.WriteLine(" + Activity tracker started");

            do
            {
                try
                {
                    // Get list of processes
                    _lastStatistic = GetExecutesProcesses();

                    // Raise event "New static data ready"
                    ApplicationStatisticReady.Invoke(_lastStatistic);

                    // Do sleep
                    Task
                        .Delay(TimeSpan.FromSeconds(trackerConfig.TrackIntervalSec), cancelToken)
                        .Wait(cancelToken);
                }
                catch ( OperationCanceledException)
                {
                    // Activity tracker is stopping
                }
            }
            while (!cancelToken.IsCancellationRequested);


            Console.WriteLine(" - Activity tracker stopped");
        }

        private ApplicationStatisticRaw GetExecutesProcesses()
        {
            var sessionId = Process.GetCurrentProcess().SessionId;

            var statistic = new ApplicationStatisticRaw();

            statistic.StatisticTime = DateTime.Now;
            statistic.StatisticTimeSlice = TimeSpan.FromSeconds(_config.TrackIntervalSec);


            //
            // Select processes of current user
            //
            Process[] procs =
                Process
                    .GetProcesses()
                    .Where(
                        process =>  process.SessionId == sessionId          /* Select all processes executed within the current user session. TODO: move it out */
                                 && process.MainWindowHandle != IntPtr.Zero /* Select all processes that have a window */
                        )
                    .ToArray();

            //
            // Get handle of Window that has a focus
            //
            IntPtr hFocusedWindow = GetForegroundWindow();

            //
            // Iterate selected processes and prepare a statistic record for each of process
            //
            foreach (Process process in procs )
            {

                if (process.HasExited)
                {
                    continue; /* Select only a working processes (exclude finished) */
                }

                string userName = null;
                
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    userName = UAC.GetUserNameByProcess(process);
                }
 
                try
                {
                    //
                    // Create & initialize statistic info for current process
                    //
                    var processStatisticInfo =
                        new ApplicationInfo
                        {
                            ApplicationName = process.ProcessName,
                            ProcessId = process.Id,
                            ApllicationId = process.MainModule?.ModuleName,
                            UserName = userName
                        };

                    //
                    // Check if current process has an active window
                    //
                    if (process.MainWindowHandle == hFocusedWindow)
                    {
                        statistic.ActiveApplicationId = processStatisticInfo.ApllicationId;
                    }

                    //
                    // Do register the process statistic record 
                    //
                    statistic.Applications.Add(processStatisticInfo);
                }
                catch (System.ComponentModel.Win32Exception)
                {

                }
                catch (InvalidOperationException ex)
                {
                    Debug.Write(
                        $"Error: failed to register statistic info for process '{process?.ProcessName ?? "unknown"}'. Error ({ex.Message})"
                        );
                }
            }


            Debug.WriteLine($"** Collect process statistics at '{statistic.StatisticTime.ToShortTimeString()}' **");


            statistic.Applications.ForEach(appInfo => Debug.WriteLine(appInfo.DebugPrintString()));

            Debug.WriteLine($"Active window is '{statistic.ActiveApplicationId}'");
            Debug.WriteLine("**");

            #region Alternative statistic approach for test purpose

            //var test = new Dictionary<string, List<ApplicationInfo>>();

            //foreach (Process proc in procs)
            //{

            //    var item =
            //        new ApplicationInfo
            //        {
            //            ApplicationName = proc.ProcessName,
            //            ProcessId = proc.Id,
            //            ApllicationId = proc.StartInfo?.FileName,
            //            /*UserName = proc.StartInfo?.UserName*/
            //        };

            //    if ( !test.ContainsKey(proc.StartInfo?.UserName))
            //    {
            //        test.Add(proc.StartInfo?.UserName, new List<ApplicationInfo>());
            //    }

            //    test[proc.StartInfo?.UserName].Add(item);


            //    Debug.WriteLine(item.DebugPrintString());

            //}
            #endregion

            return statistic;
        }
    }
}
