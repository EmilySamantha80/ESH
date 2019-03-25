using ESH.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESH.Examples
{
    public class ImpersonationExample
    {
        public static void ImpersonateUser()
        {
            using (new Impersonation("mydomain.com", "username", "password", Impersonation.LogonType.NewCredentials))
            {
                // Do stuff as the user
            }
        }
    }
}
