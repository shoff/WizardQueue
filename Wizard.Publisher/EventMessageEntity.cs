using System.Threading.Tasks;
using EasyNetQ;
using Wizard.Shared;

namespace Wizard.Publisher
{
    public class EventMessageEntity : IEventMessageEntity
    {
        public async Task SendEventCreationMessage(IEvent model)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                await bus.PublishAsync(model);
            }
        }
    }
}