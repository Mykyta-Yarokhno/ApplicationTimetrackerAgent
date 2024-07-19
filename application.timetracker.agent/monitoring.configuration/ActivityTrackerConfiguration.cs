using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace application.timetracker.agent.monitoring.configuration
{
    public class ActivityTrackerConfiguration
    {
        private List<UserConfiguration> _configuration = new List<UserConfiguration>(); 

        public IEnumerable<UserConfiguration> UserConfiguration => _configuration;
        
        public long TrackIntervalSec { get; } = 10;

    }
}
