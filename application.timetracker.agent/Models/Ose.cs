using System;
using System.Collections.Generic;

#nullable disable

namespace application.timetracker.agent.Models
{
    public partial class Ose
    {
        public string Os { get; set; }
        public int? UserId { get; set; }
        public string Platform { get; set; }
    }
}
