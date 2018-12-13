using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Wizard.Shared.Messages;

namespace Wizard.Shared.Handlers
{
    public class EventMessageHandler : IRequestHandler<EventMessage, bool>
    {
        private readonly IEventMessageEntity eventMessageEntity;

        public EventMessageHandler(IEventMessageEntity eventMessageEntity)
        {
            this.eventMessageEntity = eventMessageEntity;
        }

        public async Task<bool> Handle(EventMessage request, CancellationToken cancellationToken)
        {
            await this.eventMessageEntity.SendEventCreationMessage(request);
            return true;
        }
    }
}