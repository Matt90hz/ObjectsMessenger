using FluentAssertions;
using Tests.Utililties;

namespace Tests;

public sealed class PublisherReceiveTests
{
    [Fact]
    public void WhenMessageIsSent_ShouldReturnTheValueSent()
    {
        //arrange
        Sender sender = new();
        Receiver receiver = new();
        GuidPublisher publisher = new();

        //act
        publisher.Send(sender);
        var value = publisher.Receive();

        //assert
        value.Should().Be(sender.Value);
    }

    [Fact]
    public void WhenMessageIsNotSent_ShouldReturnDefault()
    {
        //arrange
        Receiver receiver = new();
        GuidPublisher publisher = new();

        //act
        var value = publisher.Receive();

        //assert
        value.Should().Be(publisher.Default);
    }

    [Fact]
    public void WhenMessageIsSent_CanBeReceivedMultipleTimes()
    {
        //arrange
        Sender sender = new();
        GuidPublisher publisher = new();

        //act
        publisher.Send(sender);
        var value1 = publisher.Receive();
        var value2 = publisher.Receive();

        //assert
        value1.Should().Be(sender.Value);
        value2.Should().Be(sender.Value);
    }
}