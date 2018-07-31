using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagement.API.Identity;
using UserManagement.API.Models;
using UserManagement.API.Models.Dtos;
using UserManagement.Notification;
using UserManagement.Notification.Models;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    public class IdentityController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IdentityController> _logger;
        private readonly INotificationService _notificationService;


        public IdentityController(
            UserManager<ApplicationUser> userManager,
            ILogger<IdentityController> logger,
            IHostingEnvironment appEnvironment,
            INotificationService notificationService)
        {
            _userManager = userManager;
            _logger = logger;
            _notificationService = notificationService;
        }


        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return NotFound("Your account wasn't found");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            //todo update PasswordReset with client url
            string callbackUrl = Url.Link("ResetPassword", new { userId = user.Id, code = code });

            var model = new ResetPasswordEmailModel()
            {
                Email = user.Email,
                ResetPasswordLink = callbackUrl
            };
            await _notificationService.NotifyAsync(model);

            return Ok(model);
        }

        [HttpPost("resetPassword", Name = "ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Code, resetPasswordDto.Password);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            AddErrors(result);

            return BadRequest(result);
        }


        [HttpGet("getEmail")]
        public async Task<IActionResult> CheckEmailAsync()
        {
            var model = new UserActivationEmailModel
            {
                UserName = "User",
                Email = "andrei_krasnou@epam.com",
                ConfirmationLink = "somelink"
            };

            await _notificationService.NotifyAsync(model);

            return NoContent();
        }


        // GET api/values
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _userManager.Users.ToListAsync());
        }

        [HttpGet("{userId}", Name = "IdentityById")]
        public async Task<IActionResult> GetByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            return Ok(await _userManager.FindByIdAsync(userId.ToString()));
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return NotFound($"user with specified id {userId} wasn't foundd in the system");
            }

            var result = await _userManager.DeleteAsync(user);

            return Ok(result);
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync([FromBody] RegisterDto registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = registerModel.Email,
                Email = registerModel.Email,

            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");


                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string callbackUrl = Url.Link("ConfirmEmail", new { userId = user.Id, code = code });

                var model = new UserActivationEmailModel
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    ConfirmationLink = callbackUrl
                };

                await _notificationService.NotifyAsync(model);

                //await _signInManager.SignInAsync(user, isPersistent: false);

                _logger.LogInformation("User created a new account with password.");
                return Created("", result);
            }

            AddErrors(result);
            return BadRequest(result);
        }


        [HttpGet("confirmEmail", Name = "ConfirmEmail")]

        public async Task<IActionResult> ConfirmEmail(Guid userId, string code)
        {
            if (Guid.Empty == userId || string.IsNullOrEmpty(code))
            {
                BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return NotFound($"user with specified id {userId} wasn't foundd in the system");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded == false)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("changePassword/{userId}")]
        public async Task<IActionResult> ChangePasswordAsync(Guid userId, [FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return BadRequest(changePasswordResult);
            }

            //await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");

            return Ok(changePasswordResult);
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
