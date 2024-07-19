using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace application.timetracker.agent.utils
{
    public static class PrintDebugExtenssions
    {
        public static string DebugPrintString(this ApplicationInfo item)
        {
            return  $@"AppName:'{item.ApplicationName}', AppId:'{item.ApllicationId}', ProcId:'{item.ProcessId}', User:'{item.UserName}'";
        }
    }
}
