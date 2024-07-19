using System;
using System.Collections.Generic;

namespace application.timetracker.agent.monitoring.statistic
{
    public class ApplicationStatistic 
    {
        public readonly string ApplicationName;

        public ApplicationStatistic(string appName)
        {
            ApplicationName = appName;
        }


        public readonly Dictionary<string, ApplicationUser> Users = new();

        public int AppId { get; set; } = 1;

        public double AppUID { get; set; } = 22;


        public void Finish(DateTime collectorTime, string userName)
        {
            Users[userName].Finish(collectorTime);
        }

        public void Update(DateTime collectorTime, ApplicationInfo statistic)
        {
            ApplicationUser user = null;
            /*
             * #1 New User
                1. Do Register new user

             * #2 Existing User
             *  2. Do Update user  
            */

            // Check if user exists
            // Update User
            // else
            // Create User

            if (Users.TryGetValue(statistic.UserName, out user))
            {
                UpdateStatistic(collectorTime, user, statistic);
            }
            else
            {
                RegisterNewUser(collectorTime, statistic);
            }

        }

        private void RegisterNewUser(DateTime collectorTime, ApplicationInfo userName)
        {
            var user = new ApplicationUser(userName.UserName);

            UpdateStatistic(collectorTime, user, userName);

            Users.TryAdd(userName.UserName, user);
        }

        private void UpdateStatistic(DateTime collectorTime, ApplicationUser user, ApplicationInfo userName )
        {
            user.Update(collectorTime, userName);
        }
    }
}
