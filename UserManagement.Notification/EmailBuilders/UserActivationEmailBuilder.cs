using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using UserManagement.Common;
using UserManagement.Notification.EmailTemplates;
using UserManagement.Notification.Models;

namespace UserManagement.Notification.EmailBuilders
{
    public class UserActivationEmailBuilder : EmailBuilder<UserActivationEmailModel>
    {

        public UserActivationEmailBuilder(IRazorViewToStringRenderer emailRenderer) : base(emailRenderer)
        {

        }

        public override string CreateEmailSubject(BaseEmailTemplateModel emailTemplateModel)
        {
            Guard.ArgumentNotNull(nameof(emailTemplateModel), emailTemplateModel);

            return "Account Activation";
        }

        public override async Task<string> CreateEmailBodyAsync(BaseEmailTemplateModel emailTemplateModel)
        {
            Guard.ArgumentNotNull(nameof(emailTemplateModel), emailTemplateModel);

            return await EmailRenderer.RenderViewToStringAsync("Views/UserActivation.cshtml", emailTemplateModel);
        }
    }
}
