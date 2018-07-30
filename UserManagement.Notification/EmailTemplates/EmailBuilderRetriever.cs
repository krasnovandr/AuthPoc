using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SendGrid.Helpers.Mail;
using UserManagement.Common;
using UserManagement.Notification.EmailBuilders;
using UserManagement.Notification.Models;

namespace UserManagement.Notification.EmailTemplates
{
    public class EmailBuilderRetriever : IEmailBuilderRetriever
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly EmailServerConnectionSettings _serverConnectionSettings;

        public EmailBuilderRetriever(
            IServiceProvider serviceProvider,
            EmailServerConnectionSettings serverConnectionSettings)
        {
            Guard.ArgumentNotNull(nameof(serverConnectionSettings), serverConnectionSettings);
            _serverConnectionSettings = serverConnectionSettings;

            Guard.ArgumentNotNull(nameof(serviceProvider), serviceProvider);
            _serviceProvider = serviceProvider;
        }

        public async Task<SendGridMessage> GetMessage(BaseEmailTemplateModel baseEmailTemplateModel)
        {
            Guard.ArgumentNotNull(nameof(baseEmailTemplateModel), baseEmailTemplateModel);
            var builderType = typeof(EmailBuilder<>).MakeGenericType(baseEmailTemplateModel.GetType());
            var builder = _serviceProvider.GetRequiredService(builderType);
            var result = await ((dynamic)builder).BuildAsync((dynamic)_serverConnectionSettings, (dynamic)baseEmailTemplateModel);
            return result;
        }
    }
}
