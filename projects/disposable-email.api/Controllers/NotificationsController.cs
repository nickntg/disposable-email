using System.Threading.Tasks;
using disposable_email.api.Models;
using disposable_email.api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace disposable_email.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationsController(ILogger<NotificationsController> logger, IMailStorage mailStorage) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AcceptReceivedMail([FromBody] NotificationRequest request)
        {
            logger.LogDebug("Mail notification, from {EnvelopeMailFrom}", request.Envelope.MailFrom);

            await mailStorage.SaveAsync(request);
            
            return Ok();
        }
    }
}
