using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Wizard.Shared;
using Wizard.Shared.Messages;
using Wizard.Shared.Models;

namespace WizardQueue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IMediator mediator;
        public EventController(
           IMediator mediator,
           IEventMessageEntity eventMessageEntity)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]EventModel eventModel)
        {
            var message = new EventMessage
            {
                EventDescription = eventModel.EventDescription,
                EventName = eventModel.EventName
            };
            await this.mediator.Send(message);
            return Ok();
        }
    }
}