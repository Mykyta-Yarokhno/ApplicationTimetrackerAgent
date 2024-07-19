using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace application.timetracker.agent.monitoring.statistic
{
    public class User
    {
        public string Name;
        public IEnumerable<ApplicationRunningTime> Times;

        public DateTime TotalActiveTime;


        
    }
}
