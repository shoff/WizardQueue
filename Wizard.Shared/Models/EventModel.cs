using System;

namespace Wizard.Shared.Models
{
    public class EventModel : IEvent
    {
        public Guid? EventModelId { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
    }
}