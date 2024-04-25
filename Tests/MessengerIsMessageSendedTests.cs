using FluentAssertions;
using Tests.Utililties;

namespace Tests;

public sealed class MessengerIsMessageSendedTests
{
    [Fact]
    public void WhenMessageIsSent_SholudBeTrue()
    {
        //arrange
        Sender sender= new();
        GuidMessenger messenger = new();

        //act
        messenger.Send(sender);

        //assert
        messenger.IsMessageSended.Should().BeTrue();
    }

    [Fact]
    public void WhenMessageIsNotSent_SholudBeFalse()
    {
        //arrange
        GuidMessenger messenger = new();

        //act

        //assert
        messenger.IsMessageSended.Should().BeFalse();
    }
}