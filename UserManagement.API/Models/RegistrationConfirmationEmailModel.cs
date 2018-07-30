using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.API.Models
{
    public class RegistrationConfirmationEmailModel
    {
        public string UserName { get; set; }

        public string SenderName { get; set; }
    }
}
