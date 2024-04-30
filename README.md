# ObjectsMessenger
Utility class to provide objects comunication.

## What it should do?
- [x] Allow comunication between objects.
- [x] It should enforce the existence of a message channel.
- [x] Should eforce that only senders can send and only receivers can receive.
- [x] Support dependency injection.
- [ ] Support asyncronous comunication. 
- [x] The comunication should be defined in a self contained object. 
- [x] Events should be exposed about the comunication process.
- [x] It should be allowed publication of a message to anyone. 
- [x] The messenging process should not be bound to properties. 
- [x] It should be possible to pick if the message has to be persisted or deleted after delivery. 
- [x] There should be no memory leaks due to the storing of the sender. 
- [ ] Only interfaces should be used to interact with the system. 
- [x] Should be present a utility to deal with property accessors.
- [x] The system should not rely on exceptions.
- [x] Rely on observables instead of events.
 
## How it is done?
Implement `Messenger<TSender, TReceiver, TMessage>` to create a single channel comunication between two objects. Otherwise implement `Messenger<TSender, TReceiver>` to establish a multichannel comunication, where one object can dispatch messages to enyone.

Optionally, register the messengers in the `MessengerHub` to handle all the messengers events and errors.

### Implement a single channel messenger
Given a sender class like:
```csharp
public sealed class UsersViewModel
{
    private User? _currentUser;

    public User? CurrentUser
    {
        get => _currentUser;
        set => _currentUser = value;
    }

    public IEnumerable<User> Users { get; }

    public UsersViewModel(...) {...}
}
```

And a receiver class like:
```csharp
public sealed class EditUserViewModel
{
    public User? User { get; set; }
}
```

Let assume that you want to deliver the current user from `UsersViewModel` to `EditUserViewModel`. Than implement a single channel messenger like:
```csharp
public sealed class CurrentUserMessenger : Messenger<UsersViewModel, EditUserViewModel, User?>
{
    public override bool IsMessagePreserved => false;

    protected override void ReceiveMessage(EditUserViewModel receiver, User? message) => receiver.User = message;

    protected override User? SendMessage(UsersViewModel sender) => sender.CurrentUser; 
}
```

Refactor `UsersViewModel` and `EditUserViewModel` to send and receive the message as appropriate. What follows is just an example of how the cumunication process might be arranged.
```csharp
public sealed class UsersViewModel
{
    private readonly CurrentUserMessenger _currentUserMessenger;

    private User? _currentUser;

    public User? CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            _currentUserMessenger.Send(this);
        }
    }

    public IEnumerable<User> Users { get; }

    public UsersViewModel(...) {...}
}

public sealed class EditUserViewModel
{
    public User? User { get; set; }

    public EditUserViewModel(CurrentUserMessenger currentUserMessenger)
    {
        currentUserMessenger.Receive(this);
    }
}
```

### Register the messenger
It is possible to register any `Messenger` into the `MessengerHub` this will allow the handle of the messengers events in a single place.

```csharp
MessengerHub.Default.RegisterMessenger(currentUserMessenger);
```

Doing this will let the creation of behaviours. For example the `EditUserViewModel` from before can be refactored to better separate concenrs.

```csharp
public sealed class ReceiveCurrentUserBehavior
{
    public ReceiveCurrentUserBehavior(CurrentUserMessenger currentUserMessenger, EditUserViewModel editUserViewModel)
    {
        currentUserMessenger.Events
            .Where(@event => @event is MessengerEvent.Sended)           
            .Subscribe(_ => currentUserMessenger.Receive(editUserViewModel)); 
    }
}
```

### Notes
The use of `IObservable` instead of the CLR events makes the system very flexible. For more information check [System.Reactive](https://github.com/dotnet/reactive).

## Utilities

### Dependency injection
```csharp
services.RegisterMessengers(Assembly.GetExecutingAssembly());
```
This call not only will register all Messengers in the given assembly