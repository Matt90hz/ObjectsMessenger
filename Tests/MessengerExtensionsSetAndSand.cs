using FluentAssertions;
using Tests.Utililties;
using IncaTechnologies.ObjectsMessenger;
using System.Reactive.Linq;

namespace Tests;

public sealed class MessengerExtensionsSetAndSand
{
    [Fact]
    public void ValueOfThePrperty_ShouldBeSetted()
    {
        //arrange
        Guid field1 = Guid.Empty;
        Guid field2 = Guid.Empty;
        Guid newGuid = Guid.NewGuid();
        GuidMessenger messenger = new();
        GuidPublisher publisher = new();
        Sender sender = new();

        //act
        messenger.SetAndSend(sender, ref field1, newGuid);
        publisher.SetAndSend(sender, ref field2, newGuid);

        //assert
        field1.Should().Be(newGuid);
        field2.Should().Be(newGuid);
    }

    [Fact]
    public void ValueGetSet_ShouldBeAlsoSent()
    {
        //arrange
        Guid field = Guid.Empty;
        Guid newGuid = Guid.NewGuid();
        GuidMessenger messenger = new();
        GuidPublisher publisher = new();
        Sender sender = new();

        var messengerEventsMonitor = messenger.Events.ToEvent().Monitor();
        var publisherEventsMonitor = publisher.Events.ToEvent().Monitor();

        //act
        messenger.SetAndSend(sender, ref field, newGuid);
        publisher.SetAndSend(sender, ref field, newGuid);

        //assert
        messengerEventsMonitor
            .Should()
            .Raise(nameof(messengerEventsMonitor.Subject.OnNext))
            .WithArgs<MessengerEvent>(me => me == MessengerEvent.Sended);

        publisherEventsMonitor
            .Should()
            .Raise(nameof(publisherEventsMonitor.Subject.OnNext))
            .WithArgs<MessengerEvent>(me => me == MessengerEvent.Sended);
    }
}