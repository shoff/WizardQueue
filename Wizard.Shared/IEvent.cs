using System;

namespace Wizard.Shared
{
    public interface IEvent
    {
        Guid? EventModelId { get; set; }
        string EventName { get; set; }
        string EventDescription { get; set; }
    }
}