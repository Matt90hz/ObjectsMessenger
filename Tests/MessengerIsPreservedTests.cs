using FluentAssertions;
using System.Reflection;
using Tests.Utililties;

namespace Tests;

public sealed class MessengerIsPreservedTests
{
    [Fact]
    public void WhenIsFalseAndMessageIsReceived_MessageFiledShouldBeDefault()
    {
        //arrange
        Sender sender = new();
        Receiver receiver = new();
        GuidMessenger messenger = new();
        FieldInfo messageField = messenger
            .GetType()
            .BaseType!
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .First(field => field.Name == "_message");

        //act
        messenger.Send(sender);
        messenger.Receive(receiver);

        //assert
        messageField.GetValue(messenger).Should().Be((Guid)default);
    }

    [Fact]
    public void WhenIsTrueAndMessageIsReceived_MessageFiledShouldNotChange()
    {
        //arrange
        Sender sender = new();
        Receiver receiver = new();
        GuidMessenger messenger = new();
        FieldInfo messageField = messenger
            .GetType()
            .BaseType!
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .First(field => field.Name == "_message");

        messenger.IsMessagePreservedSwitch = true;

        //act
        messenger.Send(sender);
        messenger.Receive(receiver);

        //assert
        messageField.GetValue(messenger).Should().Be(sender.Value);
    }
}
