using FluentAssertions;
using IncaTechnologies.ObjectsMessenger.Exceptions;
using Tests.Utililties;

namespace Tests;

public sealed class PublisherErrorsTests
{
    [Fact]
    public void WhenReceivedIsCalledBeforeSend_ShouldEmitMessageNeverSentException()
    {
        //arrange
        Receiver receiver = new();
        GuidPublisher publisher = new();
        Exception? exception = null;

        publisher.Errors.Subscribe(ex => exception = ex);

        //act
        _ = publisher.Receive();

        //assert
        exception.Should().BeAssignableTo(typeof(MessageNeverSentException));
    }
}