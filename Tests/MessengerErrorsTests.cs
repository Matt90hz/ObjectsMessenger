using FluentAssertions;
using IncaTechnologies.ObjectsMessenger.Exceptions;
using Tests.Utililties;

namespace Tests;

public sealed class MessengerErrorsTests
{
    [Fact]
    public void WhenReceivedIsCalledBeforeSend_ShouldEmitMessageNeverSentException()
    {
        //arrange
        Receiver receiver = new();
        GuidMessenger messenger = new();
        Exception? exception = null;
        
        messenger.Errors.Subscribe(ex => exception = ex);

        //act
        messenger.Receive(receiver);

        //assert
        exception.Should().BeAssignableTo(typeof(MessageNeverSentException));
    }

    [Fact]
    public void WhenReceivedIsCalledTwiceOnUnpresevedMessage_ShouldEmitMessageAlreadyReceivedException()
    {
        //arrange
        Sender sender = new();
        Receiver receiver = new();
        GuidMessenger messenger = new();
        Exception? exception = null;

        messenger.Errors.Subscribe(ex => exception = ex);

        //act
        messenger.Send(sender);
        messenger.Receive(receiver);
        messenger.Receive(receiver);

        //assert
        exception.Should().BeAssignableTo(typeof(MessageAlreadyReceivedException));
    }
}
