using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace application.timetracker.agent.monitoring.configuration
{
    public sealed class UserManager
    {
        private static readonly Lazy<UserManager> lazy = new Lazy<UserManager>(() => new UserManager());

        private List<User> _users = new List<User>();

        public static UserManager Instance { get { return lazy.Value; } }

        private UserManager()
        {
        }
       
        public User GetUserByName(string userName)
        {

            return 
                _users
                    .FirstOrDefault<User>((user) => user.Name == userName);
        }

        public IEnumerable<User> GetUsersByGroup(string groupName)
        {
            
            return null;
        }
    }
}
