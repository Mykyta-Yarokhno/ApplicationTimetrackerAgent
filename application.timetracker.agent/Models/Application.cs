using System;
using System.Collections.Generic;

#nullable disable

namespace application.timetracker.agent.Models
{
    public partial class Application
    {
        public Application()
        {
            AppRunningTimes = new HashSet<AppRunningTime>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AppRunningTime> AppRunningTimes { get; set; }
    }
}
