using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SimpleEmailApi.Mediatr;

using System.Net;

namespace SimpleEmailApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IMediator mediatR;

        public EmailController(IMediator mediatR)
        {:""
            this.mediatR = mediatR;
        }
        [HttpPost]
        [ProducesResponseType(typeof(EmailSend.Response),(int) HttpStatusCode.OK)]
        public async Task<IActionResult> Sendasync([FromBody]EmailSend.Request request)
        {
            try
            {
                var response = await mediatR.Send(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message); //Real world, log and return generic messages
            }

        }
    }
}
