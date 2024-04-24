using FluentAssertions;
using IncaTechnologies.ObjectsMessenger;
using Tests.Utililties;

namespace Tests;

public class MessengerEventsTests
{
    [Fact]
    public void WhenMessageIsSent_ShouldEmitReceived()
    {
        //arrange
        Sender sender = new();
        Receiver receiver = new();
        GuidMessenger messenger = new();

        var messengerEvent = MessengerEvent.ReceiveFailed;
        messenger.Events.Subscribe(ev => messengerEvent = ev);

        //act
        messenger.Send(sender);
        messenger.Receive(receiver);

        //assert
        messengerEvent.Should().Be(MessengerEvent.Received);
    }

    [Fact]
    public void WhenMessageIsNotSent_ShouldEmitReceivedFailed()
    {
        //arrange
        Receiver receiver = new();
        GuidMessenger messenger = new();

        var messengerEvent = MessengerEvent.Received;
        messenger.Events.Subscribe(ev => messengerEvent = ev);

        //act
        messenger.Receive(receiver);

        //assert
        messengerEvent.Should().Be(MessengerEvent.ReceiveFailed);
    }

    [Fact]
    public void WhenMessageAreadyReceived_ShouldEmitReceivedFailed()
    {
        //arrange
        Receiver receiver = new();
        GuidMessenger messenger = new();

        var messengerEvent = MessengerEvent.Received;
        messenger.Events.Subscribe(ev => messengerEvent = ev);

        //act
        messenger.Receive(receiver);
        messenger.Receive(receiver);

        //assert
        messengerEvent.Should().Be(MessengerEvent.ReceiveFailed);
    }
}
