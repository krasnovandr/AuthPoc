namespace UserManagement.Notification.Models
{
    public class UserActivationEmailModel : BaseEmailTemplateModel
    {
        public string UserName{ get; set; }
        public object ConfirmationLink { get; set; }
    }
}
