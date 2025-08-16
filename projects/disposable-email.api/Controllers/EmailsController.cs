using System.Collections.Generic;
using disposable_email.api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using disposable_email.api.Models;

namespace disposable_email.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailsController(IMailStorage mailStorage) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string email)
        {
            var emails = await mailStorage.GetByRecipientAsync(email);

            var lst = new List<ParsedEmail>();
            foreach (var e in emails)
            {
                lst.Add(await e.DecodeAsync());
            }

            lst.Reverse();

            return Ok(lst);
        }
    }
}
