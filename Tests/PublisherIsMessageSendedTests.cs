using FluentAssertions;
using Tests.Utililties;

namespace Tests;

public sealed class PublisherIsMessageSendedTests
{
    [Fact]
    public void WhenMessageIsSent_SholudBeTrue()
    {
        //arrange
        Sender sender = new();
        GuidPublisher publisher = new();

        //act
        publisher.Send(sender);

        //assert
        publisher.IsMessageSended.Should().BeTrue();
    }

    [Fact]
    public void WhenMessageIsNotSent_SholudBeFalse()
    {
        //arrange
        GuidPublisher publisher = new();

        //act

        //assert
        publisher.IsMessageSended.Should().BeFalse();
    }
}