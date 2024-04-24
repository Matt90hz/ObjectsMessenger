using IncaTechnologies.ObjectsMessenger;
using System.Reactive.Linq;

namespace Example;

public sealed class EditUserViewModel
{
    public User? User { get; set; }

    public EditUserViewModel(CurrentUserMessenger currentUserMessenger)
    {
        currentUserMessenger.Events
            .Where(@event => @event is MessengerEvent.Sended)           
            .Subscribe(_ => currentUserMessenger.Receive(this));
    }
}