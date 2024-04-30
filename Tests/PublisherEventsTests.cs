using FluentAssertions;
using IncaTechnologies.ObjectsMessenger;
using System.Reactive;
using System.Reactive.Linq;
using Tests.Utililties;

namespace Tests;

public sealed class PublisherEventsTests
{
    [Fact]
    public void WhenMessageIsReceived_ShouldEmitReceiving()
    {
        //arrange
        GuidPublisher publisher = new();
        using var monitor = publisher.Events.ToEvent().Monitor();

        //act
        publisher.Receive();

        //assert
        monitor
            .Should()
            .Raise(nameof(monitor.Subject.OnNext))
            .WithArgs<MessengerEvent>(ev => ev == MessengerEvent.Receiving);
    }

    [Fact]
    public void WhenMessageIsReceived_ShouldNotEmitReceived()
    {
        //arrange
        Sender sender = new();
        GuidPublisher publisher = new();
        using var monitor = publisher.Events.ToEvent().Monitor();

        //act
        publisher.Send(sender);
        publisher.Receive();

        //assert
        monitor.OccurredEvents
            .SelectMany(occuredEvent => occuredEvent.Parameters)
            .Cast<MessengerEvent>()
            .Should()
            .NotContain(MessengerEvent.Received);
    }

    [Fact]
    public void WhenMessageIsNotSent_ShouldEmitReceivedFailed()
    {
        //arrange
        Receiver receiver = new();
        GuidPublisher publisher = new();
        using var monitor = publisher.Events.ToEvent().Monitor();

        //act
        publisher.Receive();

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
        GuidPublisher publisher = new();
        using var monitor = publisher.Events.ToEvent().Monitor();

        //act
        publisher.Send(sender);

        //assert
        monitor
            .Should()
            .Raise(nameof(monitor.Subject.OnNext))
            .WithArgs<MessengerEvent>(ev => ev == MessengerEvent.Sended);
    }

    [Fact]
    public void WhenMessageSent_ShouldEmitSending()
    {
        //arrange
        Sender sender = new();
        GuidPublisher publisher = new();
        using var monitor = publisher.Events.ToEvent().Monitor();

        //act
        publisher.Send(sender);

        //assert
        monitor
            .Should()
            .Raise(nameof(monitor.Subject.OnNext))
            .WithArgs<MessengerEvent>(ev => ev == MessengerEvent.Sending);
    }

    [Fact]
    public void WhenMessageIsSended_ShouldEmitSendingAndThenSended()
    {
        //arrange
        Sender sender = new();
        GuidPublisher publisher = new();
        List<MessengerEvent> messengerEvents = [];

        publisher.Events.Take(2).Subscribe(ev => messengerEvents.Add(ev));

        //act
        publisher.Send(sender);

        //assert
        messengerEvents.Should().Equal([MessengerEvent.Sending, MessengerEvent.Sended]);
    }
}