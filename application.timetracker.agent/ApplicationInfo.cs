using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace application.timetracker.agent
{
    public class ApplicationInfo
    {
        private string _appName;

        public string ApllicationId {
            get => _appName ?? ApplicationName; 
            set=> _appName = value; 
            }

        public string ApplicationName { get; set; }
        
        public long ProcessId;


        public string UserName;
                
        //public override string ToString()
        //{
        //    return $@"AppName:'{ApplicationName}', AppId:'{ApllicationId}', ProcId:'{ProcessId}', User:'{UserName}'";

        //    //StringBuilder t = new StringBuilder();
        //    //t.Append("AppName: :").Append(ApplicationName);
        //    ////"AppName:'" +  +" AppId:" + ApllicationId}', ProcId:'{ProcessId}', User:'{UserName}'";
        //    //t.Append("");
        //    //return t.ToString();
        //}
    }
}
