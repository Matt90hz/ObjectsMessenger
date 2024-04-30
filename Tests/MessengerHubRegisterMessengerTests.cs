using FluentAssertions;
using IncaTechnologies.ObjectsMessenger;
using System.Reactive.Linq;
using Tests.Utililties;

namespace Tests;

public sealed class MessengerHubRegisterMessengerTests
{
    [Fact]
    public void WhenMessengerIsRegistered_MessengerHubEventAndErrorsShouldBeEmitted()
    {
        //arrange
        Sender sender = new();
        Receiver receiver = new();
        GuidMessenger messenger = new();
        MessengerHub messengerHub = (MessengerHub)Activator.CreateInstance(typeof(MessengerHub), true)!;

        messengerHub.RegisterMessenger(messenger);

        using var monitorEvents = messengerHub.MessengersEvents.ToEvent().Monitor();
        using var monitorErrors = messengerHub.MessengersErrors.ToEvent().Monitor();

        //act
        messenger.Send(sender);
        messenger.Receive(receiver);
        messenger.Receive(receiver);

        //assert
        monitorEvents.Should().Raise(nameof(monitorEvents.Subject.OnNext));
        monitorErrors.Should().Raise(nameof(monitorEvents.Subject.OnNext));
    }
}