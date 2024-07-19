using System;
using System.Collections.Generic;

#nullable disable

namespace application.timetracker.agent.Models
{
    public partial class AppRunningTime
    {
        public DateTime StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public int AppId { get; set; }
        public int UserId { get; set; }

        public virtual Application App { get; set; }
        public virtual User User { get; set; }
    }
}
