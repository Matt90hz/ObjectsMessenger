using FluentAssertions;
using Tests.Utililties;

namespace Tests;

public sealed class MessengerIsMessageReceivedTests
{
    [Fact]
    public void WhenMessageIsReceived_ShouldBeTrue()
    {
        //arrange
        Sender sender = new();
        Receiver receiver = new();
        GuidMessenger messenger = new();

        //act
        messenger.Send(sender);
        messenger.Receive(receiver);

        //assert
        messenger.IsMessageReceived.Should().BeTrue();
    }

    [Fact]
    public void WhenMessageIsNotReceivedYet_ShouldBeFalse()
    {
        //arrange
        Sender sender = new();
        GuidMessenger messenger = new();

        //act
        messenger.Send(sender);

        //assert
        messenger.IsMessageReceived.Should().BeFalse();
    }

    [Fact]
    public void WhenMessageIsReceivedAndThenResent_ShouldBeFalse()
    {
        //arrange
        Sender sender = new();
        Receiver receiver = new();
        GuidMessenger messenger = new();

        //act
        messenger.Send(sender);
        messenger.Receive(receiver);
        messenger.Send(sender);

        //assert
        messenger.IsMessageReceived.Should().BeFalse();
    }
}
