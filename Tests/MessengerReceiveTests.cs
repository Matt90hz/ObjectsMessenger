using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Tests.Utililties;

namespace Tests;

public sealed class MessengerReceiveTests
{
    [Fact]
    public void WhenMessageIsSent_ShouldReturnTheValueSent()
    {
        //arrange
        Sender sender = new();
        Receiver receiver = new();
        GuidMessenger messenger = new();

        //act
        messenger.Send(sender);
        messenger.Receive(receiver);

        //assert
        receiver.Value.Should().Be(sender.Value);
    }

    [Fact]
    public void WhenMessageIsNotSent_ShouldDoNothing()
    {
        //arrange
        Receiver receiver = new();
        GuidMessenger messenger = new();

        //act
        messenger.Receive(receiver);

        //assert
        receiver.Value.Should().Be(Guid.Empty);
    }

    [Fact]
    public void WhenMessageIsAreadyReceived_ShouldDoNothing()
    {
        //arrange
        Sender sender = new();
        Receiver receiver = new();
        GuidMessenger messenger = new();

        //act
        messenger.Send(sender);
        messenger.Receive(receiver);

        receiver.Value = Guid.Empty;
        messenger.Receive(receiver);

        //assert
        receiver.Value.Should().Be(Guid.Empty);
    }
}
