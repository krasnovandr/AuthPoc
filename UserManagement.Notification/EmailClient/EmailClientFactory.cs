using SendGrid;
using UserManagement.Common;

namespace UserManagement.Notification.EmailClient
{
    public class EmailClientFactory : IEmailClientFactory
    {
        public ISendGridClient CreateClient(string apiKey)
        {
            Guard.ArgumentNotNull(nameof(apiKey), apiKey);

            return new SendGridClient(apiKey);
        }
    }
}
