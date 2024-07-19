using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace application.timetracker.agent
{
    public class ApplicationStatisticRaw
    {
        public TimeSpan StatisticTimeSlice { get; set; }
        
        public string ActiveApplicationId { get; set; }

        public List<ApplicationInfo> Applications { get; set; } = new List<ApplicationInfo>();

        public DateTime StatisticTime { get; set; }
    }
}
