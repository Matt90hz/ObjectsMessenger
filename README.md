# ObjectsMessenger
Utility class to provide objects comunication.

## What it should do?
- [x] Allow comunication between objects.
- [x] It should enforce the existance of a message channel. 
- [ ] Support dependency injection.
- [ ] Support asyncronous comunication. 
- [x] The comunication should be defined in a self contained object. 
- [x] Events should be exposed about the comunication process.
- [x] It should be allowed publication of a message to anyone. 
- [x] The messenging process should not be bound to properties. 
- [x] It should be possible to pick if the message has to be persisted or deleted after delivery. 
- [x] There should be no memory leaks due to the storing of the sender. 
- [ ] Only interfaces should be used to interact with the system. 
 
## How It is done?

### The message
```csharp
//single channel cumunication
public abstract class Message<TSender, TReceiver, T>
{
	protected T? _message;
	private bool _preserve;

	//All message should be singletons
	public static Message<TSender, TReceiver, T> Default { get; } = new();

	//Stores the message for reception, and return the message to chain calls easyly
	//Preserve let you decide if the message has to be kept in memory once received
	public T Send(TSender sender, bool preserve = false) 
	{
		_message = GetMessage(_sender);
		_preserve = preserve;

		return _message;
	}

	//Lets you retrive the message only if in some way you own the receiver enforcing the comunication
	public T Receive(TReceiver receiver) 
	{
		if(_message is null) throw new Exception();
		if(preserve) return _message;

		T message = _message;
		_message = null;

		return message;
	}
	
	//How to get the message is completely encapsulated in the object and no need to implement it inside the sender
	protected abstract T GetMessage(TSender sender);

	public Message(){}
}

//multichannel comunication
public abstract class Message<TSender, T>
{
	protected T? _message;

	public static Message<TSender, T> Default { get; } = new();

	public T Send(TSender sender) 
	{
		_message = GetMessage(_sender);
		return _message;
	}

	public T Receive() => _message;
	
	protected abstract T GetMessage(TSender sender);

	private Message(){}
}

```