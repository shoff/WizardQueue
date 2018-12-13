using System.Threading.Tasks;

namespace Wizard.Shared
{
    public interface IEventMessageEntity
    {
        Task SendEventCreationMessage(IEvent model);
    }
}