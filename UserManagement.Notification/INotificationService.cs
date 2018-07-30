using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Notification.Models;

namespace UserManagement.Notification
{
    public interface INotificationService
    {
        Task NotifyAsync(BaseEmailTemplateModel baseEmailTemplateModel);
    }
}
