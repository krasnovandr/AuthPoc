using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.API
{
    public class AuthMessageSenderOptions
    {
        public string SendGridKeyName { get; set; }
        public string SendGridKeyValue { get; set; }
    }
}
