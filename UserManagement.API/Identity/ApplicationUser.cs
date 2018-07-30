using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace UserManagement.API.Identity
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base()
        {
            PreviousUserPasswords = new List<PreviousPassword>();
        }
        public virtual IList<PreviousPassword> PreviousUserPasswords { get; set; }
    }
}
