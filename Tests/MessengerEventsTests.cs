using FluentAssertions;
using IncaTechnologies.ObjectsMessenger;
using System.Reactive;
using System.Reactive.Linq;
using Tests.Utililties;

namespace Tests;

public sealed class MessengerEventsTests
{
    [Fact]
    public void WhenMessageIsReceived_ShouldEmitReceiving()
    {
        //arrange
        Receiver receiver = new();
        GuidMessenger messenger = new();
        using var monitor = messenger.Events.ToEvent().Monitor();

        //act
        messenger.Receive(receiver);

        //assert
        monitor
            .Should()
            .Raise(nameof(IEventSource<Guid>.OnNext))
            .WithArgs<MessengerEvent>(ev => ev == MessengerEvent.Receiving);
    }

    [Fact]
    public void WhenMessageIsReceived_ShouldEmitReceived()
    {
        //arrange
        Sender sender = new();
        Receiver receiver = new();
        GuidMessenger messenger = new();
        using var monitor = messenger.Events.ToEvent().Monitor();

        //act
        messenger.Send(sender);
        messenger.Receive(receiver);

        //assert
        monitor
            .Should()
            .Raise(nameof(IEventSource<Guid>.OnNext))
            .WithArgs<MessengerEvent>(ev => ev == MessengerEvent.Received);
    }

    [Fact]
    public void WhenMessageIsReceived_ShouldEmitReceivingAndThenReceived()
    {
        //arrange
        Sender sender = new();
        Receiver receiver = new();
        GuidMessenger messenger = new();
        List<MessengerEvent> messengerEvents = [];

        messenger.Events.Skip(2).Subscribe(ev => messengerEvents.Add(ev));

        //act
        messenger.Send(sender);
        messenger.Receive(receiver);

        //assert
        messengerEvents.Should().Equal([MessengerEvent.Receiving, MessengerEvent.Received]);
    }

    [Fact]
    public void WhenMessageIsNotSent_ShouldEmitReceivedFailed()
    {
        //arrange
        Receiver receiver = new();
        GuidMessenger messenger = new();
        using var monitor = messenger.Events.ToEvent().Monitor();

        //act
        messenger.Receive(receiver);

        //assert
        monitor
            .Should()
            .Raise(nameof(IEventSource<Guid>.OnNext))
            .WithArgs<MessengerEvent>(ev => ev == MessengerEvent.ReceiveFailed);
    }

    [Fact]
    public void WhenMessageAreadyReceived_ShouldEmitReceivedFailed()
    {
        //arrange
        Receiver receiver = new();
        GuidMessenger messenger = new();
        using var monitor = messenger.Events.ToEvent().Monitor();

        //act
        messenger.Receive(receiver);
        messenger.Receive(receiver);

        //assert
        monitor
            .Should()
            .Raise(nameof(IEventSource<Guid>.OnNext))
            .WithArgs<MessengerEvent>(ev => ev == MessengerEvent.ReceiveFailed);
    }

    [Fact]
    public void WhenMessageSent_ShouldEmitSended()
    {
        //arrange
        Sender sender = new();
        GuidMessenger messenger = new();
        using var monitor = messenger.Events.ToEvent().Monitor();

        //act
        messenger.Send(sender);

        //assert
        monitor
            .Should()
            .Raise(nameof(IEventSource<Guid>.OnNext))
            .WithArgs<MessengerEvent>(ev => ev == MessengerEvent.Sended);
    }

    [Fact]
    public void WhenMessageSent_ShouldEmitSending()
    {
        //arrange
        Sender sender = new();
        GuidMessenger messenger = new();
        using var monitor = messenger.Events.ToEvent().Monitor();

        //act
        messenger.Send(sender);

        //assert
        monitor
            .Should()
            .Raise(nameof(IEventSource<Guid>.OnNext))
            .WithArgs<MessengerEvent>(ev => ev == MessengerEvent.Sending);
    }

    [Fact]
    public void WhenMessageIsSended_ShouldEmitSendingAndThenSended()
    {
        //arrange
        Sender sender = new();
        GuidMessenger messenger = new();
        List<MessengerEvent> messengerEvents = [];

        messenger.Events.Take(2).Subscribe(ev => messengerEvents.Add(ev));

        //act
        messenger.Send(sender);

        //assert
        messengerEvents.Should().Equal([MessengerEvent.Sending, MessengerEvent.Sended]);
    }
}
