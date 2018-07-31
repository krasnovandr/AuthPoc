using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserManagement.API.Identity;
using UserManagement.Common;

namespace UserManagement.IdentityManagement.Identity
{
    public class CustomUserManager : UserManager<ApplicationUser>
    {
        private const int UsedPasswordLimit = 3;
        public CustomUserManager(
            IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public override async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            if (await IsPreviousPassword(user.Id, newPassword))
            {
                return await Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Description = "You Cannot Reuse Previous Password"
                }));
            }

            var result = await base.ChangePasswordAsync(user, currentPassword, newPassword);

            if (result.Succeeded)
            {
                await UpdatePasswordInfo(user);
                await AddPasswordToHistory(user, newPassword);
            }

            return result;
        }

        public override async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            if (await IsPreviousPassword(user.Id, newPassword))
            {
                return await Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Description = "You Cannot Reuse Previous Password"
                }));
            }

            var result = await base.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                await UpdatePasswordInfo(user);
                await AddPasswordToHistory(user, newPassword);
            }

            return result;
        }

        private async Task UpdatePasswordInfo(ApplicationUser user)
        {
            user.ChangePasswordRequired = false;
            user.LastPasswordChangedDate = DateTime.UtcNow;
            await UpdateAsync(user);
        }

        private async Task AddPasswordToHistory(ApplicationUser user, string newPassword)
        {
            var appStore = Store as ApplicationUserStore;
            Guard.ArgumentNotNull(nameof(appStore), appStore);

            await appStore.AddToPreviousPasswordsAsync(user, PasswordHasher.HashPassword(user, newPassword));
        }

        private async Task<bool> IsPreviousPassword(string userId, string newPassword)
        {
            var user = await FindByIdAsync(userId);

            if (user.PreviousUserPasswords
                .OrderByDescending(up => up.CreateDate)
                .Select(up => up.PasswordHash)
                .Take(UsedPasswordLimit)
                .Any(up => PasswordHasher.VerifyHashedPassword(user, up, newPassword) != PasswordVerificationResult.Failed))
            {
                return true;
            }

            return false;
        }
    }
}
