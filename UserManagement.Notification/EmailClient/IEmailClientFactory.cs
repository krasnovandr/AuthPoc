using SendGrid;

namespace UserManagement.Notification.EmailClient
{
    public interface IEmailClientFactory
    {
        ISendGridClient CreateClient(string apiKey);
    }
}
