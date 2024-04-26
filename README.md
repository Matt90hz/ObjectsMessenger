# ObjectsMessenger
Utility class to provide objects comunication.

## What it should do?
- [x] Allow comunication between objects.
- [x] It should enforce the existance of a message channel.
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
 
## How It is done?
