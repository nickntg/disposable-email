using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using disposable_email.api.Models;
using disposable_email.api.Services.Interfaces;

namespace disposable_email.api.Services
{
    public class InMemoryMailStorage : IMailStorage
    {
        private readonly ConcurrentDictionary<string, IList<NotificationRequest>> _mapByRecipient = new();
        private readonly ConcurrentDictionary<string, NotificationRequest> _mapById = new();

        public Task<NotificationRequest> SaveAsync(NotificationRequest request)
        {
            if (request is null)
            {
                throw new ArgumentException("Request is null");
            }

            request.Id = Guid.NewGuid().ToString("N");

            _mapById.TryAdd(request.Id, request);

            var mailTo = request.Headers.To?.First();

            if (mailTo is null)
            {
                throw new ArgumentException("To is null");
            }

            _mapByRecipient.TryAdd(mailTo, []);
            _mapByRecipient[mailTo].Add(request);

            return Task.FromResult(request);
        }

        public Task RemoveAsync(string id)
        {
            if (_mapById.Remove(id, out var request))
            {
                var mailTo = request.Headers.To?.First();

                _mapByRecipient.Remove(mailTo, out _);
            }

            return Task.CompletedTask;
        }

        public Task<NotificationRequest> GetByIdAsync(string id)
        {
            return _mapById.TryGetValue(id, out var request) ? Task.FromResult(request) : null;
        }

        public Task<IList<NotificationRequest>> GetByRecipientAsync(string email)
        {
            return _mapByRecipient.TryGetValue(email, out var lst) ? Task.FromResult(lst) : Task.FromResult((IList<NotificationRequest>)[]);
        }
    }
}
