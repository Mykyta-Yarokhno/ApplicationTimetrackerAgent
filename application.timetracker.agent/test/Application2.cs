using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace application.timetracker.agent.test
{
    public class Application
    {
        public static int stest = 22;
        public int test =0;

        public int Test2()
        {
            return Test3() + 22;
        }

        public static int Test3()
        {
            return Test4() + 222;
        }

        public static int Test4()
        {
            return 444;
        }
    }
    
}
