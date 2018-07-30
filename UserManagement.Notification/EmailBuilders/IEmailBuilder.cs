using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using UserManagement.Notification.Models;

namespace UserManagement.Notification.EmailTemplates
{
    public interface IEmailBuilder<in TEmailModel>
        where TEmailModel : BaseEmailTemplateModel
    {
        Task<SendGridMessage> BuildAsync(EmailServerConnectionSettings connectionSettings, TEmailModel emailTemplateModel);
    }
}
