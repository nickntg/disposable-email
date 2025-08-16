using System.Threading.Tasks;
using disposable_email.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace disposable_email.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationsController(ILogger<NotificationsController> logger) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AcceptReceivedMail([FromBody] NotificationRequest request)
        {
            var decoded = await request.Decode();
            return Ok();
        }
    }
}
