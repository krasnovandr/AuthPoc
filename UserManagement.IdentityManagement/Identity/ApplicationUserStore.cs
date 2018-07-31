using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Identity;

namespace UserManagement.IdentityManagement.Identity
{
    public class ApplicationUserStore : UserStore<ApplicationUser>
    {
           private const int UsedPasswordLimit = 3;

        public ApplicationUserStore(ApplicationDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        public override async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = new CancellationToken())
        {
            var identity = await base.CreateAsync(user, cancellationToken);

            await AddToPreviousPasswordsAsync(user, user.PasswordHash);

            return identity;
        }

        public override async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = new CancellationToken())
        {
            var user = await
                this.Users
                    .Include(t => t.PreviousUserPasswords)
                    .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

            return user;
        }

        public override async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken = new CancellationToken())
        {
            var user = await
                this.Users
                    .Include(t => t.PreviousUserPasswords)
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            return user;
        }
        //public override async Task<ApplicationUser> FindByIdAsync(string normalizedEmail, CancellationToken cancellationToken = new CancellationToken())
        //{
        //    var user = await
        //        this.Users
        //            .Include(t => t.PreviousUserPasswords)
        //            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

        //    return user;
        //}

        //FindByIdAsync

        public async Task AddToPreviousPasswordsAsync(ApplicationUser user, string password)
        {
            user.PreviousUserPasswords.Add(new PreviousPassword
            {
                UserId = user.Id,
                PasswordHash = password,
            });

            await UpdateAsync(user);
        }
    }
}
