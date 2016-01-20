using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRC
{
    public class User
    {
        private String nick;
        public String Nick
        {
            get { return nick; }
            set { nick = value; }
        }
        public User(String nick){
            this.nick=nick;

        }
    }
}
