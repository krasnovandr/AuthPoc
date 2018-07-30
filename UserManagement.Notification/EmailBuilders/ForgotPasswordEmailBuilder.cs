using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using UserManagement.Common;
using UserManagement.Notification.EmailTemplates;
using UserManagement.Notification.Models;

namespace UserManagement.Notification.EmailBuilders
{
    public class ForgotPasswordEmailBuilder : EmailBuilder<ResetPasswordEmailModel>
    {

        public ForgotPasswordEmailBuilder(IRazorViewToStringRenderer emailRenderer) : base(emailRenderer)
        {

        }

        public override string CreateEmailSubject(BaseEmailTemplateModel emailTemplateModel)
        {
            Guard.ArgumentNotNull(nameof(emailTemplateModel), emailTemplateModel);

            return "Reset Password";
        }

        public override async Task<string> CreateEmailBodyAsync(BaseEmailTemplateModel emailTemplateModel)
        {
            Guard.ArgumentNotNull(nameof(emailTemplateModel), emailTemplateModel);

            return await EmailRenderer.RenderViewToStringAsync("Views/ForgotPassword.cshtml", emailTemplateModel);
        }
    }
}
