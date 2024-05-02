# ObjectsMessenger
Utility library to provide objects communication.

## What it should do?
- [x] Allow communication between objects.
- [x] Enforce the existence of a message channel.
- [x] Ensure that only senders can send messages, and only receivers can receive them.
- [x] Support dependency injection.
- [ ] Support asyncronous communication. 
- [x] Define communication in a self-contained object.
- [x] Expose events related to the communication process.
- [x] Optionally deliver messages to anyone. 
- [x] Not bind the messaging process to properties.
- [x] Provide an option to persist or delete messages after delivery.
- [x] Not cause memory leaks due to sender storage.
- [ ] Only interfaces should be used to interact with the system. 
- [x] Include a utility for dealing with property accessors.
- [x] Not rely on exceptions.
- [x] Rely on observables instead of events.
 
## How it is done?
Implement `Messenger<TSender, TReceiver, TMessage>` to create a single-channel communication between two objects. Otherwise, implement `Messenger<TSender, TReceiver>` to establish multichannel communication, where one object can dispatch messages to anyone.

Optionally, register the messengers in the `MessengerHub` to handle all messenger events and errors.

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

Let's assume that you want to deliver the current user from `UsersViewModel` to `EditUserViewModel`. Then, implement a single-channel messenger like:
```csharp
public sealed class CurrentUserMessenger : Messenger<UsersViewModel, EditUserViewModel, User?>
{
    public override bool IsMessagePreserved => false;

    protected override void ReceiveMessage(EditUserViewModel receiver, User? message) => receiver.User = message;

    protected override User? SendMessage(UsersViewModel sender) => sender.CurrentUser; 
}
```

Refactor `UsersViewModel` and `EditUserViewModel` to send and receive the message as appropriate. What follows is just an example of how the communication process might be arranged.
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
### Implement a multichannel messenger
Implementing a multi-channel `Messenger` does not differ much from the single channel. First, implement `Messenger<TSender, TMessage>`.
```csharp
public sealed class CurrentUSerPublisher : Messenger<UsersViewModel, User?>
{
    public override User? Default => default;

    protected override User? SendMessage(UsersViewModel sender) => sender.CurrentUser;
}
```

The sending process remains the same, whereas the receiving process lets you get the message directly. Like so:
```csharp
public sealed class EditUserViewModel
{
    public User? User { get; set; }

    public EditUserViewModel(CurrentUserMessenger currentUserMessenger)
    {
        User = currentUserMessenger.Receive();
    }
}
```

### Register the messenger
It is possible to register any `Messenger` into the `MessengerHub`. This will allow handling the messenger's events in a single place.

```csharp
MessengerHub.Default.RegisterMessenger(currentUserMessenger);
```

Doing this will allow the creation of behaviors. For example, the `EditUserViewModel` from before can be refactored to better separate concerns.

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
The use of `IObservable` instead of CLR events makes the system very flexible. For more information, check [System.Reactive](https://github.com/dotnet/reactive).

## Utilities

### Dependency injection
```csharp
services.RegisterMessengers(Assembly.GetExecutingAssembly());
```
This call not only registers all `Messenger` instances in the given assembly, but also registers all the `Messenger` instances in the `MessengerHub`.

### Property setter
The `SetAndSend(...)` function can be used to simplify the sending process for a property.

```csharp
public sealed class UsersViewModel
{
    private readonly CurrentUserMessenger _currentUserMessenger;

    private User? _currentUser;

    public User? CurrentUser
    {
        get => _currentUser;
        set => _currentUserMessenger.SetAndSend(this, ref _currentUser, value);
    }

    public IEnumerable<User> Users { get; }

    public UsersViewModel(...) {...}
}
```

## Contribute
Do you like this library, and do you want to make it better? Just open an issue on [GitHub](https://github.com/Matt90hz/ObjectsMessenger).