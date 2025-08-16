using System.Collections.Generic;
using System.Threading.Tasks;
using disposable_email.api.Models;

namespace disposable_email.api.Services.Interfaces
{
    public interface IMailStorage
    {
        Task<NotificationRequest> SaveAsync(NotificationRequest request);
        Task RemoveAsync(string id);
        Task<NotificationRequest> GetByIdAsync(string id);
        Task<IList<NotificationRequest>> GetByRecipientAsync(string email);
    }
}
