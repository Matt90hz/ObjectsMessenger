using FluentAssertions;
using IncaTechnologies.ObjectsMessenger;
using System.Reactive.Linq;
using Tests.Utililties;

namespace Tests;

public sealed class MessengerHubMessengersErrorsTests
{
    [Fact]
    public void WhenMessengersAreSubscribed_ErrorsShouldBeReemittedByMesssengerHub()
    {
        //arrange
        Sender sender = new();
        Receiver receiver = new();
        GuidMessenger messenger1 = new();
        GuidMessenger messenger2 = new();
        MessengerHub messengerHub = (MessengerHub)Activator.CreateInstance(typeof(MessengerHub), true)!;

        messengerHub.RegisterMessenger(messenger1);
        messengerHub.RegisterMessenger(messenger2);

        List<(Messenger, Exception)> errorsEmittedByMessengers = [];
        List<(Messenger, Exception)> errorsEmittedByHub = [];

        messengerHub.MessengersErrors.Subscribe(ex => errorsEmittedByHub.Add(ex));
        messenger1.Errors.Subscribe(ex => errorsEmittedByMessengers.Add((messenger1, ex)));
        messenger2.Errors.Subscribe(ex => errorsEmittedByMessengers.Add((messenger2, ex)));

        //act
        messenger1.Receive(receiver);
        messenger2.Receive(receiver);
        messenger1.Send(sender);
        messenger2.Send(sender);
        messenger1.Receive(receiver);
        messenger1.Receive(receiver);
        messenger2.Receive(receiver);
        messenger2.Receive(receiver);

        //assert
        errorsEmittedByHub.Should().Equal(errorsEmittedByMessengers);
    }
}
