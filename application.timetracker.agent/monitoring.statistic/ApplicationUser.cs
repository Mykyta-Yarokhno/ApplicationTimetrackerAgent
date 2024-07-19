using System;
using System.Collections.Generic;
using System.Linq;

namespace application.timetracker.agent.monitoring.statistic
{
    public class ApplicationUser
    {
        public readonly string UserName;

        public TimeSpan TotalActiveTime;

        public TimeSpan CurrentActiveTime;

        public List<ApplicationRunningTime> Times = new();


        public ApplicationUser(string userName)
        {
            UserName = userName;
        }

        public void Update(DateTime collectorTime, ApplicationInfo statistic)
        {
            if (Times.Count == 0)
            {
                // Application first time started
                var appTime = new ApplicationRunningTime();

                appTime.StartTime = collectorTime;

                Times.Add(appTime);
            }
            
            if (Times.Last().EndTime != DateTime.MinValue)
            {
                // Last Application execution is finished
                Times.Last().EndTime = collectorTime;
            }
            else
            {
                // Last Application is executing now
                // Do nothing
            }
        }

        public void Finish(DateTime collectorTime)
        {
            Times.Last().EndTime = collectorTime;
        }
    }
}
