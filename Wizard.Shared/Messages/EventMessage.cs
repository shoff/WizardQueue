using System;
using MediatR;

namespace Wizard.Shared.Messages
{
    // [Queue("EventCreationQueue", ExchangeName = "EventCreationExchange")]
    public class EventMessage : IEvent, IRequest<bool>
    {
        public EventMessage()
        {
            this.EventModelId = Guid.NewGuid();
        }

        public EventMessage(string id)
        {
            this.EventModelId = Guid.Parse(id);
        }

        public EventMessage(Guid id)
        {
            this.EventModelId = id;
        }

        public Guid? EventModelId { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
    }
}