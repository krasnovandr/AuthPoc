using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using UserManagement.Notification.Models;

namespace UserManagement.Notification.EmailTemplates
{
    public interface IEmailBuilderRetriever
    {
        Task<SendGridMessage> GetMessage(BaseEmailTemplateModel baseEmailTemplateModel);
    }
}
