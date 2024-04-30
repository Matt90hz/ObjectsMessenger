using FluentAssertions;
using IncaTechnologies.ObjectsMessenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tests.Utililties;

namespace Tests;
public sealed class MessengerHubMessengersEventsTests
{
    [Fact]
    public void WhenMessengersAreSubscribed_EventsShouldBeReemittedByMesssengerHub()
    {
        //arrange
        Sender sender = new();
        Receiver receiver = new();
        GuidMessenger messenger1 = new();
        GuidMessenger messenger2 = new();
        MessengerHub messengerHub = (MessengerHub)Activator.CreateInstance(typeof(MessengerHub), true)!;

        messengerHub.RegisterMessenger(messenger1);
        messengerHub.RegisterMessenger(messenger2);

        List<(Messenger, MessengerEvent)> eventEmittedByMessengers = [];
        List<(Messenger, MessengerEvent)> eventEmittedByHub = [];

        messengerHub.MessengersEvents.Subscribe(ev => eventEmittedByHub.Add(ev));
        messenger1.Events.Subscribe(ev => eventEmittedByMessengers.Add((messenger1, ev)));
        messenger2.Events.Subscribe(ev => eventEmittedByMessengers.Add((messenger2, ev)));

        //act
        messenger1.Send(sender);
        messenger2.Send(sender);
        messenger1.Receive(receiver);
        messenger2.Receive(receiver);
        messenger2.Send(sender);
        messenger2.Receive(receiver);
        messenger1.Send(sender);
        messenger1.Receive(receiver);

        //assert
        eventEmittedByHub.Should().Equal(eventEmittedByMessengers);
    }
}